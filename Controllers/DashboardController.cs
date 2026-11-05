using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartStudyPlanner.Data;
using SmartStudyPlanner.Models;
using SmartStudyPlanner.Models.ViewModels;
using SmartStudyPlanner.Services;

namespace SmartStudyPlanner.Controllers;

[Authorize]
public class DashboardController : BaseController
{
    private readonly ApplicationDbContext _context;
    private readonly INotificationService _notificationService;

    public DashboardController(ApplicationDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<IActionResult> Index()
    {
        int userId = CurrentUserId;

        // Auto check and trigger smart reminders (exams & deadlines)
        await _notificationService.CheckAndGenerateRemindersAsync(userId);

        var today = DateTime.Today;

        // Get or initialize today's daily goal
        var todayGoal = await _context.DailyGoals
            .FirstOrDefaultAsync(g => g.UserId == userId && g.GoalDate == today);

        if (todayGoal == null)
        {
            // Auto create today's goal container
            todayGoal = new DailyGoal
            {
                UserId = userId,
                GoalDate = today,
                TargetStudyMinutes = 120,
                CompletedStudyMinutes = 0,
                TargetTasks = 3,
                CompletedTasks = 0,
                Status = "Pending",
                Notes = "Daily goal created automatically."
            };
            _context.DailyGoals.Add(todayGoal);
            await _context.SaveChangesAsync();
        }

        // Sync today's completed study minutes from actual StudySessions logged today
        var todayLoggedMinutes = await _context.StudySessions
            .Where(s => s.UserId == userId && s.StudyDate == today && s.Status == "Completed")
            .SumAsync(s => s.DurationMinutes);

        var todayCompletedTasks = await _context.Tasks
            .Include(t => t.Subject)
            .Where(t => t.Subject!.UserId == userId && t.Status == "Completed" && t.CompletedAt.HasValue && t.CompletedAt.Value.Date == today)
            .CountAsync();

        if (todayGoal.CompletedStudyMinutes != todayLoggedMinutes || todayGoal.CompletedTasks != todayCompletedTasks)
        {
            todayGoal.CompletedStudyMinutes = todayLoggedMinutes;
            todayGoal.CompletedTasks = todayCompletedTasks;

            if (todayGoal.CompletedStudyMinutes >= todayGoal.TargetStudyMinutes && 
                todayGoal.CompletedTasks >= todayGoal.TargetTasks && 
                todayGoal.TargetTasks > 0)
            {
                todayGoal.Status = "Achieved";
            }
            else if (todayGoal.CompletedStudyMinutes > 0 || todayGoal.CompletedTasks > 0)
            {
                todayGoal.Status = "In Progress";
            }
            await _context.SaveChangesAsync();
        }

        // Summary KPI Counts
        int totalSubjects = await _context.Subjects.CountAsync(s => s.UserId == userId && s.IsActive);

        int activeTasksCount = await _context.Tasks
            .Include(t => t.Subject)
            .CountAsync(t => t.Subject!.UserId == userId && t.Status != "Completed");

        int completedTasksCount = await _context.Tasks
            .Include(t => t.Subject)
            .CountAsync(t => t.Subject!.UserId == userId && t.Status == "Completed");

        int upcomingExamsCount = await _context.Exams
            .Include(e => e.Subject)
            .CountAsync(e => e.Subject!.UserId == userId && e.ExamDate >= today);

        int totalStudyMinutes = await _context.StudySessions
            .Where(s => s.UserId == userId && s.Status == "Completed")
            .SumAsync(s => s.DurationMinutes);

        int unreadNotifs = await _notificationService.GetUnreadCountAsync(userId);

        // Next Upcoming Exam
        var nextExam = await _context.Exams
            .Include(e => e.Subject)
            .Where(e => e.Subject!.UserId == userId && e.ExamDate >= today)
            .OrderBy(e => e.ExamDate)
            .FirstOrDefaultAsync();

        int? daysUntilNextExam = nextExam != null ? (nextExam.ExamDate.Date - today).Days : null;

        // Upcoming Tasks (next 5)
        var upcomingTasks = await _context.Tasks
            .Include(t => t.Subject)
            .Where(t => t.Subject!.UserId == userId && t.Status != "Completed")
            .OrderBy(t => t.Deadline)
            .Take(5)
            .ToListAsync();

        // Upcoming Exams (next 4)
        var upcomingExams = await _context.Exams
            .Include(e => e.Subject)
            .Where(e => e.Subject!.UserId == userId && e.ExamDate >= today)
            .OrderBy(e => e.ExamDate)
            .Take(4)
            .ToListAsync();

        // Recent Study Sessions (last 5)
        var recentSessions = await _context.StudySessions
            .Include(s => s.Subject)
            .Include(s => s.Task)
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.StudyDate)
            .ThenByDescending(s => s.StudySessionId)
            .Take(5)
            .ToListAsync();

        // Upcoming Revisions (next 4)
        var upcomingRevisions = await _context.RevisionPlans
            .Include(r => r.Subject)
            .Include(r => r.Exam)
            .Where(r => r.UserId == userId && r.RevisionDate >= today)
            .OrderBy(r => r.RevisionDate)
            .Take(4)
            .ToListAsync();

        // Recent Notifications (last 4)
        var recentNotifications = await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.NotificationDate)
            .Take(4)
            .ToListAsync();

