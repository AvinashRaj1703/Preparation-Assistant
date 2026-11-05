using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartStudyPlanner.Data;
using SmartStudyPlanner.Services;

namespace SmartStudyPlanner.Controllers;

[Authorize]
public class NotificationsController : BaseController
{
    private readonly ApplicationDbContext _context;
    private readonly INotificationService _notificationService;

    public NotificationsController(ApplicationDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    // GET: Notifications
    public async Task<IActionResult> Index()
    {
        var notifications = await _context.Notifications
            .Where(n => n.UserId == CurrentUserId)
            .OrderByDescending(n => n.NotificationDate)
            .ToListAsync();

        ViewBag.UnreadCount = notifications.Count(n => !n.IsRead);

        return View(notifications);
    }

    // POST: Notifications/MarkAsRead/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var notif = await _context.Notifications
            .FirstOrDefaultAsync(n => n.NotificationId == id && n.UserId == CurrentUserId);

        if (notif != null)
        {
            notif.IsRead = true;
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: Notifications/MarkAllAsRead
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAllAsRead()
    {
        await _notificationService.MarkAllAsReadAsync(CurrentUserId);
        TempData["SuccessMessage"] = "All notifications marked as read.";
        return RedirectToAction(nameof(Index));
    }

    // POST: Notifications/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var notif = await _context.Notifications
            .FirstOrDefaultAsync(n => n.NotificationId == id && n.UserId == CurrentUserId);

        if (notif != null)
        {
            _context.Notifications.Remove(notif);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Notification deleted.";
        }

        return RedirectToAction(nameof(Index));
    }
}

