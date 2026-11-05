using System.Security.Claims;
using BCrypt.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartStudyPlanner.Data;
using SmartStudyPlanner.Models;
using SmartStudyPlanner.Models.ViewModels;

namespace SmartStudyPlanner.Controllers;

public class AccountController : BaseController
{
    private readonly ApplicationDbContext _context;

    public AccountController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Dashboard");
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == model.Email.ToLower());

        if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
        {
            ModelState.AddModelError(string.Empty, "Invalid email address or password.");
            return View(model);
        }

        if (!user.IsActive)
        {
            ModelState.AddModelError(string.Empty, "Your account has been deactivated. Please contact support.");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Email, user.Email)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = model.RememberMe,
            ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(14) : DateTimeOffset.UtcNow.AddHours(8)
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Dashboard");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Dashboard");
        }

        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        bool emailExists = await _context.Users
            .AnyAsync(u => u.Email.ToLower() == model.Email.ToLower());

        if (emailExists)
        {
            ModelState.AddModelError("Email", "An account with this email address already exists.");
            return View(model);
        }

        var user = new User
        {
            FullName = model.FullName.Trim(),
            Email = model.Email.Trim().ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
            IsActive = true
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Seed initial default subjects and initial daily goal for smooth onboarding
        var defaultSubjects = new List<Subject>
        {
            new() { UserId = user.UserId, SubjectName = "Core .NET & C#", IsActive = true },
            new() { UserId = user.UserId, SubjectName = "Data Structures", IsActive = true },
            new() { UserId = user.UserId, SubjectName = "Database Systems", IsActive = true }
        };
        _context.Subjects.AddRange(defaultSubjects);

        var initialGoal = new DailyGoal
        {
            UserId = user.UserId,
            GoalDate = DateTime.Today,
            TargetStudyMinutes = 120,
            CompletedStudyMinutes = 0,
            TargetTasks = 3,
            CompletedTasks = 0,
            Status = "Pending",
            Notes = "Welcome to Smart Study Planner! Set your daily goals and start tracking."
        };
        _context.DailyGoals.Add(initialGoal);

        var welcomeNotification = new Notification
        {
            UserId = user.UserId,
            Title = "Welcome to Smart Study Planner!",
            Message = "Get started by adding your subjects, assignments, upcoming exams, and logging study sessions.",
            NotificationType = "System",
            NotificationDate = DateTime.Now,
            IsRead = false
        };
        _context.Notifications.Add(welcomeNotification);

        await _context.SaveChangesAsync();

        // Sign in automatically
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Email, user.Email)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity));

        TempData["SuccessMessage"] = "Registration successful! Welcome to your Study Planner.";
        return RedirectToAction("Index", "Dashboard");
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Account");
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var user = await _context.Users
            .Include(u => u.Subjects)
            .Include(u => u.StudySessions)
            .Include(u => u.DailyGoals)
            .FirstOrDefaultAsync(u => u.UserId == CurrentUserId);

        if (user == null)
        {
            return NotFound();
        }

        ViewBag.TotalStudyHours = Math.Round((double)user.StudySessions.Sum(s => s.DurationMinutes) / 60.0, 1);
        ViewBag.TotalSubjects = user.Subjects.Count;
        ViewBag.TotalGoalsAchieved = user.DailyGoals.Count(g => g.Status == "Achieved");

        return View(user);
    }
}

