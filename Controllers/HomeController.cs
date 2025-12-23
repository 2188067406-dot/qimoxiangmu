using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TeachingAI1.Models;
using TeachingAI1.ViewModels;

namespace TeachingAI1.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        ViewData["ShowSearch"] = false;

        var courses = new List<CourseViewModel>
            {
                new CourseViewModel
                { 
                    Id = 1, 
                    Title = "AI基础课程", 
                    Instructor = "A老师",
                    Progress = 75,
                    LessonsTotal = 12,
                    LessonsCompleted = 9,
                    TimeLeft = "6h left",
                    Status = "In Progress"
                },
                new CourseViewModel 
                { 
                    Id = 2, 
                    Title = "思维力训练：用框架解决问题", 
                    Instructor = "B老师",
                    Progress = 45,
                    LessonsTotal = 16,
                    LessonsCompleted = 7,
                    TimeLeft = "12h left",
                    Status = "In Progress"
                },
                new CourseViewModel 
                { 
                    Id = 3, 
                    Title = "一分钟建模-饼干", 
                    Instructor = "C老师",
                    Progress = 60,
                    LessonsTotal = 10,
                    LessonsCompleted = 6,
                    TimeLeft = "5h left",
                    Status = "In Progress"
                },
                new CourseViewModel
                { 
                    Id = 4, 
                    Title = "耶鲁大学课程-心理学课程介绍", 
                    Instructor = "D老师",
                    Progress = 100,
                    LessonsTotal = 20,
                    LessonsCompleted = 20,
                    TimeLeft = "0h left",
                    Status = "Completed"
                },
                new CourseViewModel
                { 
                    Id = 5, 
                    Title = "小猪佩奇-吹口哨", 
                    Instructor = "E老师",
                    Progress = 100,
                    LessonsTotal = 15,
                    LessonsCompleted = 15,
                    TimeLeft = "0h left",
                    Status = "Completed"
                }
            };
        return View(courses);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }

    public IActionResult Search(string query)
    {
        //Perform search logics here (eg; query a database or filter results)
        ViewBag.SearchQuery = query;
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { 
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            StatusCode = 500
        });
    }
}
