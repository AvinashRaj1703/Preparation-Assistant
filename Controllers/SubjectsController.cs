using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartStudyPlanner.Data;
using SmartStudyPlanner.Models;

namespace SmartStudyPlanner.Controllers;

[Authorize]
public class SubjectsController : BaseController
{
    private readonly ApplicationDbContext _context;

    public SubjectsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Subjects
    public async Task<IActionResult> Index()
    {
        var subjects = await _context.Subjects
            .Include(s => s.Tasks)
            .Include(s => s.Exams)
            .Include(s => s.StudySessions)
            .Where(s => s.UserId == CurrentUserId)
            .OrderByDescending(s => s.IsActive)
            .ThenBy(s => s.SubjectName)
            .ToListAsync();

        return View(subjects);
    }

    // GET: Subjects/Create
    public IActionResult Create()
    {
        return View(new Subject { UserId = CurrentUserId, IsActive = true });
    }

    // POST: Subjects/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Subject subject)
    {
        subject.UserId = CurrentUserId;

        if (string.IsNullOrWhiteSpace(subject.SubjectName))
        {
            ModelState.AddModelError("SubjectName", "Subject name is required.");
        }

        if (ModelState.IsValid)
        {
            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Subject '{subject.SubjectName}' added successfully!";
            return RedirectToAction(nameof(Index));
        }

        return View(subject);
    }

    // GET: Subjects/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var subject = await _context.Subjects
            .FirstOrDefaultAsync(s => s.SubjectId == id && s.UserId == CurrentUserId);

        if (subject == null) return NotFound();

        return View(subject);
    }

    // POST: Subjects/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Subject subject)
    {
        if (id != subject.SubjectId) return NotFound();

        var existing = await _context.Subjects
            .FirstOrDefaultAsync(s => s.SubjectId == id && s.UserId == CurrentUserId);

        if (existing == null) return NotFound();

        if (string.IsNullOrWhiteSpace(subject.SubjectName))
        {
            ModelState.AddModelError("SubjectName", "Subject name is required.");
        }

        if (ModelState.IsValid)
        {
            existing.SubjectName = subject.SubjectName.Trim();
            existing.IsActive = subject.IsActive;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Subject '{existing.SubjectName}' updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        return View(subject);
    }

    // POST: Subjects/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var subject = await _context.Subjects
            .Include(s => s.Tasks)
            .Include(s => s.Exams)
            .Include(s => s.StudySessions)
            .Include(s => s.RevisionPlans)
            .FirstOrDefaultAsync(s => s.SubjectId == id && s.UserId == CurrentUserId);

        if (subject != null)
        {
            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Subject '{subject.SubjectName}' and its related items were deleted.";
        }

        return RedirectToAction(nameof(Index));
    }
}

