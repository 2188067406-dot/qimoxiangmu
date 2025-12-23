using TeachingAI1.Models;

public class Lesson
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; } // Could include text, videos, or links
    public bool IsCompleted { get; set; } = false;
    public int ModuleId { get; set; }
    public Module Module { get; set; } = null!;

    public ICollection<Quiz> Quizzes { get; set; } // QuizOptional quizzes for the lesson
}
