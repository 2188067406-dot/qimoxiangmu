namespace TeachingAI1.ViewModels;

public class CourseModule
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Locked";
    public int LessonCount { get; set; }
    public int CompletedLessons { get; set; }
    public double Rating { get; set; } 
    public string VideoUrl { get; set; } = string.Empty;
}