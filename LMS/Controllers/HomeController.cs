using LMS.Models;
using LMS.Service.Abstract;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICourseService _courseService;
        public HomeController(ILogger<HomeController> logger, ICourseService courseService)
        {
            _logger = logger;
            _courseService = courseService;
        }

        public IActionResult Index()
        {
            var courses = _courseService.GetAll();
            return View(courses);
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? id)
        {
            // Reached both from /Home/Error/{code} (status code re-execute) and from the exception handler,
            // where the response already carries 500.
            var statusCode = id ?? Response.StatusCode;
            if (statusCode is < 400 or > 599)
            {
                statusCode = StatusCodes.Status500InternalServerError;
            }

            Response.StatusCode = statusCode;

            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                StatusCode = statusCode
            });
        }
    }
}
