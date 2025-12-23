// Services/CourseService.cs
using TeachingAI1.Models;

namespace TeachingAI1.Services
{
    public class CourseService : ICourseService
    {
        public async Task<string> GetCourseContentAsync(int courseId)
        {
            // 示例：返回模拟内容（后续可连接数据库）
            return $"课程 {courseId} 的教学内容";
        }
        public Task<Course> GetCourseByIdAsync(int id)
        {
            // 临时 mock 数据（后续可连数据库）
            var course = new Course
            {
                Id = id,
                Title = "人工智能基础",
                Instructor = "A老师",
                Description = "学习 AI 核心概念与应用。",
                Modules = new List<Module>
                {
                    new Module { Title = "第一章", Description = "监督与无监督学习" },
                    new Module { Title = "第二章", Description = "深度学习入门" },
                    new Module { Title = "第三章", Description = "通义千问原理" }
                }
            };

            foreach (var module in course.Modules)
            {
                module.CourseId = course.Id;
                module.Course = course; // 如果视图需要 module.Course.Title
            }
            
            return Task.FromResult(course);
        }
    }
}