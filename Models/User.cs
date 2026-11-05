using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartStudyPlanner.Models;

[Table("Users")]
public class User
{
    [Key]
    public int UserId { get; set; }

    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
    public ICollection<StudySession> StudySessions { get; set; } = new List<StudySession>();
    public ICollection<RevisionPlan> RevisionPlans { get; set; } = new List<RevisionPlan>();
    public ICollection<DailyGoal> DailyGoals { get; set; } = new List<DailyGoal>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}

