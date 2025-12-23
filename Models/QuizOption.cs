// Models/QuizOption.cs
namespace TeachingAI1.Models
{
    public class QuizOption
    {
        public string Text { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty; // "A", "B", "C"...
        // 未来可扩展：public bool IsCorrect { get; set; } 等
    }
}