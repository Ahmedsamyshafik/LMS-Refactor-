using LMS.Helper;
using LMS.Models.Entities;
using LMS.Service.Abstract;
using LMS.ViewModels.QuizVM;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;

namespace LMS.Controllers
{
	public class QuizController : Controller
	{
		#region Fields
		private readonly IQuizService _quizService;
		private readonly IStudentQuizService _studentQuizService;
		private readonly IMediaService _mediaService;

		#endregion

		#region Ctor
		public QuizController(IQuizService quizService, IStudentQuizService studentQuizService, IMediaService mediaService)
		{
			_quizService = quizService;
			_studentQuizService = studentQuizService;
			_mediaService = mediaService;
		}
		#endregion

		#region Actions



		[HttpGet]
		public IActionResult Add(int id)
		{
			ViewBag.id = id;
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Add(Quiz quiz)//make id = 0 
		{
			quiz.Id = 0;
			await _quizService.Add(quiz);
			return View();
		}
		[HttpGet]
		public IActionResult AddWritten(int id)
		{
			var obj = new AddWritten()
			{
				CourseId = id
			};
			return View(obj);
		}
		[HttpPost]
		public async Task<IActionResult> AddWritten(AddWritten obj)
		{
			//save pdf
			string path = await _mediaService.UploadImage(obj.file, Constants.Quiz);
			//then save quiz
			var quiz = new Quiz()
			{
				courseId = obj.CourseId,
				Description = obj.description,
				Name = obj.Name,
				FilePath = path
			};
			await _quizService.Add(quiz);
			return View(obj);
		}

		[HttpGet]
		public IActionResult GetAll(int id)//CourseId
		{
			string userId = GetUserId();
			var obj = new GetAllVM();
			var Qz = _quizService.GetAllByCourseId(id);
			var ListOfQuizes = new List<inhertQuiz>();
			//forloop in quizes to know if user take it or no and assign to VM
			foreach (var item in Qz)
			{
				var temp = new inhertQuiz()
				{
					courseId = item.courseId,
					Description = item.Description,
					FilePath = item.FilePath,
					Name = item.Name,
					StudentQuizzes = item.StudentQuizzes,
					Course = item.Course,
					Id = item.Id,
					Questions = item.Questions
				};
				temp.StudentTakedExam = _studentQuizService.StudentTakeExam(userId, item.Id);

				ListOfQuizes.Add(temp);
			}

			var students = _studentQuizService.GetStudents(id);
			obj.students = students;
			obj.quizzes = ListOfQuizes;
			return View(obj);
		}

		[HttpGet]//Quiz --
		public IActionResult Get(int id) //quiz id
		{
			DateTime startTime = DateTime.Now;
			HttpContext.Session.Set("StartTime", Encoding.UTF8.GetBytes(startTime.ToString()));

			// Calculate the end time of the exam (e.g., 1 hour time limit)
			DateTime endTime = startTime.AddSeconds(40);
			ViewBag.EndTime = endTime;

			var quiz = _quizService.Get(id);
			if (quiz.FilePath != string.Empty)
			{//pdf exam
			 //var pdfName = quiz.FileNameMaster;
			 //// string uploadedFolder = Path.Combine(hostingEnvironment.WebRootPath, "assets", "images", "Quiz");
			 //// string filePath = Path.Combine(uploadedFolder, pdfName);
			 //string pdfUrl = "/assets/images/Quiz/" + pdfName;

				//// Pass the pdfUrl to the view
				//ViewBag.PDF = pdfUrl;
				//ViewBag.Id = id;
			}
			return View(quiz);
		}


		[HttpPost]
		public async Task<IActionResult> Submit(int quizId, List<int> selectedAnswers)
		{
			int score = 0, maxScore = 0;
			//correcting exam
			string userId = GetUserId();
			var quiz = _quizService.Get(quizId);//including questions and options
			if (quiz.FilePath == string.Empty || quiz.FilePath == null)
			{
				score = _quizService.CalcMcq(quiz, userId, selectedAnswers);
				maxScore = quiz.Questions.Count;
			}
			//add in UserQuiz table
			var StudentQuiz = new StudentQuiz()
			{
				CourseId = quiz.courseId,
				ExamDegree = score,
				MaxDegree = maxScore,
				QuizId = quizId,
				StudentId = userId
			};
			await _studentQuizService.Add(StudentQuiz);

			return RedirectToAction("GetAll", quiz.courseId);
		}

		[HttpPost]
		public async Task<IActionResult> SubmitWritten(IFormFile File, int id)//QuizId (GetAll didn't get student when redirect)
		{//(GetAll didn't get student when redirect)
			string userId = GetUserId();
			var quiz = _quizService.Get(id);
			string path = await _mediaService.UploadImage(File, Constants.Quiz);//change to folder for students in quiz folder
			var StudentQuiz = new StudentQuiz()
			{
				CourseId = quiz.courseId,
				ExamDegree = 0,
				MaxDegree = 0,
				QuizId = quiz.Id,
				StudentId = userId,
				StudentAnswerPath = path
			};
			await _studentQuizService.Add(StudentQuiz);
			return RedirectToAction("GetAll", new { id = quiz.courseId });

		}

		[HttpGet]
		public async Task<IActionResult> CorrectWritten(int id)//CourseId
		{
			//StudentQuiz with Student
			var obj = _studentQuizService.GetAll(id);
			return View(obj);
		}
		//DisplayPdf
		[HttpGet]
		public IActionResult DisplayPdf(int id) //Student Quiz ID
		{
			var studentQuiz = _studentQuizService.GetById(id);
			return View(studentQuiz);
		}
		[HttpPost]
		public async Task<IActionResult> CorrectWritten(List<StudentQuiz> correctquiz)
		{
			//saving
			await _studentQuizService.CorrectingWritten(correctquiz);
			return View(correctquiz);
		}

		#endregion




		#region Private
		private string GetUserId()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var uId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
			if (uId == null) return string.Empty;
			return uId.Value;
		}
		#endregion



	}
}
