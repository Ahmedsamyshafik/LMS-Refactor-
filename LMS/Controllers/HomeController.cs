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

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
