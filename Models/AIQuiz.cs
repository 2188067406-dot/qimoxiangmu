public class Quiz
{
    public int Id { get; set; }
    public string QuizQuestion { get; set; }
    public string Answer { get; set; } // Correct answer
    public ICollection<string> QuizOptions { get; set; } // Multiple-choice QuizOptions
    public int LessonId { get; set; }
    public Lesson Lesson { get; set; }
}
