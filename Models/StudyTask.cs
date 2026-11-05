using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartStudyPlanner.Models;

[Table("Tasks")]
public class StudyTask
{
    [Key]
    public int TaskId { get; set; }

    [Required]
    [Display(Name = "Subject")]
    public int SubjectId { get; set; }

    [ForeignKey("SubjectId")]
    public Subject? Subject { get; set; }

    [Required]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    [StringLength(50)]
    [Display(Name = "Task Type")]
    public string TaskType { get; set; } = "Assignment"; // Assignment, Reading, Project, Practice, Homework

    [Required]
    [DataType(DataType.Date)]
    public DateTime Deadline { get; set; }

    [Required]
    [StringLength(20)]
    public string Priority { get; set; } = "Medium"; // Low, Medium, High, Urgent

    [Required]
    [StringLength(20)]
    public string Difficulty { get; set; } = "Medium"; // Easy, Medium, Hard

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "Pending"; // Pending, In Progress, Completed, Overdue

    [Range(0, 100)]
    [Display(Name = "Progress (%)")]
    public int ProgressPercentage { get; set; } = 0;

    [Column(TypeName = "decimal(6,2)")]
    [Display(Name = "Estimated Hours")]
    public decimal EstimatedHours { get; set; } = 1.0m;

    [Column(TypeName = "decimal(6,2)")]
    [Display(Name = "Actual Hours")]
    public decimal ActualHours { get; set; } = 0.0m;

    [Display(Name = "Completed At")]
    public DateTime? CompletedAt { get; set; }

    [Display(Name = "Created At")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Navigation properties
    public ICollection<StudySession> StudySessions { get; set; } = new List<StudySession>();
}

