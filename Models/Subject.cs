using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartStudyPlanner.Models;

[Table("Subjects")]
public class Subject
{
    [Key]
    public int SubjectId { get; set; }

    [Required]
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public User? User { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "Subject Name")]
    public string SubjectName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<StudyTask> Tasks { get; set; } = new List<StudyTask>();
    public ICollection<Exam> Exams { get; set; } = new List<Exam>();
    public ICollection<StudySession> StudySessions { get; set; } = new List<StudySession>();
    public ICollection<RevisionPlan> RevisionPlans { get; set; } = new List<RevisionPlan>();
}

