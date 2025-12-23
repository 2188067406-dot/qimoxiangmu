using Microsoft.AspNetCore.Mvc;
using System.Text;
using TeachingAI1.Services;   
using TeachingAI1.Models;
    
namespace TeachingAI1.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuizController : ControllerBase
    {
        private readonly ICourseService _courseService; // 假设你有这个服务
        private readonly IAiQuizService _aiQuizService;

        public QuizController(ICourseService courseService, IAiQuizService aiQuizService)
        {
            _courseService = courseService;
            _aiQuizService = aiQuizService;
        }

        [HttpGet("generate")]
        public async Task<IActionResult> GenerateQuiz(int courseId, int count = 3)
        {
            var course = await _courseService.GetCourseByIdAsync(courseId);
            if (course == null) return NotFound(new { success = false, message = "Course not found." });

            // 构建上下文：课程标题 + 描述 + 所有模块标题和描述
            var context = new StringBuilder();
            context.AppendLine($"课程名称：{course.Title}");
            context.AppendLine($"课程描述：{course.Description}");
            context.AppendLine("包含模块：");
            foreach (var module in course.Modules)
            {
                context.AppendLine($"- {module.Title}: {module.Description}");
            }

            try
            {
                var QuizQuizQuestions = await _aiQuizService.GenerateQuizQuestionsAsync(context.ToString(), count);
                return Ok(new { success = true, QuizQuizQuestions });
            }
            catch (Exception ex)
            {
                // 记录日志（可用 ILogger）
                return StatusCode(500, new { success = false, message = "出题失败，请稍后重试。" });
            }
        }
    }
}