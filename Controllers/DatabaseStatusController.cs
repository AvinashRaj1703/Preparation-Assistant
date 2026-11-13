using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using SmartStudyPlanner.Data;
using SmartStudyPlanner.Services;

namespace SmartStudyPlanner.Controllers;

[Authorize]
public class DatabaseStatusController : BaseController
{
    private readonly ApplicationDbContext _efContext;
    private readonly IMongoDbService _mongoService;
    private readonly IConfiguration _configuration;

    public DatabaseStatusController(ApplicationDbContext efContext, IMongoDbService mongoService, IConfiguration configuration)
    {
        _efContext = efContext;
        _mongoService = mongoService;
        _configuration = configuration;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewBag.ActiveProvider = _configuration.GetValue<string>("DatabaseProvider") ?? "Sqlite";
        ViewBag.MaskedMongoUri = _mongoService.MaskedConnectionString;
        ViewBag.MongoDbName = _mongoService.DatabaseName;
        ViewBag.IsConfigured = _mongoService.IsConfigured;

        // Perform live ping
        var pingResult = await _mongoService.PingAsync();
        ViewBag.IsConnected = pingResult.IsConnected;
        ViewBag.PingMessage = pingResult.Message;
        ViewBag.LatencyMs = pingResult.LatencyMs;

        // Count documents in MongoDB collections if connected
        var collectionCounts = new Dictionary<string, long>();
        if (pingResult.IsConnected && _mongoService.GetDatabase() != null)
        {
            var db = _mongoService.GetDatabase()!;
            string[] collections = { "users", "subjects", "tasks", "exams", "studysessions", "revisionplans", "dailygoals", "notifications" };
            foreach (var collName in collections)
            {
                try
                {
                    var count = await db.GetCollection<BsonDocument>(collName).CountDocumentsAsync(FilterDefinition<BsonDocument>.Empty);
                    collectionCounts[collName] = count;
                }
                catch
                {
                    collectionCounts[collName] = 0;
                }
            }
        }
        ViewBag.CollectionCounts = collectionCounts;

        // Local Relational Counts
        ViewBag.LocalUsersCount = await _efContext.Users.CountAsync();
        ViewBag.LocalSubjectsCount = await _efContext.Subjects.CountAsync();
        ViewBag.LocalTasksCount = await _efContext.Tasks.CountAsync();
        ViewBag.LocalExamsCount = await _efContext.Exams.CountAsync();
        ViewBag.LocalSessionsCount = await _efContext.StudySessions.CountAsync();
        ViewBag.LocalRevisionsCount = await _efContext.RevisionPlans.CountAsync();
        ViewBag.LocalGoalsCount = await _efContext.DailyGoals.CountAsync();
        ViewBag.LocalNotifsCount = await _efContext.Notifications.CountAsync();

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SyncToMongo()
    {
        var result = await _mongoService.SyncAllDataFromEfAsync(_efContext);
        if (result.Success)
        {
            TempData["SuccessMessage"] = result.Message;
        }
        else
        {
            TempData["ErrorMessage"] = result.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}
