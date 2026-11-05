using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartStudyPlanner.Models;

[Table("StudySessions")]
public class StudySession
{
    [Key]
    public int StudySessionId { get; set; }

    [Required]
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public User? User { get; set; }

    [Required]
    [Display(Name = "Subject")]
    public int SubjectId { get; set; }

    [ForeignKey("SubjectId")]
    public Subject? Subject { get; set; }

    [Display(Name = "Related Task")]
    public int? TaskId { get; set; }

    [ForeignKey("TaskId")]
    public StudyTask? Task { get; set; }

    [Required]
    [StringLength(200)]
    public string Topic { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Study Date")]
    public DateTime StudyDate { get; set; } = DateTime.Today;

    [Required]
    [StringLength(20)]
    [Display(Name = "Start Time")]
    public string StartTime { get; set; } = "14:00";

    [StringLength(20)]
    [Display(Name = "End Time")]
    public string? EndTime { get; set; }

    [Required]
    [Range(1, 1440)]
    [Display(Name = "Duration (Minutes)")]
    public int DurationMinutes { get; set; } = 60;

    [Required]
    [StringLength(30)]
    public string Status { get; set; } = "Completed"; // Planned, In Progress, Completed, Cancelled

    [StringLength(1000)]
    public string? Notes { get; set; }

    [Display(Name = "Created At")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

