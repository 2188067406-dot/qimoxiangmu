using Microsoft.AspNetCore.Mvc;
using TeachingAI1.Services;
using TeachingAI1.Models;
using TeachingAI1.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using System.Text;
using System.Net.Http.Headers;

namespace TeachingAI1.Controllers
{
   
    public class StudentController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> SubmitQuiz(int courseId, int[] answers)
        {
            // 重新加载该课程的所有题目
            var allQuizQuestions = LoadQuizQuestionsForCourse(courseId);

            var random = new Random();
            //List<QuizQuestion> allQuizQuestionsList = _aiQuizService.GenerateQuizQuestionsAsync(courseId);
            var selectedQuizQuestions = allQuizQuestions.OrderBy(x => random.Next()).Take(3).ToList();
            int score = 0;
            int total = Math.Min(answers.Length, selectedQuizQuestions.Count);

            for (int i = 0; i < total; i++)
            {
                var QuizQuestion = selectedQuizQuestions[i];
                string userAnswer = answers[i].ToString();

                string correctLetter = QuizQuestion.Options[QuizQuestion.AnswerIndex].Value;
                
                if (userAnswer == correctLetter)
                {
                    score++;
                }
            }

            ViewBag.ShowResult = true;
            ViewBag.Score = $"{score}/{total}";

            // 重新获取课程信息并返回原视图
            var course = GetCourseById(courseId);
            if (course == null)
                return NotFound();

            var viewModel = new CourseDetailsViewModel
            {
                Course = course,
                Questions = await GetQuizQuestionsForCourse(courseId) // 你的 quiz 数据
            };
            return View("CourseDetails", viewModel);
        }

        private List<QuizQuestion> LoadQuizQuestionsForCourse(int courseId)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", $"QuizQuestions_course{courseId}.json");
            
            if (!System.IO.File.Exists(filePath))
            {
                // 如果文件不存在，返回空列表或默认题目
                return new List<QuizQuestion>();
            }

