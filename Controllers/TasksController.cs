using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartStudyPlanner.Data;
using SmartStudyPlanner.Models;

namespace SmartStudyPlanner.Controllers;

[Authorize]
public class TasksController : BaseController
{
    private readonly ApplicationDbContext _context;

    public TasksController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Tasks
    public async Task<IActionResult> Index(int? subjectId, string? status, string? priority, string? search)
    {
        var query = _context.Tasks
            .Include(t => t.Subject)
            .Where(t => t.Subject!.UserId == CurrentUserId);

        if (subjectId.HasValue && subjectId.Value > 0)
        {
            query = query.Where(t => t.SubjectId == subjectId.Value);
        }

        if (!string.IsNullOrWhiteSpace(status) && status != "All")
        {
            query = query.Where(t => t.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(priority) && priority != "All")
        {
            query = query.Where(t => t.Priority == priority);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(t => t.Title.ToLower().Contains(search.ToLower()) || 
                                     (t.Description != null && t.Description.ToLower().Contains(search.ToLower())));
        }

        var tasks = await query.OrderBy(t => t.Status == "Completed")
                               .ThenBy(t => t.Deadline)
                               .ThenByDescending(t => t.Priority)
                               .ToListAsync();

        ViewBag.Subjects = new SelectList(
            await _context.Subjects.Where(s => s.UserId == CurrentUserId && s.IsActive).OrderBy(s => s.SubjectName).ToListAsync(),
            "SubjectId", "SubjectName", subjectId);

        ViewBag.SelectedSubjectId = subjectId;
        ViewBag.SelectedStatus = status;
        ViewBag.SelectedPriority = priority;
        ViewBag.SearchTerm = search;

        return View(tasks);
    }

    // GET: Tasks/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var task = await _context.Tasks
            .Include(t => t.Subject)
            .Include(t => t.StudySessions)
            .FirstOrDefaultAsync(t => t.TaskId == id && t.Subject!.UserId == CurrentUserId);

        if (task == null) return NotFound();

        return View(task);
    }

    // GET: Tasks/Create
    public async Task<IActionResult> Create(string? from = null)
    {
        await PopulateSubjectsDropDownListAsync();
        await PopulateExistingTasksAndTypesAsync();
        ViewBag.FromSource = from;
        return View(new StudyTask
        {
            Deadline = from == "DailyGoals" ? DateTime.Today : DateTime.Today.AddDays(7),
            Priority = "Medium",
            Difficulty = "Medium",
            TaskType = "Assignment",
            Status = "Pending",
            ProgressPercentage = 0,
            EstimatedHours = 2.0m,
            CreatedAt = DateTime.Now
        });
    }

    // POST: Tasks/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(StudyTask task, string? from = null)
    {
        // Verify subject belongs to current user
        var subject = await _context.Subjects
            .FirstOrDefaultAsync(s => s.SubjectId == task.SubjectId && s.UserId == CurrentUserId);

        if (subject == null)
        {
            ModelState.AddModelError("SubjectId", "Selected subject is invalid.");
        }

        // Check for duplicate name conflict under the same subject
        bool nameConflict = await _context.Tasks
            .AnyAsync(t => t.SubjectId == task.SubjectId && t.Title.ToLower() == task.Title.Trim().ToLower());

        if (nameConflict)
        {
            ModelState.AddModelError("Title", "A task with this title already exists under the selected subject. To avoid name conflicts, pick from existing tasks or enter a unique title.");
        }

        if (ModelState.IsValid)
        {
            task.CreatedAt = DateTime.Now;
            if (task.Status == "Completed" && task.CompletedAt == null)
            {
                task.CompletedAt = DateTime.Now;
                task.ProgressPercentage = 100;
            }

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Task '{task.Title}' added successfully!";

            if (from == "DailyGoals")
            {
                return RedirectToAction("Index", "DailyGoals");
            }
            return RedirectToAction(nameof(Index));
        }

        await PopulateSubjectsDropDownListAsync(task.SubjectId);
        await PopulateExistingTasksAndTypesAsync();
        ViewBag.FromSource = from;
        return View(task);
    }

    // GET: Tasks/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var task = await _context.Tasks
            .Include(t => t.Subject)
            .FirstOrDefaultAsync(t => t.TaskId == id && t.Subject!.UserId == CurrentUserId);

