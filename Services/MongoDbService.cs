using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using SmartStudyPlanner.Data;
using SmartStudyPlanner.Models;

namespace SmartStudyPlanner.Services;

public interface IMongoDbService
{
    bool IsConfigured { get; }
    string DatabaseName { get; }
    string MaskedConnectionString { get; }
    Task<(bool IsConnected, string Message, long LatencyMs)> PingAsync();
    Task<(bool Success, string Message, int TotalDocumentsSynced)> SyncAllDataFromEfAsync(ApplicationDbContext efContext);
    IMongoDatabase? GetDatabase();
    IMongoCollection<T> GetCollection<T>(string collectionName);
}

public class MongoDbService : IMongoDbService
{
    private readonly MongoClient? _client;
    private readonly IMongoDatabase? _database;
    private readonly string _connectionString;
    private readonly string _databaseName;
    private readonly ILogger<MongoDbService> _logger;

    public bool IsConfigured => !string.IsNullOrWhiteSpace(_connectionString);
    public string DatabaseName => _databaseName;

    public string MaskedConnectionString
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_connectionString)) return "Not configured";
            try
            {
                var uri = new Uri(_connectionString);
                var userInfo = uri.UserInfo;
                if (!string.IsNullOrEmpty(userInfo) && userInfo.Contains(':'))
                {
                    var user = userInfo.Split(':')[0];
                    return _connectionString.Replace(userInfo, $"{user}:********");
                }
            }
            catch
            {
                // Simple regex/substring fallback
                int atIdx = _connectionString.IndexOf('@');
                int slashIdx = _connectionString.IndexOf("://");
                if (atIdx > 0 && slashIdx > 0)
                {
                    return _connectionString.Substring(0, slashIdx + 3) + "***:***" + _connectionString.Substring(atIdx);
                }
            }
            return "mongodb+srv://***:***@cluster0...";
        }
    }

    public MongoDbService(IConfiguration configuration, ILogger<MongoDbService> logger)
    {
        _logger = logger;
        _connectionString = configuration.GetConnectionString("MongoDbConnection") ?? string.Empty;
        _databaseName = configuration.GetValue<string>("MongoDbDatabaseName") ?? "SmartStudyPlannerDb";

        if (!string.IsNullOrWhiteSpace(_connectionString))
        {
            try
            {
                var settings = MongoClientSettings.FromConnectionString(_connectionString);
                settings.ServerSelectionTimeout = TimeSpan.FromSeconds(5);
                settings.ConnectTimeout = TimeSpan.FromSeconds(5);
                _client = new MongoClient(settings);
                _database = _client.GetDatabase(_databaseName);
                _logger.LogInformation("MongoDB client initialized for database '{DatabaseName}'", _databaseName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize MongoDB client with provided connection string");
            }
        }
    }

    public IMongoDatabase? GetDatabase() => _database;

    public IMongoCollection<T> GetCollection<T>(string collectionName)
    {
        if (_database == null)
            throw new InvalidOperationException("MongoDB is not configured or connection failed.");

        return _database.GetCollection<T>(collectionName);
    }

    public async Task<(bool IsConnected, string Message, long LatencyMs)> PingAsync()
    {
        if (_database == null)
        {
            return (false, "MongoDB connection string is not configured in appsettings.json.", 0);
        }

        var sw = Stopwatch.StartNew();
        try
        {
            // Execute ping command
            var command = new BsonDocument("ping", 1);
            await _database.RunCommandAsync<BsonDocument>(command);
            sw.Stop();
            return (true, $"Successfully connected to MongoDB Atlas! Cluster is healthy and responding.", sw.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, "MongoDB ping failed");
            return (false, $"Connection Error: {ex.Message}", sw.ElapsedMilliseconds);
        }
    }

    public async Task<(bool Success, string Message, int TotalDocumentsSynced)> SyncAllDataFromEfAsync(ApplicationDbContext efContext)
    {
        if (_database == null)
        {
            return (false, "MongoDB is not configured.", 0);
        }

        try
        {
            int totalSynced = 0;

            // 1. Sync Users
            var users = await efContext.Users.AsNoTracking().ToListAsync();
            var usersCollection = _database.GetCollection<BsonDocument>("users");
            await usersCollection.DeleteManyAsync(FilterDefinition<BsonDocument>.Empty);
            if (users.Any())
            {
                var userDocs = users.Select(u => new BsonDocument
                {
                    { "UserId", u.UserId },
                    { "FullName", u.FullName },
                    { "Email", u.Email },
                    { "PasswordHash", u.PasswordHash },
                    { "IsActive", u.IsActive },
                    { "SyncedAt", DateTime.UtcNow }
                });
                await usersCollection.InsertManyAsync(userDocs);
                totalSynced += users.Count;
            }

            // 2. Sync Subjects
            var subjects = await efContext.Subjects.AsNoTracking().ToListAsync();
            var subjectsCollection = _database.GetCollection<BsonDocument>("subjects");
            await subjectsCollection.DeleteManyAsync(FilterDefinition<BsonDocument>.Empty);
            if (subjects.Any())
            {
                var subDocs = subjects.Select(s => new BsonDocument
                {
                    { "SubjectId", s.SubjectId },
                    { "UserId", s.UserId },
                    { "SubjectName", s.SubjectName },
                    { "IsActive", s.IsActive },
                    { "SyncedAt", DateTime.UtcNow }
                });
                await subjectsCollection.InsertManyAsync(subDocs);
                totalSynced += subjects.Count;
            }

            // 3. Sync Tasks
            var tasks = await efContext.Tasks.AsNoTracking().ToListAsync();
            var tasksCollection = _database.GetCollection<BsonDocument>("tasks");
            await tasksCollection.DeleteManyAsync(FilterDefinition<BsonDocument>.Empty);
            if (tasks.Any())
            {
                var taskDocs = tasks.Select(t => new BsonDocument
                {
                    { "TaskId", t.TaskId },
                    { "SubjectId", t.SubjectId },
                    { "Title", t.Title },
                    { "Description", t.Description ?? string.Empty },
                    { "TaskType", t.TaskType },
                    { "Deadline", t.Deadline },
                    { "Priority", t.Priority },
                    { "Difficulty", t.Difficulty },
                    { "Status", t.Status },
                    { "ProgressPercentage", t.ProgressPercentage },
                    { "EstimatedHours", (double)t.EstimatedHours },
                    { "ActualHours", (double)t.ActualHours },
                    { "CompletedAt", t.CompletedAt.HasValue ? BsonValue.Create(t.CompletedAt.Value) : BsonNull.Value },
                    { "CreatedAt", t.CreatedAt },
                    { "SyncedAt", DateTime.UtcNow }
                });
                await tasksCollection.InsertManyAsync(taskDocs);
                totalSynced += tasks.Count;
            }

            // 4. Sync Exams
            var exams = await efContext.Exams.AsNoTracking().ToListAsync();
            var examsCollection = _database.GetCollection<BsonDocument>("exams");
            await examsCollection.DeleteManyAsync(FilterDefinition<BsonDocument>.Empty);
            if (exams.Any())
            {
                var examDocs = exams.Select(e => new BsonDocument
                {
                    { "ExamId", e.ExamId },
                    { "SubjectId", e.SubjectId },
                    { "ExamName", e.ExamName },
                    { "ExamDate", e.ExamDate },
                    { "ExamTime", e.ExamTime },
                    { "TotalMarks", e.TotalMarks },
                    { "PassingMarks", e.PassingMarks },
                    { "Syllabus", e.Syllabus ?? string.Empty },
                    { "SyncedAt", DateTime.UtcNow }
                });
                await examsCollection.InsertManyAsync(examDocs);
                totalSynced += exams.Count;
            }

            // 5. Sync StudySessions
            var sessions = await efContext.StudySessions.AsNoTracking().ToListAsync();
            var sessionsCollection = _database.GetCollection<BsonDocument>("studysessions");
            await sessionsCollection.DeleteManyAsync(FilterDefinition<BsonDocument>.Empty);
            if (sessions.Any())
            {
                var sessionDocs = sessions.Select(s => new BsonDocument
                {
                    { "StudySessionId", s.StudySessionId },
                    { "UserId", s.UserId },
                    { "SubjectId", s.SubjectId },
                    { "TaskId", s.TaskId.HasValue ? BsonValue.Create(s.TaskId.Value) : BsonNull.Value },
                    { "Topic", s.Topic },
                    { "StudyDate", s.StudyDate },
                    { "StartTime", s.StartTime },
                    { "EndTime", s.EndTime ?? string.Empty },
                    { "DurationMinutes", s.DurationMinutes },
                    { "Status", s.Status },
                    { "Notes", s.Notes ?? string.Empty },
                    { "CreatedAt", s.CreatedAt },
                    { "SyncedAt", DateTime.UtcNow }
                });
                await sessionsCollection.InsertManyAsync(sessionDocs);
                totalSynced += sessions.Count;
            }

            // 6. Sync RevisionPlans
            var revisions = await efContext.RevisionPlans.AsNoTracking().ToListAsync();
            var revisionsCollection = _database.GetCollection<BsonDocument>("revisionplans");
            await revisionsCollection.DeleteManyAsync(FilterDefinition<BsonDocument>.Empty);
            if (revisions.Any())
            {
                var revDocs = revisions.Select(r => new BsonDocument
                {
                    { "RevisionPlanId", r.RevisionPlanId },
                    { "UserId", r.UserId },
                    { "SubjectId", r.SubjectId },
                    { "ExamId", r.ExamId.HasValue ? BsonValue.Create(r.ExamId.Value) : BsonNull.Value },
                    { "Topic", r.Topic },
                    { "RevisionDate", r.RevisionDate },
                    { "StartTime", r.StartTime ?? string.Empty },
                    { "DurationMinutes", r.DurationMinutes },
                    { "Status", r.Status },
                    { "Notes", r.Notes ?? string.Empty },
                    { "CreatedAt", r.CreatedAt },
                    { "SyncedAt", DateTime.UtcNow }
                });
                await revisionsCollection.InsertManyAsync(revDocs);
                totalSynced += revisions.Count;
            }

            // 7. Sync DailyGoals
            var goals = await efContext.DailyGoals.AsNoTracking().ToListAsync();
            var goalsCollection = _database.GetCollection<BsonDocument>("dailygoals");
            await goalsCollection.DeleteManyAsync(FilterDefinition<BsonDocument>.Empty);
            if (goals.Any())
            {
                var goalDocs = goals.Select(g => new BsonDocument
                {
                    { "GoalId", g.GoalId },
                    { "UserId", g.UserId },
                    { "GoalDate", g.GoalDate },
                    { "TargetStudyMinutes", g.TargetStudyMinutes },
                    { "CompletedStudyMinutes", g.CompletedStudyMinutes },
                    { "TargetTasks", g.TargetTasks },
                    { "CompletedTasks", g.CompletedTasks },
                    { "Status", g.Status },
                    { "Notes", g.Notes ?? string.Empty },
                    { "SyncedAt", DateTime.UtcNow }
                });
                await goalsCollection.InsertManyAsync(goalDocs);
                totalSynced += goals.Count;
            }

            // 8. Sync Notifications
            var notifications = await efContext.Notifications.AsNoTracking().ToListAsync();
            var notifsCollection = _database.GetCollection<BsonDocument>("notifications");
            await notifsCollection.DeleteManyAsync(FilterDefinition<BsonDocument>.Empty);
            if (notifications.Any())
            {
                var notifDocs = notifications.Select(n => new BsonDocument
                {
                    { "NotificationId", n.NotificationId },
                    { "UserId", n.UserId },
                    { "Title", n.Title },
                    { "Message", n.Message },
                    { "NotificationType", n.NotificationType },
                    { "NotificationDate", n.NotificationDate },
                    { "IsRead", n.IsRead },
                    { "SyncedAt", DateTime.UtcNow }
                });
                await notifsCollection.InsertManyAsync(notifDocs);
                totalSynced += notifications.Count;
            }

            return (true, $"Successfully synced all 8 modules ({totalSynced} total documents) to MongoDB Atlas!", totalSynced);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sync data to MongoDB Atlas");
            return (false, $"Sync error: {ex.Message}", 0);
        }
    }
}
