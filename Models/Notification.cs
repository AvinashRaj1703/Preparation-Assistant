using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartStudyPlanner.Models;

[Table("Notifications")]
public class Notification
{
    [Key]
    public int NotificationId { get; set; }

    [Required]
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public User? User { get; set; }

    [Required]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string Message { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    [Display(Name = "Notification Type")]
    public string NotificationType { get; set; } = "General"; // ExamCountdown, DeadlineReminder, DailyGoal, System

    [Display(Name = "Notification Date")]
    public DateTime NotificationDate { get; set; } = DateTime.Now;

    public bool IsRead { get; set; } = false;
}