            var json = System.IO.File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<QuizQuestion>>(json) ?? new List<QuizQuestion>();
        }
        // GET: Student
        public ActionResult Index()
        {
            return View();
        }
        
        public IActionResult Dashboard()
        {
            ViewData["ShowSearch"] = true;
            
            var allCourses = GetAllCourses();
            var inProgressCourses = allCourses
                .Where(c => c.Status == "In Progress")
                .ToList();

            return View(allCourses);
        }

        public IActionResult Courses(string filter = null)
        
        {    
            ViewData["Title"] = "My Courses";
            
            var courses = GetAllCourses();

            if (!string.IsNullOrEmpty(filter))
            {
                if (filter.Equals("completed", StringComparison.OrdinalIgnoreCase))
                {
                    courses = courses.Where(c => c.Status == "Completed").ToList();
                }
                else if (filter.Equals("inprogress", StringComparison.OrdinalIgnoreCase))
                {
                    courses = courses.Where(c => c.Status == "In Progress").ToList();
                }
            }

            return View(courses);
        }
        
        private Course GetCourseById(int id)
        {
            switch (id)
            {
                case 1:
                    return new Course
                    { 
                        Id = 1, 
                        Title = "AI基础课程", 
                        Instructor = "A老师",
                        Description = "本AI基础课程面向零基础学习者，系统讲解AI核心概念、机器学习等基础技术及典型应用，帮助快速建立AI认知框架，轻松入门人工智能领域。",
                        Progress = 75,
                        LessonsTotal = 12,
                        LessonsCompleted = 9,
                        TimeLeft = "6h left",
                        ImageUrl = "/images/course1.jpg",
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
                    };
                case 2:
                    return new Course
                    { 
                        Id = 2, 
                        Title = "思维力训练：用框架解决问题", 
                        Instructor = "B老师",
                        Description = "通过系统训练掌握框架思维方法，帮助你从混乱中理出头绪，高效分析解决复杂问题，建立结构化思考习惯。",
                        Progress = 45,
                        LessonsTotal = 16,
                        LessonsCompleted = 7,
                        TimeLeft = "12h left",
                        ImageUrl = "/images/course2.jpg",
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
                    };
                case 3:
                    return new Course
                    { 
                        Id = 3, 
                        Title = "一分钟建模-饼干", 
                        Instructor = "C老师",
                        Description = "通过简洁高效的步骤教学，让你在短时间内掌握使用软件创建饼干模型的核心技巧，适合零基础学生入门。",
                        Progress = 60,
                        LessonsTotal = 10,
                        LessonsCompleted = 6,
                        TimeLeft = "5h left",
                        ImageUrl = "/images/course3.jpg",
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
                    };
                case 4:
                    return new Course
                    { 
                        Id = 4, 
                        Title = "耶鲁大学课程-心理学课程介绍", 
                        Instructor = "D老师",
                        Description = "全面探索人类思维与行为的科学原理，帮助学生理解自我与他人，培养批判性思考能力，是理解人类心智的入门指南。",
                        Progress = 100,
                        LessonsTotal = 20,
                        LessonsCompleted = 20,
                        TimeLeft = "0h left",
                        ImageUrl = "/images/course4.jpg",
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
                    };
                case 5:
                    return new Course
                    { 
                        Id = 5, 
                        Title = "小猪佩奇-吹口哨", 
                        Instructor = "E老师",
                        Description = "讲述了小猪佩奇看到家人都会吹口哨而自己不会感到沮丧，经过不断尝试和练习，最终在吹饼干时成功吹响口哨，收获自信与快乐的故事。",
                        Progress = 100,
                        LessonsTotal = 15,
                        LessonsCompleted = 15,
                        TimeLeft = "0h left",
                        ImageUrl = "/images/course5.jpg",
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
                    };
                default:
                    return null;
            }
        }
        private List<CourseViewModel> GetAllCourses()
        {
            return new List<CourseViewModel>
            {
                new CourseViewModel 
                { 
                    Id = 1, Title = "AI基础课程", Instructor = "A老师",
                    Progress = 75, LessonsTotal = 12, LessonsCompleted = 9,
                    TimeLeft = "6h left", Status = "In Progress",ImageUrl = "/images/course1.jpg"
                },
                new CourseViewModel 
                { 
                    Id = 2, Title = "思维力训练：用框架解决问题", Instructor = "B老师",
                    Progress = 45, LessonsTotal = 16, LessonsCompleted = 7,
                    TimeLeft = "12h left", Status = "In Progress",ImageUrl = "/images/course2.jpg"
                },
                new CourseViewModel 
                { 
                    Id = 3, Title = "一分钟建模-饼干", Instructor = "C老师",
                    Progress = 60, LessonsTotal = 10, LessonsCompleted = 6,
                    TimeLeft = "5h left", Status = "In Progress",ImageUrl = "/images/course3.jpg"
                },
                new CourseViewModel
                { 
                    Id = 4, Title = "耶鲁大学课程-心理学课程介绍", Instructor = "D老师",
                    Progress = 100, LessonsTotal = 20, LessonsCompleted = 20,
                    TimeLeft = "0h left", Status = "Completed",ImageUrl = "/images/course4.jpg"
                },
                new CourseViewModel
                { 
                    Id = 5, Title = "小猪佩奇-吹口哨", Instructor = "E老师",
                    Progress = 100, LessonsTotal = 15, LessonsCompleted = 15,
                    TimeLeft = "0h left", Status = "Completed",ImageUrl = "/images/course5.jpg"
                }
            };
        }

        public async Task<IActionResult> CourseDetails(int id)
        {
            var course = GetCourseById(id);
            if (course == null)
            {
                return NotFound();
            }

            var viewModel = new CourseDetailsViewModel
            {
                Questions = GenerateQuizQuestions(id),
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                LessonsTotal = course.LessonsTotal,
                LessonsCompleted = course.LessonsCompleted,
                Status = course.Status,
                Modules = course.Modules.Select(m => new CourseModuleViewModel
                {
                    Title = m.Title,
                    Description = m.Description,
                    VideoUrl = m.VideoUrl,
                    LessonCount = m.LessonCount,
                    CompletedLessons = m.CompletedLessons,
                    Status = m.Status,
                    Lessons = m.Lessons?.Select(l => new Lesson
                    {
                        Title = l.Title,
                        IsCompleted = l.IsCompleted
                    }).ToList() ?? new List<Lesson>()
                }).ToList(),
                Course = course,
                
            };

            return View(viewModel);
        }
        private List<QuizQuestion> GenerateQuizQuestions(int courseId)
        {
            return new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    QuestionText = "C# 中用于定义类的关键字是？",
                    Options = new List<TeachingAI1.Models.QuizOption>
                    {
                        new TeachingAI1.Models.QuizOption { Text = "struct", Value = "A" },
                        new TeachingAI1.Models.QuizOption { Text = "class", Value = "B" },
                        new TeachingAI1.Models.QuizOption { Text = "interface", Value = "C" },
                        new TeachingAI1.Models.QuizOption { Text = "enum", Value = "D" }
                    },
                    AnswerIndex = 1,
                    CorrectAnswer = "B"
                },
                new QuizQuestion
                {
                    QuestionText = "ASP.NET Core 默认使用什么协议？",
                    Options = new List<TeachingAI1.Models.QuizOption>
                    {
                        new TeachingAI1.Models.QuizOption { Text = "HTTP", Value = "A" },
                        new TeachingAI1.Models.QuizOption { Text = "HTTPS", Value = "B" },
                        new TeachingAI1.Models.QuizOption { Text = "FTP", Value = "C" },
                        new TeachingAI1.Models.QuizOption { Text = "SMTP", Value = "D" }
                    },
                    AnswerIndex = 1,
                    CorrectAnswer = "B"
                }
            };
        }

        [HttpPost]
        public async Task<IActionResult> SubmitAnswers(string QuizQuestionsJson,[FromForm] string[] answers)
        {
            if (string.IsNullOrEmpty(QuizQuestionsJson))
            {
                // 可选：记录日志、返回错误页等
                TempData["ErrorMessage"] = "题目数据丢失，请重新进入考试页面。";
                return RedirectToAction("Index"); // 或其他合理跳转
            }

            List<QuizQuestion> QuizQuestions;
            try
            {
                QuizQuestions = JsonSerializer.Deserialize<List<QuizQuestion>>(QuizQuestionsJson);
            }
            catch (JsonException ex)
            {
                // 可选：记录异常 ex.Message
                TempData["ErrorMessage"] = "题目格式错误，无法解析。";
                return RedirectToAction("Index");
            }

            if (QuizQuestions == null || QuizQuestions.Count == 0)
            {
                TempData["ErrorMessage"] = "题目列表为空。";
                return RedirectToAction("Index");
            }

            var userAnswers = new List<string>();
            foreach (var answer in answers)
            {
                userAnswers.Add(answer);
            }

            while (userAnswers.Count < QuizQuestions.Count)
            {
                userAnswers.Add("");
            }

            var prompt = "你是一个教学助手，请批改以下题目...\n\n";
            for (int i = 0; i < QuizQuestions.Count; i++)
            {
                var q = QuizQuestions[i];
                prompt += $"【第{i + 1}题】\n问题：{q.QuestionText}\n选项：\n";
                foreach (var opt in q.Options)
                {
                    prompt += $"{opt.Value}. {opt.Text}\n";
                }
                prompt += $"学生作答：{(answers != null && i < answers.Length ? answers[i] : "未作答")}\n\n";
            }
            var aiResponse = await CallQwenAPI(prompt);

            var resultModel = new ExamResultViewModel
            {
                QuizQuestions = QuizQuestions,
                UserAnswers = answers,
                AiExplanation = aiResponse
            };

            return View("ExamResult", resultModel);
        }
        private string GeneratePrompt(List<QuizQuestion> QuizQuestions, string[] userAnswers)
        {
            var prompt = @"
        你是一个专业的 AI 教学助手，请根据以下题目和学生的作答，给出每道题的：
        1. 正确答案
        2. 详细解析
        3. 如果学生答错了，指出错误原因并提供学习建议

        题目如下：

        ";

            for (int i = 0; i < QuizQuestions.Count; i++)
            {
                var q = QuizQuestions[i];
                var answer = userAnswers?[i] ?? "未作答";
                prompt += $@"
        【第{i+1}题】
        问题：{q.QuestionText}
        选项：
        A. {q.Options[0].Text}
        B. {q.Options[1].Text}
        C. {q.Options[2].Text}
        D. {q.Options[3].Text}
        学生作答：{answer}

        请回答：
        ";

                if (i == QuizQuestions.Count - 1)
                    prompt += "（注意：只需返回答案和解析，不要重复问题）";
            }

            return prompt;
        }
        private async Task<string> CallQwenAPI(string prompt)
        {
            using var client = new HttpClient();
            
            var requestBody = new
            {
                model = "qwen-turbo",
                input = new { messages = new[] { new { role = "user", content = prompt } } },
                parameters = new { temperature = 0.7, top_p = 0.8 }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, "https://dashscope.aliyuncs.com/api/v1/services/aigc/text-generation/generation")
            {
                Content = content
            };

            // ✅ 只添加 Authorization（合法的请求头）
            request.Headers.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "sk-069abb4d6d41498bab30fd7e2e8bc61a");

            // ❌ 不要手动加 Content-Type！StringContent 已经设置了

            var response = await client.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                // 可选：记录错误详情
                return $"AI 服务调用失败 ({response.StatusCode})";
            }

            try
            {
                using var doc = JsonDocument.Parse(responseBody);
                var root = doc.RootElement;
                if (root.TryGetProperty("output", out var output) &&
                    output.TryGetProperty("text", out var text))
                {
                    return text.GetString()?.Trim() ?? "AI 返回内容为空";
                }
                else
                {
                    return "AI 响应格式异常";
                }
            }
            catch
            {
                return "AI 响应解析失败";
            }
        }
        

        public IActionResult WatchVideo(int id)
        {
            if (id <= 0) // 防止 0 或负数
            {
                return BadRequest("Invalid course ID");
            }
            
            var course = GetCourseById(id);
            if (course == null)
            {
                return NotFound();
            }

            // 假设每个课程有一个对应的视频文件（你可以根据模块扩展）
            // 这里简化：使用课程图片 URL 作为占位，实际应替换为 /videos/course1.mp4 等
            ViewBag.VideoUrl = $"/videos/course{id}.mp4"; // 或从模型中读取 VideoPath
            ViewBag.CourseTitle = course.Title;
            ViewBag.CourseId = id;

            return View();
        }

        public IActionResult MarkVideoComplete(int courseId)
        {
            // TODO: 未来可在此处更新数据库中的学习进度
            TempData["Message"] = "Video marked as completed!";
            return RedirectToAction("CourseDetails", new { id = courseId });
        }

        public IActionResult Assignments()
        {
            ViewData["Title"] = "My Assignments";
            return View();
        }

        public IActionResult AssignmentSubmission(int id, string assignmentName, string courseName, string dueDate, string worth, bool isOverdue = false)
        {
            ViewData["Title"] = "Assignment Submission";
            ViewData["AssignmentId"] = id;
            ViewData["AssignmentName"] = assignmentName;
            ViewData["CourseName"] = courseName;
            ViewData["DueDate"] = dueDate;
            ViewData["Worth"] = worth;
            ViewData["IsOverdue"] = isOverdue;
            
            return View();
        }

        [HttpPost]
        public IActionResult AssignmentSubmission(int assignmentId, string submissionTitle, string submissionDescription, string submissionText)
        {
            TempData["SuccessMessage"] = "Assignment submitted successfully!";
            return RedirectToAction("Assignments");
        }

        public IActionResult AssignmentFeedback(int id, string assignmentName, string courseName, string submittedDate, string dueDate, string worth, string grade, string instructorName = "Professor")
        {
            ViewData["Title"] = "Assignment Feedback";
            ViewData["AssignmentId"] = id;
            ViewData["AssignmentName"] = assignmentName;
            ViewData["CourseName"] = courseName;
            ViewData["SubmittedDate"] = submittedDate;
            ViewData["DueDate"] = dueDate;
            ViewData["Worth"] = worth;
            ViewData["Grade"] = grade;
            ViewData["InstructorName"] = instructorName;
            
            return View();
        }

        public IActionResult CourseGradeDetails(int id, string courseName, string courseCode, string credits, string instructor, string finalGrade, string currentGradePercentage, string status, string gradeClass, string gradeColorClass, string gradeTextClass)
        {
            ViewData["Title"] = "Course Grade Details";
            ViewData["CourseName"] = courseName;
            ViewData["CourseCode"] = courseCode;
            ViewData["Credits"] = credits;
            ViewData["Instructor"] = instructor;
            ViewData["FinalGrade"] = finalGrade;
            ViewData["CurrentGradePercentage"] = currentGradePercentage;
            ViewData["Status"] = status;
            ViewData["GradeClass"] = gradeClass;
            ViewData["GradeColorClass"] = gradeColorClass;
            ViewData["GradeTextClass"] = gradeTextClass;
            
            return View();
        }

        public IActionResult Grades()
        {
            ViewData["Title"] = "My Grades";
            return View();
        }

        public IActionResult Certificates()
        {
            ViewData["Title"] = "My Certificates";
            
            var certificates = new List<CertificateViewModel>
            {
                new CertificateViewModel { Title = "AI基础课程", IssueDate = DateTime.Now.AddMonths(-3) },
                new CertificateViewModel { Title = "思维力训练：用框架解决问题", IssueDate = DateTime.Now.AddMonths(-2) },
                new CertificateViewModel { Title = "一分钟建模-饼干", IssueDate = DateTime.Now.AddMonths(-1) },
                new CertificateViewModel { Title = "小猪佩奇-吹口哨", IssueDate = DateTime.Now.AddDays(-15) },
                new CertificateViewModel { Title = "耶鲁大学课程-心理学课程介绍", IssueDate = DateTime.Now.AddDays(-7) }
            };
            
            return View(certificates);
        }

        public IActionResult Calendar()
        {
            ViewData["Title"] = "My Calendar";
            return View();
        }

        public IActionResult Messages()
        {
            ViewData["Title"] = "My Messages";
            return View();
        }

        public IActionResult Profile()
        {
            ViewData["Title"] = "My Profile";
            
            var studentProfile = new StudentProfileViewModel
            {
                Name = "QinXi",
                Email = "john.doe@example.com",
                Bio = "程序课作业也太难了吧.",
                EnrolledCourses = 4,
                AvgGrade = 85,
                LearningHours = 24,
                JoinDate = DateTime.Now.AddMonths(-6),
                ProfileImage = "https://randomuser.me/api/portraits/men/32.jpg ",
                Certificates = new List<CertificateViewModel>
                {
                    new CertificateViewModel { Title = "AI基础课程", IssueDate = DateTime.Now.AddMonths(-3) },
                    new CertificateViewModel { Title = "思维力训练：用框架解决问题", IssueDate = DateTime.Now.AddMonths(-2) }
                }
            };
            
            return View(studentProfile);
        }
        
        public IActionResult EditProfile()
        {
            ViewData["Title"] = "Edit Profile";
            
            var studentProfile = new StudentProfileViewModel
            {
                Name = "John Doe",
                Email = "john.doe@example.com",
                Bio = "I'm a passionate AI and machine learning enthusiast with a background in computer science. Currently focused on 一分钟建模-饼干 in healthcare.",
                Phone = "+1 (555) 123-4567",
                Address = "123 Learning St, Education City, 12345",
                BirthDate = new DateTime(1995, 5, 15),
                Education = "Bachelor of Science in Computer Science",
                Institution = "Tech University",
                Skills = "Python, TensorFlow, Data Analysis, Machine Learning",
                EnrolledCourses = 4,
                AvgGrade = 85,
                LearningHours = 24,
                JoinDate = DateTime.Now.AddMonths(-6),
                ProfileImage = "https://randomuser.me/api/portraits/men/32.jpg "
            };
            
            return View(studentProfile);
        }
        
        [HttpPost]
        public IActionResult EditProfile(StudentProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction("Profile");
            }
            return View(model);
        }

        public IActionResult HelpCenter()
        {
            ViewData["Title"] = "Help Center";
            return View();
        }

        // ✅ 新增：AI 聊天页面（GET）
        public IActionResult AIChat()
        {
            return View();
        }

        // ✅ 新增：处理 AI 提问（POST）
        [HttpPost]
        public async Task<IActionResult> AIChat(string userQuizQuestion)
        {
            ViewBag.UserQuizQuestion = userQuizQuestion;

            if (string.IsNullOrWhiteSpace(userQuizQuestion))
            {
                ViewBag.AiResponse = "请输入问题后再提交。";
                return View();
            }

            try
            {
                var response = await _aiQuizService.GetCompletionAsync(userQuizQuestion);
                ViewBag.AiResponse = response;
            }
            catch (Exception ex)
            {
                ViewBag.AiResponse = $"AI 暂时无法回答。错误：{ex.Message}";
            }

            return View();
        }

        public IActionResult Settings()
        {
            ViewData["Title"] = "Account Settings";
            
            var userSettings = new UserSettings
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                StudentId = "S12345678"
            };
            
            return View(userSettings);
        }
        
        [HttpPost]
        public IActionResult SaveAccountSettings(UserSettings model)
        {
            if (ModelState.IsValid)
            {
                TempData["SuccessMessage"] = "Account settings updated successfully!";
                return RedirectToAction("Settings");
            }
            ViewData["Title"] = "Account Settings";
            return View("Settings", model);
        }
        
        [HttpPost]
        public IActionResult ChangePassword(UserSettings model)
        {
            if (string.IsNullOrEmpty(model.CurrentPassword))
            {
                ModelState.AddModelError("CurrentPassword", "Current password is required");
            }
            if (string.IsNullOrEmpty(model.NewPassword))
            {
                ModelState.AddModelError("NewPassword", "New password is required");
            }
            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "The new password and confirmation password do not match");
            }
            if (ModelState.IsValid)
            {
                TempData["SuccessMessage"] = "Password changed successfully!";
                return RedirectToAction("Settings");
            }
            ViewData["Title"] = "Account Settings";
            return View("Settings", model);
        }
        
        [HttpPost]
        public IActionResult SavePreferences(UserSettings model)
        {
            if (ModelState.IsValid)
            {
                TempData["SuccessMessage"] = "Preferences updated successfully!";
                return RedirectToAction("Settings");
            }
            ViewData["Title"] = "Account Settings";
            return View("Settings", model);
        }
        
        [HttpPost]
        public IActionResult SaveNotifications(UserSettings model)
        {
            if (ModelState.IsValid)
            {
                TempData["SuccessMessage"] = "Notification settings updated successfully!";
                return RedirectToAction("Settings");
            }
            ViewData["Title"] = "Account Settings";
            return View("Settings", model);
        }
        
        [HttpPost]
        public IActionResult SavePrivacySettings(UserSettings model)
        {
            if (ModelState.IsValid)
            {
                TempData["SuccessMessage"] = "Privacy settings updated successfully!";
                return RedirectToAction("Settings");
            }
            ViewData["Title"] = "Account Settings";
            return View("Settings", model);
        }
        
        [HttpPost]
        public IActionResult SaveAppearance(UserSettings model)
        {
            if (ModelState.IsValid)
            {
                TempData["SuccessMessage"] = "Appearance settings updated successfully!";
                return RedirectToAction("Settings");
            }
            ViewData["Title"] = "Account Settings";
            return View("Settings", model);
        }
        
        [HttpPost]
        public IActionResult SaveAccessibility(UserSettings model)
        {
            if (ModelState.IsValid)
            {
                TempData["SuccessMessage"] = "Accessibility settings updated successfully!";
                return RedirectToAction("Settings");
            }
            ViewData["Title"] = "Account Settings";
            return View("Settings", model);
        }
        
        [HttpPost]
        public IActionResult ConnectAccount(string provider)
        {
            TempData["SuccessMessage"] = $"Connected to {provider} successfully!";
            return RedirectToAction("Settings");
        }
        
        [HttpPost]
        public IActionResult DisconnectAccount(string provider)
        {
            TempData["SuccessMessage"] = $"Disconnected from {provider} successfully!";
            return RedirectToAction("Settings");
        }
        
        [HttpPost]
        public IActionResult DeleteAccount()
        {
            TempData["SuccessMessage"] = "Account deleted successfully!";
            return RedirectToAction("Login", "Account");
        }

        private readonly IAiQuizService _aiQuizService;
        private readonly ICourseService _courseService;
        public StudentController(IAiQuizService aiQuizService,ICourseService courseService)
        {
            _aiQuizService = aiQuizService;
            _courseService = courseService;
        }
        private async Task<List<QuizQuestion>> GetQuizQuestionsForCourse(int courseId)
        {
            try
            {
                var courseContext = await _courseService.GetCourseContentAsync(courseId);
                return await _aiQuizService.GenerateQuizQuestionsAsync(courseContext, 3);
            }
            catch (Exception ex)
            {
                // ✅ 如果 AI 失败，返回测试数据
                return new List<QuizQuestion>
                {
                    new QuizQuestion
                    {
                        QuestionText = "这是测试题目",
                        Options = new List<TeachingAI1.Models.QuizOption>
                        {
                            new TeachingAI1.Models.QuizOption { Text = "选项A", Value = "A" },
                            new TeachingAI1.Models.QuizOption { Text = "选项B", Value = "B" },
                            new TeachingAI1.Models.QuizOption { Text = "选项C", Value = "C" }
                        },
                        AnswerIndex = 1, // 正确答案是 B
                        Explanation = "这是题目的解析。"
                    }
                };
            }
        }
    }
}


public class StudentProfileViewModel
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Bio { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public DateTime? BirthDate { get; set; }
    public string ProfileImage { get; set; }
    public string Education { get; set; }
    public string Institution { get; set; }
    public string Skills { get; set; }
    
    // Stats
    public int EnrolledCourses { get; set; }
    public int AvgGrade { get; set; }
    public int LearningHours { get; set; }
    public DateTime JoinDate { get; set; }
    
    // Navigation properties
    public List<CertificateViewModel> Certificates { get; set; }
}

public class CertificateViewModel
{
    public string Title { get; set; }
    public DateTime IssueDate { get; set; }
}
