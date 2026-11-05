namespace SmartStudyPlanner.Models.ViewModels;

public class CalendarEventViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Start { get; set; } = string.Empty;
    public string? End { get; set; }
    public string BackgroundColor { get; set; } = "#3b82f6";
    public string BorderColor { get; set; } = "#2563eb";
    public string TextColor { get; set; } = "#ffffff";
    public string Url { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "Exam", "Task", "StudySession", "Revision"
    public string? Description { get; set; }
}

