using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartStudyPlanner.Data;
using SmartStudyPlanner.Models.ViewModels;

namespace SmartStudyPlanner.Controllers;

[Authorize]
public class CalendarController : BaseController
{
    private readonly ApplicationDbContext _context;

    public CalendarController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetEvents()
    {
        int userId = CurrentUserId;
        var events = new List<CalendarEventViewModel>();

        // 1. Exams (Red / Rose)
        var exams = await _context.Exams
            .Include(e => e.Subject)
            .Where(e => e.Subject!.UserId == userId)
            .ToListAsync();

        foreach (var exam in exams)
        {
            events.Add(new CalendarEventViewModel
            {
                Id = $"exam_{exam.ExamId}",
                Title = $"📝 Exam: {exam.ExamName}",
                Start = exam.ExamDate.ToString("yyyy-MM-dd"),
                BackgroundColor = "#ef4444",
                BorderColor = "#dc2626",
                Type = "Exam",
                Description = $"{exam.Subject?.SubjectName} | Time: {exam.ExamTime} | Marks: {exam.TotalMarks}",
                Url = Url.Action("Details", "Exams", new { id = exam.ExamId }) ?? "/Exams"
            });
        }

        // 2. Tasks Due (Amber / Orange)
        var tasks = await _context.Tasks
            .Include(t => t.Subject)
            .Where(t => t.Subject!.UserId == userId)
            .ToListAsync();

        foreach (var task in tasks)
        {
            string statusIcon = task.Status == "Completed" ? "✅" : "⏳";
            events.Add(new CalendarEventViewModel
            {
                Id = $"task_{task.TaskId}",
                Title = $"{statusIcon} Due: {task.Title}",
                Start = task.Deadline.ToString("yyyy-MM-dd"),
                BackgroundColor = task.Status == "Completed" ? "#10b981" : "#f59e0b",
                BorderColor = task.Status == "Completed" ? "#059669" : "#d97706",
                Type = "Task",
                Description = $"{task.Subject?.SubjectName} | Priority: {task.Priority} | Status: {task.Status}",
                Url = Url.Action("Details", "Tasks", new { id = task.TaskId }) ?? "/Tasks"
            });
        }

        // 3. Study Sessions (Blue / Sky)
        var sessions = await _context.StudySessions
            .Include(s => s.Subject)
            .Where(s => s.UserId == userId)
            .ToListAsync();

        foreach (var session in sessions)
        {
            events.Add(new CalendarEventViewModel
            {
                Id = $"session_{session.StudySessionId}",
                Title = $"⏱️ Study: {session.Topic}",
                Start = session.StudyDate.ToString("yyyy-MM-dd"),
                BackgroundColor = "#3b82f6",
                BorderColor = "#2563eb",
                Type = "StudySession",
                Description = $"{session.Subject?.SubjectName} | {session.DurationMinutes} mins | Status: {session.Status}",
                Url = Url.Action("Index", "StudySessions") ?? "/StudySessions"
            });
        }

        // 4. Revision Plans (Purple / Indigo)
        var revisions = await _context.RevisionPlans
            .Include(r => r.Subject)
            .Where(r => r.UserId == userId)
            .ToListAsync();

        foreach (var rev in revisions)
        {
            events.Add(new CalendarEventViewModel
            {
                Id = $"rev_{rev.RevisionPlanId}",
                Title = $"🔄 Revise: {rev.Topic}",
                Start = rev.RevisionDate.ToString("yyyy-MM-dd"),
                BackgroundColor = "#8b5cf6",
                BorderColor = "#7c3aed",
                Type = "Revision",
                Description = $"{rev.Subject?.SubjectName} | {rev.DurationMinutes} mins | Status: {rev.Status}",
                Url = Url.Action("Index", "RevisionPlans") ?? "/RevisionPlans"
            });
        }

        return Json(events);
    }
}

