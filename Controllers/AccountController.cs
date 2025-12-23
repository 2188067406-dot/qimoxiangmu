using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeachingAI1.Data;
using TeachingAI1.Models;

namespace TeachingAI1.Controllers
{
    public class AccountController : Controller
    {

        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.Roles = new[] { "Student", "Teacher", "Admin" };
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        //For sign in - members
        [HttpPost]
        public IActionResult Register(string username, string password, string role, string email, string studentOrStaffId)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Name = username,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                    Email = email,
                    Role = role,
                    CreatedAt = DateTime.Now,
                    StudentOrStaffId = studentOrStaffId,
                    IsLoggedIn = false,
                    LastActivity = DateTime.Now
                };

                _context.Users.Add(user);
                _context.SaveChanges();
                return RedirectToAction("Login");
            }

            // ❗关键：模型验证失败时，必须重新设置角色列表！
            ViewBag.Roles = new[] { "Student", "Teacher", "Admin" };
            return View();
        }

        //For log in - members
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Name == username);
           
           if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
           {
                //set session or authentication logic here
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Name),          //User's name
                    new Claim(ClaimTypes.Email, user.Email),        //User's email
                    new Claim("UserId", user.Id.ToString()),        //Custome claim for user ID
                    new Claim(ClaimTypes.Role, user.Role)           //User's role
                };

                //Create the identity and principal
                //Identity: This identity represents the user's authentication and holds the claims
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                //principal: This principal is the main object that represents the user and is used for authorization
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                //Sign the user in with the claims
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                //Redirect based on the user's role
                return RedirectToAction("Dashboard", user.Role);

                // HttpContext.Session.SetString("Role", user.Role);
           }

           //If login fails
           ModelState.AddModelError("", "Invalid login");
           return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // var user = await _userManager.GetUserAsync(User); 
            var UserId = User.FindFirst("UserId")?.Value; //get the logged in user

            if(!string.IsNullOrEmpty(UserId))
            {
                //Fetching the user from the databse
                var user  = await _context.Users.FindAsync(int.Parse(UserId));
                //Logic for checking if anyone has logged in or not
                if(user != null){
                    //Updating user's loggedin status
                    user.IsLoggedIn = false;
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
            }
            //Sign out the user and clear the authentication cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Dashboard(string role)
        {
            switch (role)
            {
                case "Student":
                    return RedirectToAction("Index1", "Student");
                case "Teacher":
                    return RedirectToAction("Index1", "Teacher");
                case "Admin":
                    return RedirectToAction("Index1", "Admin");
                default:
                    return RedirectToAction("Login");

            }
        }

        public IActionResult Profile()
        {
            var username = User.FindFirst(ClaimTypes.Name)?. Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var userId = User.FindFirst("UserId")?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            return View();
        }

  }
}
