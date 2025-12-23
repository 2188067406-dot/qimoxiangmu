using Microsoft.AspNetCore.Mvc;
using TeachingAI1.Models;
using TeachingAI1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TeachingAI1.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace TeachingAI1.Controllers
{
    public class TeacherController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostEnvironment _environment;

        public TeacherController(ApplicationDbContext context, IHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Teacher
        public ActionResult Index()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            ViewData["ShowSearch"] = true;
            ViewData["Title"] = "Teacher Dashboard";
            
            ViewBag.ActiveCourses = _context.Courses.Count(c => c.Status == "Active");
            ViewBag.TotalStudents = _context.Users.Count();
            ViewBag.PendingAssignments = 12;
            ViewBag.NewMessages = 24;
            
            return View();
        }
        
        public IActionResult Courses()
        {
            ViewData["Title"] = "My Courses";
            
            int teacherId = 1; // Placeholder
            
            var courses = _context.Courses
                .Where(c => c.TeacherId == teacherId)
                .Select(c => new ViewModels.CourseViewModel
                {
                    Id = c.Id,
                    Title = c.Name,
                    Description = "No description available",
                    EnrolledStudents = 32,
                    Duration = "12 Weeks",
                    Attendance = 87,
                    Completion = 92,
                    Rating = 4.8,
                    Status = c.Status,
                    StartDate = DateTime.Now.AddDays(-30),
                    EndDate = DateTime.Now.AddDays(60)
                })
                .ToList();
            
            if (!courses.Any())
            {
                courses = GetSampleCourses();
            }
            
            return View(courses);
        }
        
        private List<ViewModels.CourseViewModel> GetSampleCourses()
        {
            return new List<ViewModels.CourseViewModel>
            {
                new ViewModels.CourseViewModel
                {
                    Id = 1,
                    Title = "AI基础课程",
                    Description = "本AI基础课程面向零基础学习者，系统讲解AI核心概念、机器学习等基础技术及典型应用，帮助快速建立AI认知框架，轻松入门人工智能领域。",
                    EnrolledStudents = 32,
                    Duration = "12 Weeks",
                    Attendance = 87,
                    Completion = 92,
                    Rating = 4.8,
                    Status = "Active",
                    StartDate = DateTime.Now.AddDays(-30),
                    EndDate = DateTime.Now.AddDays(60)
                }
            };
        }
        
        public IActionResult CourseDetails(int id)
        {
            ViewData["Title"] = "Course Details";
            
            var dbCourse = _context.Courses.FirstOrDefault(c => c.Id == id);
            
            if (dbCourse != null)
            {
                var course = new ViewModels.CourseViewModel
                {
                    Id = dbCourse.Id,
                    Title = dbCourse.Name,
                    Description = "No description available",
                    EnrolledStudents = 32,
                    Duration = "12 Weeks",
                    Attendance = 87,
                    Completion = 92,
                    Rating = 4.8,
                    Status = dbCourse.Status,
                    StartDate = DateTime.Now.AddDays(-30),
                    EndDate = DateTime.Now.AddDays(60),
                    Syllabus = "Week 1: AI基础课程\nWeek 2: Problem Solving\n..."
                };
                return View(course);
            }
            
            var sampleCourse = new ViewModels.CourseViewModel
            {
                Id = id,
                Title = id == 1 ? "AI基础课程" : "思维力训练：用框架解决问题",
                Description = "本AI基础课程面向零基础学习者，系统讲解AI核心概念、机器学习等基础技术及典型应用，帮助快速建立AI认知框架，轻松入门人工智能领域。",
                EnrolledStudents = id == 1 ? 32 : 28,
                Duration = id == 1 ? "12 Weeks" : "16 Weeks",
                Attendance = id == 1 ? 87 : 92,
                Completion = id == 1 ? 92 : 78,
                Rating = id == 1 ? 4.8 : 4.6,
                Status = "Active",
                StartDate = DateTime.Now.AddDays(-30),
                EndDate = DateTime.Now.AddDays(60),
                Syllabus = "Week 1: AI基础课程\nWeek 2: Problem Solving\nWeek 3: Knowledge Representation\nWeek 4: Machine Learning Basics\nWeek 5: Neural Networks\nWeek 6: 耶鲁大学课程-心理学课程介绍\nWeek 7: 小猪佩奇-吹口哨\nWeek 8: Robotics\nWeek 9: Ethics in AI\nWeek 10: Future of AI\nWeek 11: Project Work\nWeek 12: Final Presentations"
            };
            
            return View(sampleCourse);
        }
        
        [HttpGet]
        public IActionResult CreateCourse()
        {
            ViewData["Title"] = "Create New Course";
            
            var model = new ViewModels.CourseViewModel
            {
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddMonths(3),
                Status = "Active"
            };
            
            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCourse(CourseViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int? userId = null;

                    if (User.Identity.IsAuthenticated && User.FindFirstValue(ClaimTypes.NameIdentifier) != null)
                    {
                        userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    }
                    else if (HttpContext.Session.GetInt32("UserId") != null)
                    {
                        userId = HttpContext.Session.GetInt32("UserId");
                    }
                    else if (_environment.IsDevelopment())
                    {
                        userId = 1;
                        ModelState.AddModelError("", "Using test teacher (ID=1) since no authenticated user found");
                    }
                    else
                    {
                        ModelState.AddModelError("", "You must be logged in as a teacher to create courses.");
                        return View(model);
                    }

                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                    if (user == null)
                    {
                        ModelState.AddModelError("", $"User with ID {userId} not found.");
                        return View(model);
                    }

                    if (user.Role != "Teacher")
                    {
                        ModelState.AddModelError("", "Only users with the Teacher role can create courses.");
                        return View(model);
                    }

                    var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == userId);

                    if (teacher == null)
                    {
                        teacher = new Teacher { UserId = user.Id };
                        _context.Teachers.Add(teacher);
                        await _context.SaveChangesAsync();
                        ModelState.AddModelError("", $"Created teacher record for user {user.Name}");
                    }

                    // ✅ 在 try 块内创建并保存 course
                    var course = new Course
                    {
                        Name = model.Title,
                        Status = model.Status,
                        Content = model.Content,
                        TeacherId = teacher.Id,
                        Modules = new List<Module>
                        {
                            new Module
                            {
                                Title = "第一章",
                                Description = "基础介绍",
                                LessonCount = 3,
                                CompletedLessons = 0,
                                Status = "Not Started",
                                Lessons = new List<Lesson>
                                {
                                    new Lesson { Title = "Lesson 1", Content = "" },
                                    new Lesson { Title = "Lesson 2", Content = "" },
                                    new Lesson { Title = "Lesson 3", Content = "" }
                                }
                            },
                            new Module
                            {
                                Title = "第二章",
                                Description = "初步理解",
                                LessonCount = 4,
                                CompletedLessons = 0,
                                Status = "Not Started",
                                Lessons = new List<Lesson>
                                {
                                    new Lesson { Title = "Lesson 1", Content = "" },
                                    new Lesson { Title = "Lesson 2", Content = "" },
                                    new Lesson { Title = "Lesson 3", Content = "" },
                                    new Lesson { Title = "Lesson 4", Content = "" }
                                }
                            },
                            new Module
                            {
                                Title = "第三章",
                                Description = "深入了解",
                                LessonCount = 3,
                                CompletedLessons = 0,
                                Status = "Not Started",
                                Lessons = new List<Lesson>
                                {
                                    new Lesson { Title = "Lesson 1", Content = "" },
                                    new Lesson { Title = "Lesson 2", Content = "" },
                                    new Lesson { Title = "Lesson 3", Content = "" }
                                }
                            },
                            new Module
                            {
                                Title = "第四章",
                                Description = "总结回顾",
                                LessonCount = 2,
                                CompletedLessons = 0,
                                Status = "Not Started",
                                Lessons = new List<Lesson>
                                {
                                    new Lesson { Title = "Lesson 1", Content = "" },
                                    new Lesson { Title = "Lesson 2", Content = "" }
                                }
                            }
                        }
                    };

                    // ✅ 这两行必须在 try 块内！
                    _context.Courses.Add(course);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Course created successfully!";
                    return RedirectToAction(nameof(Courses));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while saving the course. Please try again.");
                    ModelState.AddModelError("", ex.Message);

                    if (ex.InnerException != null)
                    {
                        ModelState.AddModelError("", $"Inner Exception: {ex.InnerException.Message}");
                    }
                }
            }

            return View(model);
        }
        
        [HttpGet]
        public IActionResult EditCourse(int id)
        {
            ViewData["Title"] = "Edit Course";
            
            var dbCourse = _context.Courses.FirstOrDefault(c => c.Id == id);
            
            if (dbCourse != null)
            {
                var course = new ViewModels.CourseViewModel
                {
                    Id = dbCourse.Id,
                    Title = dbCourse.Name,   // Map DB Name → ViewModel Title
                    Description = "No description available",
                    Status = dbCourse.Status,
                    StartDate = DateTime.Now.AddDays(-30),
                    EndDate = DateTime.Now.AddDays(60)
                };
                return View(course);
            }
            
            var sampleCourse = new ViewModels.CourseViewModel
            {
                Id = id,
                Title = id == 1 ? "AI基础课程" : "思维力训练：用框架解决问题",
                Description = "本AI基础课程面向零基础学习者，系统讲解AI核心概念、机器学习等基础技术及典型应用，帮助快速建立AI认知框架，轻松入门人工智能领域。",
                EnrolledStudents = id == 1 ? 32 : 28,
                Duration = id == 1 ? "12 Weeks" : "16 Weeks",
                Status = "Active",
                StartDate = DateTime.Now.AddDays(-30),
                EndDate = DateTime.Now.AddDays(60),
                Syllabus = "Week 1: AI基础课程\nWeek 2: Problem Solving\nWeek 3: Knowledge Representation\nWeek 4: Machine Learning Basics\nWeek 5: Neural Networks\nWeek 6: 耶鲁大学课程-心理学课程介绍\nWeek 7: 小猪佩奇-吹口哨\nWeek 8: Robotics\nWeek 9: Ethics in AI\nWeek 10: Future of AI\nWeek 11: Project Work\nWeek 12: Final Presentations"
            };
            
            return View(sampleCourse);
        }
        
        [HttpPost]
        public async Task<IActionResult> EditCourse(ViewModels.CourseViewModel model)
        {
            ModelState.Remove("Students");
            ModelState.Remove("Assignments");
            ModelState.Remove("Grade");
            
            if (ModelState.IsValid)
            {
                try
                {
                    var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == model.Id);
                    
                    if (course != null)
                    {
                        // ✅ FIXED: Assign model.Title to course.Name (consistent with CreateCourse)
                        course.Name = model.Title;
                        course.Status = model.Status;
                        
                        await _context.SaveChangesAsync();
                        
                        TempData["SuccessMessage"] = "Course updated successfully!";
                        return RedirectToAction("CourseDetails", new { id = model.Id });
                    }
                    else
                    {
                        ModelState.AddModelError("", "Course not found.");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            
            ViewData["Title"] = "Edit Course";
            return View(model);
        }
        
        public IActionResult Students()
        {
            ViewData["Title"] = "Students";
            
            var students = new List<TeachingAI1.ViewModels.StudentViewModel>
            {
                new TeachingAI1.ViewModels.StudentViewModel
                {
                    Id = 1,
                    Name = "John Doe",
                    Email = "john.doe@example.com",
                    EnrolledCourses = 3,
                    OverallProgress = 85,
                    JoinDate = DateTime.Now.AddDays(-90)
                },
                new TeachingAI1.ViewModels.StudentViewModel
                {
                    Id = 2,
                    Name = "Jane Smith",
                    Email = "jane.smith@example.com",
                    EnrolledCourses = 2,
                    OverallProgress = 92,
                    JoinDate = DateTime.Now.AddDays(-45)
                },
                new TeachingAI1.ViewModels.StudentViewModel
                {
                    Id = 3,
                    Name = "Michael Johnson",
                    Email = "michael.johnson@example.com",
                    EnrolledCourses = 4,
                    OverallProgress = 78,
                    JoinDate = DateTime.Now.AddDays(-180)
                },
                new TeachingAI1.ViewModels.StudentViewModel
                {
                    Id = 4,
                    Name = "Sarah Williams",
                    Email = "sarah.williams@example.com",
                    EnrolledCourses = 1,
                    OverallProgress = 65,
                    JoinDate = DateTime.Now.AddDays(-15)
                },
                new TeachingAI1.ViewModels.StudentViewModel
                {
                    Id = 5,
                    Name = "Robert Brown",
                    Email = "robert.brown@example.com",
                    EnrolledCourses = 3,
                    OverallProgress = 45,
                    JoinDate = DateTime.Now.AddDays(-30)
                },
                new TeachingAI1.ViewModels.StudentViewModel
                {
                    Id = 6,
                    Name = "Emily Davis",
                    Email = "emily.davis@example.com",
                    EnrolledCourses = 2,
                    OverallProgress = 88,
                    JoinDate = DateTime.Now.AddDays(-10)
                },
                new TeachingAI1.ViewModels.StudentViewModel
                {
                    Id = 7,
                    Name = "David Miller",
                    Email = "david.miller@example.com",
                    EnrolledCourses = 3,
                    OverallProgress = 72,
                    JoinDate = DateTime.Now.AddDays(-60)
                },
                new TeachingAI1.ViewModels.StudentViewModel
                {
                    Id = 8,
                    Name = "Jessica Wilson",
                    Email = "jessica.wilson@example.com",
                    EnrolledCourses = 1,
                    OverallProgress = 25,
                    JoinDate = DateTime.Now.AddDays(-5)
                }
            };
            
            return View(students);
        }
        
        public IActionResult StudentDetails(int id)
        {
            ViewData["Title"] = "Student Details";
            
            var student = new TeachingAI1.ViewModels.StudentViewModel
            {
                Id = id,
                Name = id == 1 ? "John Doe" : "Jane Smith",
                Email = id == 1 ? "john.doe@example.com" : "jane.smith@example.com",
                EnrolledCourses = id == 1 ? 3 : 2,
                OverallProgress = id == 1 ? 85 : 92,
                JoinDate = DateTime.Now.AddDays(-90),
                Courses = new List<ViewModels.CourseViewModel>
                {
                    new ViewModels.CourseViewModel
                    {
                        Id = 1,
                        Title = "AI基础课程",
                        Progress = 90,
                        Grade = "A-"
                    },
                    new ViewModels.CourseViewModel
                    {
                        Id = 2,
                        Title = "思维力训练：用框架解决问题",
                        Progress = 85,
                        Grade = "B+"
                    },
                    new ViewModels.CourseViewModel
                    {
                        Id = 3,
                        Title = "一分钟建模-饼干",
                        Progress = 75,
                        Grade = "B"
                    }
                }
            };
            
            return View(student);
        }
        
        public IActionResult Assignments()
        {
            ViewData["Title"] = "Assignments";
            
            var assignments = new List<AssignmentViewModel>
            {
                new AssignmentViewModel
                {
                    Id = 1,
                    Title = "AI基础课程",
                    CourseTitle = "AI基础课程",
                    DueDate = DateTime.Now.AddDays(1),
                    AssignedStudents = 32,
                    SubmittedCount = 18,
                    Status = "Upcoming"
                },
                new AssignmentViewModel
                {
                    Id = 2,
                    Title = "思维力训练：用框架解决问题",
                    CourseTitle = "思维力训练：用框架解决问题",
                    DueDate = DateTime.Now.AddDays(3),
                    AssignedStudents = 28,
                    SubmittedCount = 12,
                    Status = "Upcoming"
                },
                new AssignmentViewModel
                {
                    Id = 3,
                    Title = "一分钟建模-饼干",
                    CourseTitle = "一分钟建模-饼干",
                    DueDate = DateTime.Now.AddDays(7),
                    AssignedStudents = 24,
                    SubmittedCount = 5,
                    Status = "Upcoming"
                },
                new AssignmentViewModel
                {
                    Id = 4,
                    Title = "耶鲁大学课程-心理学课程介绍",
                    CourseTitle = "耶鲁大学课程-心理学课程介绍",
                    DueDate = DateTime.Now.AddDays(-2),
                    AssignedStudents = 22,
                    SubmittedCount = 20,
                    Status = "Past Due"
                },
                new AssignmentViewModel
                {
                    Id = 5,
                    Title = "小猪佩奇-吹口哨",
                    CourseTitle = "小猪佩奇-吹口哨",
                    DueDate = DateTime.Now.AddDays(-5),
                    AssignedStudents = 20,
                    SubmittedCount = 18,
                    Status = "Graded"
                }
            };
            
            return View(assignments);
        }
        
        public IActionResult AssignmentDetails(int id)
        {
            ViewData["Title"] = "Assignment Details";
            
            var assignment = new AssignmentViewModel
            {
                Id = id,
                Title = id == 1 ? "AI Ethics Case Study" : "Neural Networks Project",
                CourseTitle = id == 1 ? "AI基础课程" : "思维力训练：用框架解决问题",
                DueDate = DateTime.Now.AddDays(id == 1 ? 1 : 3),
                AssignedStudents = id == 1 ? 32 : 28,
                SubmittedCount = id == 1 ? 18 : 12,
                Status = "Upcoming",
                Description = "In this assignment, students will explore the ethical implications of AI in various contexts. They will analyze case studies and provide recommendations for addressing ethical concerns.",
                Instructions = "1. Choose one of the provided case studies\n2. Identify the ethical issues involved\n3. Research relevant ethical frameworks\n4. Analyze the case using these frameworks\n5. Provide recommendations\n6. Submit a 1500-word report",
                PointsPossible = 100,
                Submissions = new List<SubmissionViewModel>
                {
                    new SubmissionViewModel
                    {
                        StudentName = "John Doe",
                        SubmissionDate = DateTime.Now.AddDays(-1),
                        Status = "Submitted",
                        Grade = null
                    },
                    new SubmissionViewModel
                    {
                        StudentName = "Jane Smith",
                        SubmissionDate = DateTime.Now.AddDays(-2),
                        Status = "Submitted",
                        Grade = null
                    },
                    new SubmissionViewModel
                    {
                        StudentName = "Michael Johnson",
                        SubmissionDate = null,
                        Status = "Not Submitted",
                        Grade = null
                    }
                }
            };
            
            return View(assignment);
        }
        
        [HttpGet]
        public IActionResult CreateAssignment()
        {
            ViewData["Title"] = "Create New Assignment";
            
            ViewBag.Courses = new List<ViewModels.CourseViewModel>
            {
                new ViewModels.CourseViewModel { Id = 1, Title = "AI基础课程" },
                new ViewModels.CourseViewModel { Id = 2, Title = "思维力训练：用框架解决问题" },
                new ViewModels.CourseViewModel { Id = 3, Title = "一分钟建模-饼干" },
                new ViewModels.CourseViewModel { Id = 4, Title = "耶鲁大学课程-心理学课程介绍" },
                new ViewModels.CourseViewModel { Id = 5, Title = "小猪佩奇-吹口哨" }
            };
            
            return View();
        }
        
        [HttpPost]
        public IActionResult CreateAssignment(AssignmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                TempData["SuccessMessage"] = "Assignment created successfully!";
                return RedirectToAction("Assignments");
            }
            
            ViewData["Title"] = "Create New Assignment";
            
            ViewBag.Courses = new List<ViewModels.CourseViewModel>
            {
                new ViewModels.CourseViewModel { Id = 1, Title = "AI基础课程" },
                new ViewModels.CourseViewModel { Id = 2, Title = "思维力训练：用框架解决问题" },
                new ViewModels.CourseViewModel { Id = 3, Title = "一分钟建模-饼干" },
                new ViewModels.CourseViewModel { Id = 4, Title = "耶鲁大学课程-心理学课程介绍" },
                new ViewModels.CourseViewModel { Id = 5, Title = "小猪佩奇-吹口哨" }
            };
            
            return View(model);
        }
        
        public IActionResult Messages()
        {
            ViewData["Title"] = "Messages";
            return View();
        }
        
        public IActionResult Profile()
        {
            ViewData["Title"] = "Teacher Profile";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult GradeSubmissions(int id)
        {
            ViewData["Title"] = "Grade Submissions";
            
            var assignment = new AssignmentViewModel
            {
                Id = id,
                Title = id == 1 ? "AI Ethics Case Study" : "Neural Networks Project",
                CourseTitle = id == 1 ? "AI基础课程" : "思维力训练：用框架解决问题",
                DueDate = DateTime.Now.AddDays(id == 1 ? 1 : 3),
                AssignedStudents = id == 1 ? 32 : 28,
                SubmittedCount = id == 1 ? 18 : 12,
                Status = "Upcoming",
                Description = "In this assignment, students will explore the ethical implications of AI in various contexts.",
                Instructions = "1. Choose one of the provided case studies\n2. Identify the ethical issues involved",
                PointsPossible = 100,
                Submissions = new List<SubmissionViewModel>
                {
                    new SubmissionViewModel
                    {
                        Id = 1,
                        StudentName = "John Doe",
                        SubmissionDate = DateTime.Now.AddDays(-1),
                        Status = "Submitted",
                        Grade = null,
                        Feedback = ""
                    },
                    new SubmissionViewModel
                    {
                        Id = 2,
                        StudentName = "Jane Smith",
                        SubmissionDate = DateTime.Now.AddDays(-2),
                        Status = "Submitted",
                        Grade = 85,
                        Feedback = "Good work! Your analysis of the ethical implications was thorough."
                    },
                    new SubmissionViewModel
                    {
                        Id = 3,
                        StudentName = "Michael Johnson",
                        SubmissionDate = null,
                        Status = "Not Submitted",
                        Grade = null,
                        Feedback = ""
                    },
                    new SubmissionViewModel
                    {
                        Id = 4,
                        StudentName = "Sarah Williams",
                        SubmissionDate = DateTime.Now.AddDays(-1).AddHours(3),
                        Status = "Submitted",
                        Grade = null,
                        Feedback = ""
                    },
                    new SubmissionViewModel
                    {
                        Id = 5,
                        StudentName = "Robert Brown",
                        SubmissionDate = DateTime.Now.AddDays(-3),
                        Status = "Graded",
                        Grade = 92,
                        Feedback = "Excellent work! Your analysis was comprehensive and well-reasoned."
                    },
                    new SubmissionViewModel
                    {
                        Id = 6,
                        StudentName = "Emily Davis",
                        SubmissionDate = DateTime.Now.AddDays(2),
                        Status = "Late",
                        Grade = null,
                        Feedback = ""
                    }
                }
            };
            
            return View(assignment);
        }
        
        [HttpPost]
        public IActionResult SaveGrade(int submissionId, int assignmentId, int? grade, string feedback)
        {
            return Json(new { success = true, message = "Grade saved successfully" });
        }
        
        [HttpPost]
        public IActionResult SendReminder(int assignmentId, List<int> studentIds)
        {
            return Json(new { success = true, message = $"Reminders sent to {studentIds.Count} students" });
        }
        
        public IActionResult PublicProfile()
        {
            ViewData["Title"] = "Public Profile";
            return View();
        }
        
        [HttpPost]
        public IActionResult UpdateProfileVisibility([FromBody] ProfileVisibilityModel model)
        {
            return Json(new { success = true, message = model.IsEnabled ? "Profile enabled" : "Profile disabled" });
        }
    

        public class ProfileVisibilityModel
        {
            public bool IsEnabled { get; set; }
        }
    }
}