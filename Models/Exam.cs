using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartStudyPlanner.Models;

[Table("Exams")]
public class Exam
{
    [Key]
    public int ExamId { get; set; }

    [Required]
    [Display(Name = "Subject")]
    public int SubjectId { get; set; }

    [ForeignKey("SubjectId")]
    public Subject? Subject { get; set; }

    [Required]
    [StringLength(150)]
    [Display(Name = "Exam Name")]
    public string ExamName { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Exam Date")]
    public DateTime ExamDate { get; set; }

    [Required]
    [StringLength(50)]
    [Display(Name = "Exam Time")]
    public string ExamTime { get; set; } = "10:00 AM";

    [Required]
    [Display(Name = "Total Marks")]
    [Range(1, 1000)]
    public int TotalMarks { get; set; } = 100;

    [Required]
    [Display(Name = "Passing Marks")]
    [Range(0, 1000)]
    public int PassingMarks { get; set; } = 40;

    [StringLength(2000)]
    public string? Syllabus { get; set; }

    // Navigation properties
    public ICollection<RevisionPlan> RevisionPlans { get; set; } = new List<RevisionPlan>();
}

