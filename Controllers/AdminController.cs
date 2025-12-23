using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeachingAI1.Data;
using TeachingAI1.Models;
using System.Linq;
using Microsoft.AspNetCore.Authentication;

namespace TeachingAI1.Controllers
{
    public class Admin : Controller
    {
        private readonly ApplicationDbContext _context;

        public Admin(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin
        // public ActionResult Index1()
        // {
        //     return View();
        // }
        
        [Authorize(Roles = "Admin")]
        public IActionResult Settings()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            ViewData["ShowSearch"] = true;
            return View();
        }

       public IActionResult Index1(string search)
{
    var users = _context.Users
        .Select(u => new
        {
            u.Id,
            u.Name,
            u.Email,
            u.Role,
            u.IsLoggedIn,
            u.LastActivity
        })
        .ToList();

    Console.WriteLine($"Number of users: {users.Count}");
    return View("Index1", users);
}

        [Authorize]
        public IActionResult Logout()
        {
            // This will sign out the user
            HttpContext.SignOutAsync();
            return RedirectToAction("Index1"); // Redirect to the desired page after logout
        }
    }
}
