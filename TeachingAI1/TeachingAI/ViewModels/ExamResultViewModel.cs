using TeachingAI1.Models;

namespace TeachingAI1.ViewModels
{
    public class ExamResultViewModel
    {
        public List<QuizQuestion> QuizQuestions { get; set; } = new();
        public string[]? UserAnswers { get; set; }
        public string AiExplanation { get; set; } = string.Empty;
    }

}