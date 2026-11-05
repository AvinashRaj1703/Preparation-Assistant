using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartStudyPlanner.Models;

[Table("DailyGoals")]
public class DailyGoal
{
    [Key]
    public int GoalId { get; set; }

    [Required]
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public User? User { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Goal Date")]
    public DateTime GoalDate { get; set; } = DateTime.Today;

    [Required]
    [Range(0, 1440)]
    [Display(Name = "Target Study Time (Minutes)")]
    public int TargetStudyMinutes { get; set; } = 120;

    [Range(0, 1440)]
    [Display(Name = "Completed Study Time (Minutes)")]
    public int CompletedStudyMinutes { get; set; } = 0;

    [Required]
    [Range(0, 50)]
    [Display(Name = "Target Tasks")]
    public int TargetTasks { get; set; } = 3;

    [Range(0, 50)]
    [Display(Name = "Completed Tasks")]
    public int CompletedTasks { get; set; } = 0;

    [Required]
    [StringLength(30)]
    public string Status { get; set; } = "Pending"; // Pending, In Progress, Achieved, Missed

    [StringLength(1000)]
    public string? Notes { get; set; }

    [NotMapped]
    public double StudyProgressPercent => TargetStudyMinutes > 0 
        ? Math.Min(100.0, Math.Round((double)CompletedStudyMinutes / TargetStudyMinutes * 100, 1)) 
        : 0;

    [NotMapped]
    public double TaskProgressPercent => TargetTasks > 0 
        ? Math.Min(100.0, Math.Round((double)CompletedTasks / TargetTasks * 100, 1)) 
        : 0;
}

