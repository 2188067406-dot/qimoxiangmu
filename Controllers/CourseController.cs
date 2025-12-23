using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeachingAI1.Data;
using TeachingAI1.Models;
using TeachingAI1.ViewModels;
using System.Text.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.Http;

namespace TeachingAI1.Controllers;

public class CourseController : Controller
{
    private readonly ApplicationDbContext _context;

    public CourseController(ApplicationDbContext context)
    {
        _context = context;
    }
    // ✅ 课程列表页
    public async Task<IActionResult> Index()
    {
        
        System.Diagnostics.Debug.WriteLine("CourseController.Index() called");

        var courses = await _context.Courses
            .Include(c => c.Teacher)
            .ToListAsync();

        var viewModelList = courses.Select(c => new CourseViewModel
        {
            Id = c.Id,
            Title = c.Name ?? "Untitled Course",
            Description = c.Description ?? "No description available.",
            Instructor = c.Teacher?.Name ?? "Unknown",
            Duration = c.Duration ?? "12 Weeks",
            Status = c.Status ?? "Active"
        }).ToList();

        return View(viewModelList);
    }
    public IActionResult CourseDetails(int id)
    {
        var course = GetMockCourse(id);

        var viewModel = new CourseDetailsViewModel
        {
            Title = course.Title,
            Description = course.Description,
            LessonsTotal = course.LessonsTotal,
            LessonsCompleted = course.LessonsCompleted,
            Status = course.Status,

            Modules = course.Modules.Select(m => new CourseModuleViewModel
            {
                Title = m.Title,
                Description = m.Description,
                LessonCount = m.LessonCount,
                CompletedLessons = m.CompletedLessons,
                Status = m.Status,

                Lessons = m.Lessons 
            }).ToList()
            
        };
        ViewData["CourseId"] = id;
        return View(viewModel);
    }
    private Course GetMockCourse(int id)
    {
        return id switch
        {
            1 => new Course
            {
                Id = 1,
                Title = "AI基础课程",
                Description = "本AI基础课程面向零基础学习者，系统讲解AI核心概念、机器学习等基础技术及典型应用，帮助快速建立AI认知框架，轻松入门人工智能领域。",
                LessonsTotal = 12,
                LessonsCompleted = 9,
                Status = "In Progress",
                Modules = new List<Module>
                {
                    new Module
                    {
                        Title = "第一章",
                        Description = "基础介绍",
                        LessonCount = 4,
                        CompletedLessons = 4,
                        Status = "Completed",
                        Lessons = new List<Lesson>
                        {
                            new Lesson { Title = "Example Lesson 1", IsCompleted = true },
                            new Lesson { Title = "Example Lesson 2", IsCompleted = true },
                            new Lesson { Title = "Example Lesson 3", IsCompleted = true },
                            new Lesson { Title = "Example Lesson 4", IsCompleted = true }
                        }
                    },
                    new Module
                    {
                        Title = "第二章",
                        Description = "初步理解",
                        LessonCount = 5,
                        CompletedLessons = 4,
                        Status = "In Progress",
                        Lessons = new List<Lesson>
                        {
                            new Lesson { Title = "Example Lesson 5", IsCompleted = true },
                            new Lesson { Title = "Example Lesson 6", IsCompleted = true },
                            new Lesson { Title = "Example Lesson 7", IsCompleted = true },
                            new Lesson { Title = "Example Lesson 8", IsCompleted = true },
                            new Lesson { Title = "Example Lesson 9", IsCompleted = false }
                        }
                    },
                    new Module
                    {
                        Title = "第三章",
                        Description = "深入了解",
                        LessonCount = 4,
                        CompletedLessons = 0,
                        Status = "Not Started",
                        Lessons = new List<Lesson>
                        {
                            new Lesson { Title = "Example Lesson 10", IsCompleted = false },
                            new Lesson { Title = "Example Lesson 11", IsCompleted = false },
                            new Lesson { Title = "Example Lesson 12", IsCompleted = false },
                            new Lesson { Title = "Example Lesson 13", IsCompleted = false }
                        }
                    },
                    new Module
                    {
                        Title = "第四章",
                        Description = "总结回顾",
                        LessonCount = 4,
                        CompletedLessons = 0,
                        Status = "Not Started",
                        Lessons = new List<Lesson>
                        {
                            new Lesson { Title = "Example Lesson 14", IsCompleted = false },
                            new Lesson { Title = "Example Lesson 15", IsCompleted = false },
                            new Lesson { Title = "Example Lesson 16", IsCompleted = false },
                            new Lesson { Title = "Example Lesson 17", IsCompleted = false }
                        }
                    }
                }
            },
            2 => new Course
            {
                Id = 2,
                Title = "思维力训练：用框架解决问题",
                Description = "通过系统训练掌握框架思维方法，帮助你从混乱中理出头绪，高效分析解决复杂问题，建立结构化思考习惯。",
                LessonsTotal = 16,
                LessonsCompleted = 7,
                Status = "In Progress",
                Modules = new List<Module>
                {
                    new Module
                    {
                        Title = "第一章",
                        Description = "基础介绍",
                        LessonCount = 4,
                        CompletedLessons = 4,
                        Status = "Completed",
                        Lessons = new List<Lesson>
                        {
                            new Lesson { Title = "Example Lesson 1", IsCompleted = true },
                            new Lesson { Title = "Example Lesson 2", IsCompleted = true },
                            new Lesson { Title = "Example Lesson 3", IsCompleted = true },
                            new Lesson { Title = "Example Lesson 4", IsCompleted = true }
                        }
                    },
                    new Module
                    {
                        Title = "第二章",
                        Description = "初步理解",
                        LessonCount = 5,
                        CompletedLessons = 4,
                        Status = "In Progress",
                        Lessons = new List<Lesson>
                        {
                            new Lesson { Title = "Example Lesson 5", IsCompleted = true },
                            new Lesson { Title = "Example Lesson 6", IsCompleted = true },
                            new Lesson { Title = "Example Lesson 7", IsCompleted = true },
                            new Lesson { Title = "Example Lesson 8", IsCompleted = true },
                            new Lesson { Title = "Example Lesson 9", IsCompleted = false }
                        }
                    },
                    new Module
                    {
                        Title = "第三章",
                        Description = "深入了解",
                        LessonCount = 4,
                        CompletedLessons = 0,
                        Status = "Not Started",
                        Lessons = new List<Lesson>
                        {
                            new Lesson { Title = "Example Lesson 10", IsCompleted = false },
                            new Lesson { Title = "Example Lesson 11", IsCompleted = false },
                            new Lesson { Title = "Example Lesson 12", IsCompleted = false },
                            new Lesson { Title = "Example Lesson 13", IsCompleted = false }
                        }
                    },
                    new Module
                    {
                        Title = "第四章",
                        Description = "总结回顾",
                        LessonCount = 4,
                        CompletedLessons = 0,
                        Status = "Not Started",
                        Lessons = new List<Lesson>
                        {
                            new Lesson { Title = "Example Lesson 14", IsCompleted = false },
                            new Lesson { Title = "Example Lesson 15", IsCompleted = false },
                            new Lesson { Title = "Example Lesson 16", IsCompleted = false },
                            new Lesson { Title = "Example Lesson 17", IsCompleted = false }
                        }
                    }
                }
            },
            3 => new Course
            {
                Id = 3, 
                Title = "一分钟建模-饼干", 
                Description = "通过简洁高效的步骤教学，让你在短时间内掌握使用软件创建饼干模型的核心技巧，适合零基础学生入门。",
                LessonsTotal = 10,
                LessonsCompleted = 6,
                Status = "In Progress",
                Modules = new List<Module>
                {
                    new Module
                    {
                        Title = "第一章",
                        Description = "基础介绍",
                        LessonCount = 4,
                        CompletedLessons = 4,
                        Status = "Completed",
                        Lessons = new List<Lesson>
                        {
                            new Lesson { Title = "Example Lesson 1", IsCompleted = true },
                            new Lesson { Title = "Example Lesson 2", IsCompleted = true },
                            new Lesson { Title = "Example Lesson 3", IsCompleted = true },
                            new Lesson { Title = "Example Lesson 4", IsCompleted = true }
                        }
                    },
                    new Module
                    {
                        Title = "第二章",
                        Description = "初步理解",
                        LessonCount = 5,
                        CompletedLessons = 4,
                        Status = "In Progress",
                        Lessons = new List<Lesson>
                        {
                            new Lesson { Title = "Example Lesson 5", IsCompleted = true },
                            new Lesson { Title = "Example Lesson 6", IsCompleted = true },
                            new Lesson { Title = "Example Lesson 7", IsCompleted = true },
                            new Lesson { Title = "Example Lesson 8", IsCompleted = true },
                            new Lesson { Title = "Example Lesson 9", IsCompleted = false }
                        }
                    },
                    new Module
                    {
                        Title = "第三章",
                        Description = "深入了解",
                        LessonCount = 4,
                        CompletedLessons = 0,
                        Status = "Not Started",
                        Lessons = new List<Lesson>
                        {
                            new Lesson { Title = "Example Lesson 10", IsCompleted = false },
                            new Lesson { Title = "Example Lesson 11", IsCompleted = false },
                            new Lesson { Title = "Example Lesson 12", IsCompleted = false },
                            new Lesson { Title = "Example Lesson 13", IsCompleted = false }
                        }
                    },
                    new Module
                    {
                        Title = "第四章",
                        Description = "总结回顾",
                        LessonCount = 4,
                        CompletedLessons = 0,
                        Status = "Not Started",
                        Lessons = new List<Lesson>
                        {
                            new Lesson { Title = "Example Lesson 14", IsCompleted = false },
                            new Lesson { Title = "Example Lesson 15", IsCompleted = false },
                            new Lesson { Title = "Example Lesson 16", IsCompleted = false },
                            new Lesson { Title = "Example Lesson 17", IsCompleted = false }
                        }
                    }
                }
            },
            4 => new Course
            {
                Id = 4, 
                Title = "耶鲁大学课程-心理学课程介绍", 
                Description = "全面探索人类思维与行为的科学原理，帮助学生理解自我与他人，培养批判性思考能力，是理解人类心智的入门指南。",
                LessonsTotal = 20,
                LessonsCompleted = 20,
                Status = "Completed",
                Modules = new List<Module>
                {
                    new Module
                    {
                        Title = "第一章",
                        Description = "基础介绍",
                        LessonCount = 4,
                        CompletedLessons = 4,
                        Status = "Completed",
                        Lessons = new List<Lesson>
                        {
                            new Lesson { Title = "Example Lesson 1", IsCompleted = true },
                            new Lesson { Title = "Example Lesson 2", IsCompleted = true },
                            new Lesson { Title = "Example Lesson 3", IsCompleted = true },
                            new Lesson { Title = "Example Lesson 4", IsCompleted = true }
                        }
                    },
                    new Module
                    {
                        Title = "第二章",
                        Description = "初步理解",
                        LessonCount = 5,
                        CompletedLessons = 4,
                        Status = "In Progress",
                        Lessons = new List<Lesson>
                        {
                            new Lesson { Title = "Example Lesson 5", IsCompleted = true },
                            new Lesson { Title = "Example Lesson 6", IsCompleted = true },
                            new Lesson { Title = "Example Lesson 7", IsCompleted = true },
                            new Lesson { Title = "Example Lesson 8", IsCompleted = true },
                            new Lesson { Title = "Example Lesson 9", IsCompleted = false }
                        }
                    },
                    new Module
                    {
                        Title = "第三章",
                        Description = "深入了解",
                        LessonCount = 4,
                        CompletedLessons = 0,
                        Status = "Not Started",
                        Lessons = new List<Lesson>
                        {
                            new Lesson { Title = "Example Lesson 10", IsCompleted = false },
                            new Lesson { Title = "Example Lesson 11", IsCompleted = false },
                            new Lesson { Title = "Example Lesson 12", IsCompleted = false },
                            new Lesson { Title = "Example Lesson 13", IsCompleted = false }
                        }
                    },
                    new Module
                    {
                        Title = "第四章",
                        Description = "总结回顾",
                        LessonCount = 4,
                        CompletedLessons = 0,
                        Status = "Not Started",
                        Lessons = new List<Lesson>
                        {
                            new Lesson { Title = "Example Lesson 14", IsCompleted = false },
                            new Lesson { Title = "Example Lesson 15", IsCompleted = false },
                            new Lesson { Title = "Example Lesson 16", IsCompleted = false },
                            new Lesson { Title = "Example Lesson 17", IsCompleted = false }
                        }
                    }
                }
            },
            5 => new Course
            {
                Id = 5, 
                Title = "小猪佩奇-吹口哨", 
                Description = "讲述了小猪佩奇看到家人都会吹口哨而自己不会感到沮丧，经过不断尝试和练习，最终在吹饼干时成功吹响口哨，收获自信与快乐的故事。",
                LessonsTotal = 15,
                LessonsCompleted = 15,
                Status = "Completed",
                Modules = new List<Module>
                {
                    new Module
                    {
                        Title = "第一章",
                        Description = "基础介绍",
                        LessonCount = 4,
                        CompletedLessons = 4,
                        Status = "Completed",
                        Lessons = new List<Lesson>
                        {
                            new Lesson { Title = "Example Lesson 1", IsCompleted = true },
                            new Lesson { Title = "Example Lesson 2", IsCompleted = true },
                            new Lesson { Title = "Example Lesson 3", IsCompleted = true },
                            new Lesson { Title = "Example Lesson 4", IsCompleted = true }
                        }
                    },
                    new Module
                    {
                        Title = "第二章",
                        Description = "初步理解",
                        LessonCount = 5,
                        CompletedLessons = 4,
                        Status = "In Progress",
                        Lessons = new List<Lesson>
                        {
                            new Lesson { Title = "Example Lesson 5", IsCompleted = true },
                            new Lesson { Title = "Example Lesson 6", IsCompleted = true },
                            new Lesson { Title = "Example Lesson 7", IsCompleted = true },
                            new Lesson { Title = "Example Lesson 8", IsCompleted = true },
                            new Lesson { Title = "Example Lesson 9", IsCompleted = false }
                        }
                    },
                    new Module
                    {
                        Title = "第三章",
                        Description = "深入了解",
                        LessonCount = 4,
                        CompletedLessons = 0,
                        Status = "Not Started",
                        Lessons = new List<Lesson>
                        {
                            new Lesson { Title = "Example Lesson 10", IsCompleted = false },
                            new Lesson { Title = "Example Lesson 11", IsCompleted = false },
                            new Lesson { Title = "Example Lesson 12", IsCompleted = false },
                            new Lesson { Title = "Example Lesson 13", IsCompleted = false }
                        }
                    },
                    new Module
                    {
                        Title = "第四章",
                        Description = "总结回顾",
                        LessonCount = 4,
                        CompletedLessons = 0,
                        Status = "Not Started",
                        Lessons = new List<Lesson>
                        {
                            new Lesson { Title = "Example Lesson 14", IsCompleted = false },
                            new Lesson { Title = "Example Lesson 15", IsCompleted = false },
                            new Lesson { Title = "Example Lesson 16", IsCompleted = false },
                            new Lesson { Title = "Example Lesson 17", IsCompleted = false }
                        }
                    }
                }
            }
        };
    }

