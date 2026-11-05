using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartStudyPlanner.Models;

[Table("RevisionPlans")]
public class RevisionPlan
{
    [Key]
    public int RevisionPlanId { get; set; }

    [Required]
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public User? User { get; set; }

    [Required]
    [Display(Name = "Subject")]
    public int SubjectId { get; set; }

    [ForeignKey("SubjectId")]
    public Subject? Subject { get; set; }

    [Display(Name = "Target Exam")]
    public int? ExamId { get; set; }

    [ForeignKey("ExamId")]
    public Exam? Exam { get; set; }

    [Required]
    [StringLength(200)]
    public string Topic { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Revision Date")]
    public DateTime RevisionDate { get; set; } = DateTime.Today;

    [StringLength(20)]
    [Display(Name = "Start Time")]
    public string? StartTime { get; set; } = "10:00";

    [Required]
    [Range(1, 1440)]
    [Display(Name = "Duration (Minutes)")]
    public int DurationMinutes { get; set; } = 45;

    [Required]
    [StringLength(30)]
    public string Status { get; set; } = "Scheduled"; // Scheduled, Completed, Skipped

    [StringLength(1000)]
    public string? Notes { get; set; }

    [Display(Name = "Created At")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

