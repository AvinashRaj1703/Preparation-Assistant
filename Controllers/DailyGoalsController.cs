using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartStudyPlanner.Data;
using SmartStudyPlanner.Models;

namespace SmartStudyPlanner.Controllers;

[Authorize]
public class DailyGoalsController : BaseController
{
    private readonly ApplicationDbContext _context;

    public DailyGoalsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: DailyGoals
    public async Task<IActionResult> Index()
    {
        var goals = await _context.DailyGoals
            .Where(g => g.UserId == CurrentUserId)
            .OrderByDescending(g => g.GoalDate)
            .ToListAsync();

        var today = DateTime.Today;
        var todayGoal = goals.FirstOrDefault(g => g.GoalDate == today);

        // Sync completed minutes and tasks for today
        if (todayGoal != null)
        {
            var loggedMins = await _context.StudySessions
                .Where(s => s.UserId == CurrentUserId && s.StudyDate == today && s.Status == "Completed")
                .SumAsync(s => s.DurationMinutes);

            var doneTasks = await _context.Tasks
                .Include(t => t.Subject)
                .Where(t => t.Subject!.UserId == CurrentUserId && t.Status == "Completed" && t.CompletedAt.HasValue && t.CompletedAt.Value.Date == today)
                .CountAsync();

            todayGoal.CompletedStudyMinutes = loggedMins;
            todayGoal.CompletedTasks = doneTasks;

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

        // Fetch active tasks to review for today's goal
        var todayTasks = await _context.Tasks
            .Include(t => t.Subject)
            .Where(t => t.Subject!.UserId == CurrentUserId &&
                       (t.Status != "Completed" || (t.CompletedAt.HasValue && t.CompletedAt.Value.Date == today)))
            .OrderBy(t => t.Status == "Completed")
            .ThenBy(t => t.Deadline)
            .ToListAsync();

        // Fetch all tasks already added to subjects so user can pick them without duplicate name conflicts
        var allSubjectTasks = await _context.Tasks
            .Include(t => t.Subject)
            .Where(t => t.Subject!.UserId == CurrentUserId && t.Subject.IsActive)
            .OrderBy(t => t.Subject!.SubjectName)
            .ThenBy(t => t.Title)
            .ToListAsync();

        ViewBag.TodayTasks = todayTasks;
        ViewBag.AllSubjectTasks = allSubjectTasks;
        ViewBag.TodayGoal = todayGoal;
        ViewBag.AchievedGoalsCount = goals.Count(g => g.Status == "Achieved");
        ViewBag.TotalGoalsCount = goals.Count;

        return View(goals);
    }

    // POST: DailyGoals/AddExistingTaskToToday
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddExistingTaskToToday(int taskId)
    {
        var task = await _context.Tasks
            .Include(t => t.Subject)
            .FirstOrDefaultAsync(t => t.TaskId == taskId && t.Subject!.UserId == CurrentUserId);

        if (task != null)
        {
            // Set deadline to today so it appears in today's active goals
            task.Deadline = DateTime.Today;
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Task '{task.Title}' from {task.Subject?.SubjectName} added to today's goals!";
        }
        else
        {
            TempData["ErrorMessage"] = "Selected task could not be found.";
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: DailyGoals/CreateOrEditToday
    public async Task<IActionResult> CreateOrEditToday()
    {
        var today = DateTime.Today;
        var goal = await _context.DailyGoals
            .FirstOrDefaultAsync(g => g.UserId == CurrentUserId && g.GoalDate == today);

        if (goal == null)
        {
            goal = new DailyGoal
            {
                UserId = CurrentUserId,
                GoalDate = today,
                TargetStudyMinutes = 120,
                TargetTasks = 3,
                Status = "Pending"
            };
        }

        return View(goal);
    }

    // POST: DailyGoals/CreateOrEditToday
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateOrEditToday(DailyGoal goal)
    {
        var today = DateTime.Today;
        var existing = await _context.DailyGoals
            .FirstOrDefaultAsync(g => g.UserId == CurrentUserId && g.GoalDate == today);

        if (existing == null)
        {
            goal.UserId = CurrentUserId;
            goal.GoalDate = today;

            // calculate actual completed for today
            goal.CompletedStudyMinutes = await _context.StudySessions
                .Where(s => s.UserId == CurrentUserId && s.StudyDate == today && s.Status == "Completed")
                .SumAsync(s => s.DurationMinutes);

            goal.CompletedTasks = await _context.Tasks
                .Include(t => t.Subject)
                .Where(t => t.Subject!.UserId == CurrentUserId && t.Status == "Completed" && t.CompletedAt.HasValue && t.CompletedAt.Value.Date == today)
                .CountAsync();

            if (goal.CompletedStudyMinutes >= goal.TargetStudyMinutes && goal.CompletedTasks >= goal.TargetTasks && goal.TargetTasks > 0)
            {
                goal.Status = "Achieved";
            }
            else if (goal.CompletedStudyMinutes > 0 || goal.CompletedTasks > 0)
            {
                goal.Status = "In Progress";
            }
            else
            {
                goal.Status = "Pending";
            }

            _context.DailyGoals.Add(goal);
        }
        else
        {
            existing.TargetStudyMinutes = goal.TargetStudyMinutes;
            existing.TargetTasks = goal.TargetTasks;
            existing.Notes = goal.Notes;

            if (existing.CompletedStudyMinutes >= existing.TargetStudyMinutes && existing.CompletedTasks >= existing.TargetTasks && existing.TargetTasks > 0)
            {
                existing.Status = "Achieved";
            }
            else if (existing.CompletedStudyMinutes > 0 || existing.CompletedTasks > 0)
            {
                existing.Status = "In Progress";
            }
            else
            {
                existing.Status = "Pending";
            }
        }

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Today's daily study goal updated!";
        return RedirectToAction(nameof(Index));
    }

    // POST: DailyGoals/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var goal = await _context.DailyGoals
            .FirstOrDefaultAsync(g => g.GoalId == id && g.UserId == CurrentUserId);

        if (goal != null)
        {
            _context.DailyGoals.Remove(goal);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Goal entry deleted.";
        }

        return RedirectToAction(nameof(Index));
    }
}

