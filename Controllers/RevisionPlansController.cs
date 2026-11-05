using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartStudyPlanner.Data;
using SmartStudyPlanner.Models;

namespace SmartStudyPlanner.Controllers;

[Authorize]
public class RevisionPlansController : BaseController
{
    private readonly ApplicationDbContext _context;

    public RevisionPlansController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: RevisionPlans
    public async Task<IActionResult> Index(int? subjectId, int? examId)
    {
        var query = _context.RevisionPlans
            .Include(r => r.Subject)
            .Include(r => r.Exam)
            .Where(r => r.UserId == CurrentUserId);

        if (subjectId.HasValue && subjectId.Value > 0)
        {
            query = query.Where(r => r.SubjectId == subjectId.Value);
        }

        if (examId.HasValue && examId.Value > 0)
        {
            query = query.Where(r => r.ExamId == examId.Value);
        }

        var plans = await query.OrderBy(r => r.Status == "Completed")
                               .ThenBy(r => r.RevisionDate)
                               .ToListAsync();

        ViewBag.Subjects = new SelectList(
            await _context.Subjects.Where(s => s.UserId == CurrentUserId && s.IsActive).OrderBy(s => s.SubjectName).ToListAsync(),
            "SubjectId", "SubjectName", subjectId);

        ViewBag.Exams = new SelectList(
            await _context.Exams.Include(e => e.Subject).Where(e => e.Subject!.UserId == CurrentUserId).OrderBy(e => e.ExamDate).ToListAsync(),
            "ExamId", "ExamName", examId);

        return View(plans);
    }

    // GET: RevisionPlans/Create
    public async Task<IActionResult> Create(int? examId, int? subjectId)
    {
        await PopulateDropDownsAsync(subjectId, examId);
        return View(new RevisionPlan
        {
            UserId = CurrentUserId,
            SubjectId = subjectId ?? 0,
            ExamId = examId,
            RevisionDate = DateTime.Today.AddDays(2),
            StartTime = "10:00",
            DurationMinutes = 45,
            Status = "Scheduled",
            CreatedAt = DateTime.Now
        });
    }

    // POST: RevisionPlans/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RevisionPlan plan)
    {
        plan.UserId = CurrentUserId;

        var subject = await _context.Subjects
            .FirstOrDefaultAsync(s => s.SubjectId == plan.SubjectId && s.UserId == CurrentUserId);

        if (subject == null)
        {
            ModelState.AddModelError("SubjectId", "Selected subject is invalid.");
        }

        if (ModelState.IsValid)
        {
            plan.CreatedAt = DateTime.Now;
            _context.RevisionPlans.Add(plan);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Revision topic '{plan.Topic}' scheduled!";
            return RedirectToAction(nameof(Index));
        }

        await PopulateDropDownsAsync(plan.SubjectId, plan.ExamId);
        return View(plan);
    }

    // GET: RevisionPlans/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var plan = await _context.RevisionPlans
            .FirstOrDefaultAsync(r => r.RevisionPlanId == id && r.UserId == CurrentUserId);

        if (plan == null) return NotFound();

        await PopulateDropDownsAsync(plan.SubjectId, plan.ExamId);
        return View(plan);
    }

    // POST: RevisionPlans/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, RevisionPlan plan)
    {
        if (id != plan.RevisionPlanId) return NotFound();

        var existing = await _context.RevisionPlans
            .FirstOrDefaultAsync(r => r.RevisionPlanId == id && r.UserId == CurrentUserId);

        if (existing == null) return NotFound();

        if (ModelState.IsValid)
        {
            existing.SubjectId = plan.SubjectId;
            existing.ExamId = plan.ExamId;
            existing.Topic = plan.Topic.Trim();
            existing.RevisionDate = plan.RevisionDate;
            existing.StartTime = plan.StartTime;
            existing.DurationMinutes = plan.DurationMinutes;
            existing.Status = plan.Status;
            existing.Notes = plan.Notes;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Revision plan updated!";
            return RedirectToAction(nameof(Index));
        }

        await PopulateDropDownsAsync(plan.SubjectId, plan.ExamId);
        return View(plan);
    }

    // POST: RevisionPlans/ToggleStatus/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var plan = await _context.RevisionPlans
            .FirstOrDefaultAsync(r => r.RevisionPlanId == id && r.UserId == CurrentUserId);

        if (plan == null) return NotFound();

        if (plan.Status == "Completed")
        {
            plan.Status = "Scheduled";
        }
        else
        {
            plan.Status = "Completed";
        }

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = $"Revision status updated to '{plan.Status}'.";
        return RedirectToAction(nameof(Index));
    }

    // POST: RevisionPlans/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var plan = await _context.RevisionPlans
            .FirstOrDefaultAsync(r => r.RevisionPlanId == id && r.UserId == CurrentUserId);

        if (plan != null)
        {
            _context.RevisionPlans.Remove(plan);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Revision plan deleted.";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropDownsAsync(int? selectedSubject = null, int? selectedExam = null)
    {
        var subjects = await _context.Subjects
            .Where(s => s.UserId == CurrentUserId && s.IsActive)
            .OrderBy(s => s.SubjectName)
            .ToListAsync();

        ViewBag.SubjectId = new SelectList(subjects, "SubjectId", "SubjectName", selectedSubject);

        var exams = await _context.Exams
            .Include(e => e.Subject)
            .Where(e => e.Subject!.UserId == CurrentUserId)
            .OrderBy(e => e.ExamDate)
            .ToListAsync();

        ViewBag.ExamId = new SelectList(exams, "ExamId", "ExamName", selectedExam);
    }
}

