// Services/ICourseService.cs
using TeachingAI1.Models;
namespace TeachingAI1.Services
{
    public interface ICourseService
    {
        Task<string> GetCourseContentAsync(int courseId);
        Task<Course> GetCourseByIdAsync(int id);
    }
}