        // Weekly Study Minutes Chart Data (Last 7 Days)
        var weeklyLabels = new List<string>();
        var weeklyMinutes = new List<int>();

        for (int i = 6; i >= 0; i--)
        {
            var date = today.AddDays(-i);
            weeklyLabels.Add(date.ToString("ddd (MMM dd)"));

            var mins = await _context.StudySessions
                .Where(s => s.UserId == userId && s.StudyDate == date && s.Status == "Completed")
                .SumAsync(s => s.DurationMinutes);

            weeklyMinutes.Add(mins);
        }

        // Subject Study Time Distribution Chart Data
        var subjectMinutes = await _context.StudySessions
            .Include(s => s.Subject)
            .Where(s => s.UserId == userId && s.Status == "Completed" && s.Subject != null)
            .GroupBy(s => s.Subject!.SubjectName)
            .Select(g => new { SubjectName = g.Key, TotalMinutes = g.Sum(x => x.DurationMinutes) })
            .ToListAsync();

        var subjectLabels = subjectMinutes.Select(x => x.SubjectName).ToList();
        var subjectData = subjectMinutes.Select(x => x.TotalMinutes).ToList();

        // Task Status counts
        int pendingTasks = await _context.Tasks
            .Include(t => t.Subject)
            .CountAsync(t => t.Subject!.UserId == userId && t.Status == "Pending");

        int inProgressTasks = await _context.Tasks
            .Include(t => t.Subject)
            .CountAsync(t => t.Subject!.UserId == userId && t.Status == "In Progress");

        var viewModel = new DashboardViewModel
        {
            TotalSubjects = totalSubjects,
            ActiveTasksCount = activeTasksCount,
            CompletedTasksCount = completedTasksCount,
            UpcomingExamsCount = upcomingExamsCount,
            TotalStudyMinutes = totalStudyMinutes,
            UnreadNotificationsCount = unreadNotifs,
            TodayGoal = todayGoal,
            NextExam = nextExam,
            DaysUntilNextExam = daysUntilNextExam,
            UpcomingTasks = upcomingTasks,
            UpcomingExams = upcomingExams,
            RecentSessions = recentSessions,
            UpcomingRevisions = upcomingRevisions,
            RecentNotifications = recentNotifications,
            WeeklyLabels = weeklyLabels,
            WeeklyStudyMinutes = weeklyMinutes,
            SubjectChartLabels = subjectLabels,
            SubjectChartMinutes = subjectData,
            TasksPendingCount = pendingTasks,
            TasksInProgressCount = inProgressTasks
        };

        return View(viewModel);
    }
}

