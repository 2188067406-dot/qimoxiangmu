using TeachingAI1.Models;
namespace TeachingAI1.Services
{
    public interface IAiQuizService
    {
        Task<string> GetCompletionAsync(string prompt);
        Task<List<QuizQuestion>> GenerateQuizQuestionsAsync(string context, int count = 3);
    }
}