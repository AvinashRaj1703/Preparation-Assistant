namespace SmartStudyPlanner.Models.ViewModels;

public class DashboardViewModel
{
    // Summary KPI Counts
    public int TotalSubjects { get; set; }
    public int ActiveTasksCount { get; set; }
    public int CompletedTasksCount { get; set; }
    public int UpcomingExamsCount { get; set; }
    public int TotalStudyMinutes { get; set; }
    public int UnreadNotificationsCount { get; set; }

    // Today's Goal
    public DailyGoal? TodayGoal { get; set; }

    // Next Exam for live countdown
    public Exam? NextExam { get; set; }
    public int? DaysUntilNextExam { get; set; }

    // Lists for Quick Action & Overview
    public List<StudyTask> UpcomingTasks { get; set; } = new();
    public List<Exam> UpcomingExams { get; set; } = new();
    public List<StudySession> RecentSessions { get; set; } = new();
    public List<RevisionPlan> UpcomingRevisions { get; set; } = new();
    public List<Notification> RecentNotifications { get; set; } = new();

    // Chart Data
    public List<string> WeeklyLabels { get; set; } = new();
    public List<int> WeeklyStudyMinutes { get; set; } = new();

    public List<string> SubjectChartLabels { get; set; } = new();
    public List<int> SubjectChartMinutes { get; set; } = new();

    public int TasksPendingCount { get; set; }
    public int TasksInProgressCount { get; set; }
}

