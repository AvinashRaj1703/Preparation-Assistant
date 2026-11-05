using Microsoft.EntityFrameworkCore;
using SmartStudyPlanner.Data;
using SmartStudyPlanner.Models;

namespace SmartStudyPlanner.Services;

public interface INotificationService
{
    Task CheckAndGenerateRemindersAsync(int userId);
    Task<int> GetUnreadCountAsync(int userId);
    Task MarkAllAsReadAsync(int userId);
}

public class NotificationService : INotificationService
{
    private readonly ApplicationDbContext _context;

    public NotificationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task CheckAndGenerateRemindersAsync(int userId)
    {
        var today = DateTime.Today;

        // 1. Check for upcoming exams in the next 14 days
        var upcomingExams = await _context.Exams
            .Include(e => e.Subject)
            .Where(e => e.Subject!.UserId == userId && e.ExamDate >= today && e.ExamDate <= today.AddDays(14))
            .ToListAsync();

        foreach (var exam in upcomingExams)
        {
            var daysLeft = (exam.ExamDate.Date - today).Days;
            var reminderTitle = $"Exam Reminder: {exam.ExamName}";
            
            // Check if notification was already sent today for this exam
            bool exists = await _context.Notifications
                .AnyAsync(n => n.UserId == userId && 
                               n.Title == reminderTitle && 
                               n.NotificationDate.Date == today);

            if (!exists)
            {
                string msg = daysLeft == 0 
                    ? $"Today is your '{exam.ExamName}' exam ({exam.ExamTime}) for {exam.Subject?.SubjectName}! Best of luck!"
                    : $"Your '{exam.ExamName}' exam is in {daysLeft} day{(daysLeft > 1 ? "s" : "")} on {exam.ExamDate:MMM dd, yyyy}.";

                _context.Notifications.Add(new Notification
                {
                    UserId = userId,
                    Title = reminderTitle,
                    Message = msg,
                    NotificationType = "ExamCountdown",
                    NotificationDate = DateTime.Now,
                    IsRead = false
                });
            }
        }

        // 2. Check for upcoming tasks due within 3 days
        var pendingTasks = await _context.Tasks
            .Include(t => t.Subject)
            .Where(t => t.Subject!.UserId == userId && 
                        t.Status != "Completed" && 
                        t.Deadline >= today && 
                        t.Deadline <= today.AddDays(3))
            .ToListAsync();

        foreach (var task in pendingTasks)
        {
            var daysLeft = (task.Deadline.Date - today).Days;
            var reminderTitle = $"Task Due: {task.Title}";

            bool exists = await _context.Notifications
                .AnyAsync(n => n.UserId == userId && 
                               n.Title == reminderTitle && 
                               n.NotificationDate.Date == today);

            if (!exists)
            {
                string msg = daysLeft == 0
                    ? $"Task '{task.Title}' ({task.Subject?.SubjectName}) is due TODAY! Current progress: {task.ProgressPercentage}%."
                    : $"Task '{task.Title}' is due in {daysLeft} day{(daysLeft > 1 ? "s" : "")}. Don't forget to complete it!";

                _context.Notifications.Add(new Notification
                {
                    UserId = userId,
                    Title = reminderTitle,
                    Message = msg,
                    NotificationType = "DeadlineReminder",
                    NotificationDate = DateTime.Now,
                    IsRead = false
                });
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task<int> GetUnreadCountAsync(int userId)
    {
        return await _context.Notifications
            .CountAsync(n => n.UserId == userId && !n.IsRead);
    }

    public async Task MarkAllAsReadAsync(int userId)
    {
        var unread = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        foreach (var notif in unread)
        {
            notif.IsRead = true;
        }

        await _context.SaveChangesAsync();
    }
}

