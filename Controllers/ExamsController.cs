using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartStudyPlanner.Data;
using SmartStudyPlanner.Models;

namespace SmartStudyPlanner.Controllers;

[Authorize]
public class ExamsController : BaseController
{
    private readonly ApplicationDbContext _context;

    public ExamsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Exams
    public async Task<IActionResult> Index()
    {
        var exams = await _context.Exams
            .Include(e => e.Subject)
            .Include(e => e.RevisionPlans)
            .Where(e => e.Subject!.UserId == CurrentUserId)
            .OrderBy(e => e.ExamDate)
            .ToListAsync();

        return View(exams);
    }

    // GET: Exams/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var exam = await _context.Exams
            .Include(e => e.Subject)
            .Include(e => e.RevisionPlans)
            .FirstOrDefaultAsync(e => e.ExamId == id && e.Subject!.UserId == CurrentUserId);

        if (exam == null) return NotFound();

        return View(exam);
    }

    // GET: Exams/Create
    public async Task<IActionResult> Create()
    {
        await PopulateSubjectsDropDownListAsync();
        return View(new Exam
        {
            ExamDate = DateTime.Today.AddDays(14),
            ExamTime = "10:00 AM - 01:00 PM",
            TotalMarks = 100,
            PassingMarks = 40
        });
    }

    // POST: Exams/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Exam exam)
    {
        var subject = await _context.Subjects
            .FirstOrDefaultAsync(s => s.SubjectId == exam.SubjectId && s.UserId == CurrentUserId);

        if (subject == null)
        {
            ModelState.AddModelError("SubjectId", "Selected subject is invalid.");
        }

        if (ModelState.IsValid)
        {
            _context.Exams.Add(exam);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Exam '{exam.ExamName}' scheduled successfully!";
            return RedirectToAction(nameof(Index));
        }

        await PopulateSubjectsDropDownListAsync(exam.SubjectId);
        return View(exam);
    }

    // GET: Exams/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var exam = await _context.Exams
            .Include(e => e.Subject)
            .FirstOrDefaultAsync(e => e.ExamId == id && e.Subject!.UserId == CurrentUserId);

        if (exam == null) return NotFound();

        await PopulateSubjectsDropDownListAsync(exam.SubjectId);
        return View(exam);
    }

    // POST: Exams/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Exam exam)
    {
        if (id != exam.ExamId) return NotFound();

        var existing = await _context.Exams
            .Include(e => e.Subject)
            .FirstOrDefaultAsync(e => e.ExamId == id && e.Subject!.UserId == CurrentUserId);

        if (existing == null) return NotFound();

        var subject = await _context.Subjects
            .FirstOrDefaultAsync(s => s.SubjectId == exam.SubjectId && s.UserId == CurrentUserId);

        if (subject == null)
        {
            ModelState.AddModelError("SubjectId", "Selected subject is invalid.");
        }

        if (ModelState.IsValid)
        {
            existing.SubjectId = exam.SubjectId;
            existing.ExamName = exam.ExamName.Trim();
            existing.ExamDate = exam.ExamDate;
            existing.ExamTime = exam.ExamTime;
            existing.TotalMarks = exam.TotalMarks;
            existing.PassingMarks = exam.PassingMarks;
            existing.Syllabus = exam.Syllabus;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Exam '{existing.ExamName}' updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        await PopulateSubjectsDropDownListAsync(exam.SubjectId);
        return View(exam);
    }

    // POST: Exams/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var exam = await _context.Exams
            .Include(e => e.Subject)
            .FirstOrDefaultAsync(e => e.ExamId == id && e.Subject!.UserId == CurrentUserId);

        if (exam != null)
        {
            _context.Exams.Remove(exam);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Exam '{exam.ExamName}' deleted.";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateSubjectsDropDownListAsync(object? selectedSubject = null)
    {
        var subjects = await _context.Subjects
            .Where(s => s.UserId == CurrentUserId && s.IsActive)
            .OrderBy(s => s.SubjectName)
            .ToListAsync();

        ViewBag.SubjectId = new SelectList(subjects, "SubjectId", "SubjectName", selectedSubject);
    }
}