        if (task == null) return NotFound();

        await PopulateSubjectsDropDownListAsync(task.SubjectId);
        return View(task);
    }

    // POST: Tasks/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, StudyTask task)
    {
        if (id != task.TaskId) return NotFound();

        var existing = await _context.Tasks
            .Include(t => t.Subject)
            .FirstOrDefaultAsync(t => t.TaskId == id && t.Subject!.UserId == CurrentUserId);

        if (existing == null) return NotFound();

        var subject = await _context.Subjects
            .FirstOrDefaultAsync(s => s.SubjectId == task.SubjectId && s.UserId == CurrentUserId);

        if (subject == null)
        {
            ModelState.AddModelError("SubjectId", "Selected subject is invalid.");
        }

        if (ModelState.IsValid)
        {
            existing.SubjectId = task.SubjectId;
            existing.Title = task.Title.Trim();
            existing.Description = task.Description;
            existing.TaskType = task.TaskType;
            existing.Deadline = task.Deadline;
            existing.Priority = task.Priority;
            existing.Difficulty = task.Difficulty;
            existing.EstimatedHours = task.EstimatedHours;
            existing.ActualHours = task.ActualHours;

            // Handle completion status
            if (task.Status == "Completed")
            {
                existing.Status = "Completed";
                existing.ProgressPercentage = 100;
                if (!existing.CompletedAt.HasValue)
                {
                    existing.CompletedAt = DateTime.Now;
                }
            }
            else
            {
                existing.Status = task.Status;
                existing.ProgressPercentage = task.ProgressPercentage;
                existing.CompletedAt = null;
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Task '{existing.Title}' updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        await PopulateSubjectsDropDownListAsync(task.SubjectId);
        return View(task);
    }

    // POST: Tasks/ToggleComplete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleComplete(int id, string? returnUrl = null)
    {
        var task = await _context.Tasks
            .Include(t => t.Subject)
            .FirstOrDefaultAsync(t => t.TaskId == id && t.Subject!.UserId == CurrentUserId);

        if (task == null) return NotFound();

        if (task.Status == "Completed")
        {
            task.Status = "In Progress";
            task.ProgressPercentage = 50;
            task.CompletedAt = null;
            TempData["SuccessMessage"] = $"Task '{task.Title}' marked as In Progress.";
        }
        else
        {
            task.Status = "Completed";
            task.ProgressPercentage = 100;
            task.CompletedAt = DateTime.Now;
            TempData["SuccessMessage"] = $"Great job! Task '{task.Title}' marked as Completed.";
        }

        await _context.SaveChangesAsync();

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: Tasks/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var task = await _context.Tasks
            .Include(t => t.Subject)
            .FirstOrDefaultAsync(t => t.TaskId == id && t.Subject!.UserId == CurrentUserId);

        if (task != null)
        {
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Task '{task.Title}' deleted.";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateSubjectsDropDownListAsync(object? selectedSubject = null)
    {
        var subjectsQuery = await _context.Subjects
            .Where(s => s.UserId == CurrentUserId && s.IsActive)
            .OrderBy(s => s.SubjectName)
            .ToListAsync();

        ViewBag.SubjectId = new SelectList(subjectsQuery, "SubjectId", "SubjectName", selectedSubject);
    }

    private async Task PopulateExistingTasksAndTypesAsync()
    {
        var existingTasks = await _context.Tasks
            .Include(t => t.Subject)
            .Where(t => t.Subject!.UserId == CurrentUserId)
            .OrderBy(t => t.Subject!.SubjectName)
            .ThenBy(t => t.Title)
            .ToListAsync();

        ViewBag.ExistingTasks = existingTasks;

        // Distinct task types already added to subjects by the user
        var distinctTypes = existingTasks
            .Select(t => t.TaskType)
            .Where(tt => !string.IsNullOrWhiteSpace(tt))
            .Distinct()
            .ToList();

        var defaultTypes = new List<string> { "Assignment", "Project", "Lab Work", "Reading", "Practice" };
        var allTypes = distinctTypes.Union(defaultTypes).Distinct().ToList();

        ViewBag.AvailableTaskTypes = allTypes;
    }
}

