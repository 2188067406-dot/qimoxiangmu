using Microsoft.AspNetCore.Mvc;

namespace TeachingAI.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Lessons()
        {
            return View();
        }

        public IActionResult Courses()
        {
            return View();
        }

        public IActionResult ProgressTracking()
        {
            return View();
        }
    }
}
