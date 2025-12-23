using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TeachingAI1.Models;

namespace TeachingAI1.Controllers
{
    [Route("Error")]
    public class ErrorController : Controller
    {
        [Route("")]
        [Route("{statusCode}")]
        public IActionResult Index(int? statusCode = null)
        {
            var error = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                StatusCode = statusCode ?? 500
            };

            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            if (exceptionFeature != null)
            {
                error.ErrorMessage = exceptionFeature.Error.Message;
            }

            return View("Error", error);
        }

        [Route("404")]
        public IActionResult PageNotFound()
        {
            var error = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                StatusCode = 404
            };

            return View("Error", error);
        }

        [Route("500")]
        public IActionResult InternalServerError()
        {
            var error = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                StatusCode = 500,
                ErrorMessage = "An internal server error occurred."
            };

            return View("Error", error);
        }

        [Route("403")]
        public IActionResult Forbidden()
        {
            var error = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                StatusCode = 403,
                ErrorMessage = "You do not have permission to access this resource."
            };

            return View("Error", error);
        }
    }
} 