    private async Task<string> CallQwenAPI(string prompt)
    {
        var apiKey = "sk-069abb4d6d41498bab30fd7e2e8bc61a";
        var url = "https://dashscope.aliyuncs.com/api/v1/services/aigc/text-generation/generation";

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

        // 构造请求体
        var requestBody = new
        {
            model = "qwen-max",
            input = new
            {
                messages = new[]
                {
                    new { role = "user", content = prompt }
                }
            },
            parameters = new
            {
                temperature = 0.3,
                top_p = 0.8
            }
        };

        string json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await client.PostAsync(url, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            // 🔍 调试：打印原始响应（可选）
            // System.Diagnostics.Debug.WriteLine("DashScope 响应: " + responseBody);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"DashScope API 错误 ({response.StatusCode}): {responseBody}");
            }

            using var doc = JsonDocument.Parse(responseBody);
            var root = doc.RootElement;

            if (root.TryGetProperty("output", out var output) &&
                output.TryGetProperty("text", out var textElement))
            {
                return textElement.GetString()?.Trim() ?? "";
            }
            else
            {
                throw new Exception("未找到 output.text。响应：" + responseBody);
            }
        }
        catch (Exception ex)
        {
            // 👇 关键：记录异常信息，方便排查 500
            Console.WriteLine($"CallQwenAPI 异常: {ex.Message}");
            throw; // 让上层捕获并返回友好错误
        }
    }

    [HttpGet]
    public async Task<IActionResult> GenerateQuizQuestions(int courseId)
    {
        try
        {
            // 构造 prompt
            var questionCount = 3; 
            var course = GetMockCourse(courseId);
            var prompt = $@"
            你是一个专业的教学 AI 助手，请严格按以下要求生成题目：

            1. 为课程《{course.Title}》生成 {questionCount} 道单选题。
            2. 每道题必须包含：
                - 问题（QuestionText）
                - 4 个选项（Options），每个选项有：
                    - Text（选项内容）
                    - Value（选项标识，依次为 ""A""、""B""、""C""、""D""）
                - 正确答案（CorrectAnswer），值为 ""A""、""B""、""C"" 或 ""D""
                - 解析（Explanation），一段简短说明
            3. 所有题目必须以 **标准 JSON 数组格式** 输出。
            4. **不要包含任何其他文字、说明、序号、Markdown、反引号或前缀**。
            5. 确保 JSON 能被 C# 的 System.Text.Json 成功解析。
            6. 不要使用中文冒号、引号等非标准符号。

            示例输出（仅作格式参考，不要照抄）：
            [{{""QuestionText"":""什么是 C#？"",""Options"":[{{""Text"":""一种编程语言"",""Value"":""A""}},{{""Text"":""一个操作系统"",""Value"":""B""}},{{""Text"":""一款游戏"",""Value"":""C""}},{{""Text"":""一种水果"",""Value"":""D""}}],""CorrectAnswer"":""A"",""Explanation"":""C# 是由微软开发的一种现代编程语言。""}}]

            现在请开始生成 {questionCount} 道新题目：
            ";
            var aiResponse = await CallQwenAPI(prompt);
            // 清理可能的 Markdown
            aiResponse = Regex.Replace(aiResponse, @"^```(?:json)?\s*", "", RegexOptions.IgnoreCase);
            aiResponse = Regex.Replace(aiResponse, @"\s*```$", "", RegexOptions.IgnoreCase);
            aiResponse = aiResponse.Trim();

            if (string.IsNullOrEmpty(aiResponse))
                return Json(new { success = false, message = "AI 未返回任何内容。" });


            // 尝试解析 AI 返回的 JSON
            List<QuizQuestion> questions;
            try
            {
                questions = JsonSerializer.Deserialize<List<QuizQuestion>>(aiResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (JsonException)
            {
                return Json(new { success = false, message = "AI 返回格式无效，请重试。" });
            }

            if (questions == null || !questions.Any())
            {
                return Json(new { success = false, message = "AI 未生成有效题目。" });
            }

            return Json(new { success = true, questions });
        }
        catch (Exception ex)
        {
            // 记录日志（可选）
            // _logger.LogError(ex, "AI 出题失败");
            return Json(new { success = false, message = "AI 服务暂时不可用，请稍后重试。" });
        }
    }
    public class AnswerModel
    {
        public int QuestionId { get; set; }
        public string SelectedOption { get; set; }
    }
}
