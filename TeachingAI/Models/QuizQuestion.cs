namespace TeachingAI1.Models
{
    public class QuizQuestion
    {
        public string QuestionText { get; set; } = string.Empty;
        public List<QuizOption> Options { get; set; } = new();        
        public int AnswerIndex { get; set; } // 0-based
        public string Explanation { get; set; } = string.Empty;
        public string CorrectAnswer { get; set; }
    }
}
