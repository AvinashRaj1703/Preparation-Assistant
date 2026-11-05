using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartStudyPlanner.Data;
using SmartStudyPlanner.Models;

namespace SmartStudyPlanner.Controllers;

[Authorize]
public class StudySessionsController : BaseController
{
    private readonly ApplicationDbContext _context;

    public StudySessionsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: StudySessions
    public async Task<IActionResult> Index(int? subjectId)
    {
        var query = _context.StudySessions
            .Include(s => s.Subject)
            .Include(s => s.Task)
            .Where(s => s.UserId == CurrentUserId);

        if (subjectId.HasValue && subjectId.Value > 0)
        {
            query = query.Where(s => s.SubjectId == subjectId.Value);
        }

        var sessions = await query.OrderByDescending(s => s.StudyDate)
                                  .ThenByDescending(s => s.StudySessionId)
                                  .ToListAsync();

        ViewBag.TotalLoggedMinutes = sessions.Where(s => s.Status == "Completed").Sum(s => s.DurationMinutes);
        ViewBag.TotalHours = Math.Round((double)ViewBag.TotalLoggedMinutes / 60.0, 1);
        ViewBag.Subjects = new SelectList(
            await _context.Subjects.Where(s => s.UserId == CurrentUserId && s.IsActive).OrderBy(s => s.SubjectName).ToListAsync(),
            "SubjectId", "SubjectName", subjectId);
        ViewBag.SelectedSubjectId = subjectId;

        return View(sessions);
    }

    // GET: StudySessions/Create
    public async Task<IActionResult> Create(int? subjectId, int? taskId)
    {
        await PopulateDropDownsAsync(subjectId, taskId);
        return View(new StudySession
        {
            UserId = CurrentUserId,
            SubjectId = subjectId ?? 0,
            TaskId = taskId,
            StudyDate = DateTime.Today,
            StartTime = DateTime.Now.ToString("HH:mm"),
            DurationMinutes = 60,
            Status = "Completed",
            CreatedAt = DateTime.Now
        });
    }

    // POST: StudySessions/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(StudySession session)
    {
        session.UserId = CurrentUserId;

        var subject = await _context.Subjects
            .FirstOrDefaultAsync(s => s.SubjectId == session.SubjectId && s.UserId == CurrentUserId);

        if (subject == null)
        {
            ModelState.AddModelError("SubjectId", "Selected subject is invalid.");
        }

        if (ModelState.IsValid)
        {
            session.CreatedAt = DateTime.Now;
            _context.StudySessions.Add(session);

            // If linked to a task, update the task's actual hours!
            if (session.TaskId.HasValue && session.TaskId.Value > 0 && session.Status == "Completed")
            {
                var task = await _context.Tasks.FindAsync(session.TaskId.Value);
                if (task != null)
                {
                    task.ActualHours += Math.Round((decimal)session.DurationMinutes / 60.0m, 2);
                }
            }

            // Sync with Today's DailyGoal if session is completed today
            if (session.StudyDate.Date == DateTime.Today && session.Status == "Completed")
            {
                var goal = await _context.DailyGoals
                    .FirstOrDefaultAsync(g => g.UserId == CurrentUserId && g.GoalDate == DateTime.Today);

                if (goal != null)
                {
                    goal.CompletedStudyMinutes += session.DurationMinutes;
                    if (goal.CompletedStudyMinutes >= goal.TargetStudyMinutes && 
                        goal.CompletedTasks >= goal.TargetTasks && 
                        goal.TargetTasks > 0)
                    {
                        goal.Status = "Achieved";
                    }
                    else if (goal.Status == "Pending")
                    {
                        goal.Status = "In Progress";
                    }
                }
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Study session for '{session.Topic}' logged successfully!";
            return RedirectToAction(nameof(Index));
        }

        await PopulateDropDownsAsync(session.SubjectId, session.TaskId);
        return View(session);
    }

    // GET: StudySessions/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var session = await _context.StudySessions
            .FirstOrDefaultAsync(s => s.StudySessionId == id && s.UserId == CurrentUserId);

        if (session == null) return NotFound();

        await PopulateDropDownsAsync(session.SubjectId, session.TaskId);
        return View(session);
    }

    // POST: StudySessions/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, StudySession session)
    {
        if (id != session.StudySessionId) return NotFound();

        var existing = await _context.StudySessions
            .FirstOrDefaultAsync(s => s.StudySessionId == id && s.UserId == CurrentUserId);

        if (existing == null) return NotFound();

        if (ModelState.IsValid)
        {
            existing.SubjectId = session.SubjectId;
            existing.TaskId = session.TaskId;
            existing.Topic = session.Topic.Trim();
            existing.StudyDate = session.StudyDate;
            existing.StartTime = session.StartTime;
            existing.EndTime = session.EndTime;
            existing.DurationMinutes = session.DurationMinutes;
            existing.Status = session.Status;
            existing.Notes = session.Notes;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Study session updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        await PopulateDropDownsAsync(session.SubjectId, session.TaskId);
        return View(session);
    }

    // POST: StudySessions/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var session = await _context.StudySessions
            .FirstOrDefaultAsync(s => s.StudySessionId == id && s.UserId == CurrentUserId);

        if (session != null)
        {
            _context.StudySessions.Remove(session);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Study session deleted.";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropDownsAsync(int? selectedSubject = null, int? selectedTask = null)
    {
        var subjects = await _context.Subjects
            .Where(s => s.UserId == CurrentUserId && s.IsActive)
            .OrderBy(s => s.SubjectName)
            .ToListAsync();

        ViewBag.SubjectId = new SelectList(subjects, "SubjectId", "SubjectName", selectedSubject);

        var tasks = await _context.Tasks
            .Include(t => t.Subject)
            .Where(t => t.Subject!.UserId == CurrentUserId && t.Status != "Completed")
            .OrderBy(t => t.Title)
            .ToListAsync();

        ViewBag.TaskId = new SelectList(tasks, "TaskId", "Title", selectedTask);
    }
}

