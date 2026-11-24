using LMS.Helper;
using LMS.Models.Entities;
using LMS.Models.Enums;
using LMS.Models.Settings;
using LMS.Service.Abstract;
using LMS.ViewModels.QuizVM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace LMS.Controllers
{
    [Authorize]
    public class QuizController : Controller
    {
        private const string MessageKey = "QuizMessage";

        #region Fields
        private readonly IQuizService _quizService;
        private readonly IStudentQuizService _studentQuizService;
        private readonly IMediaService _mediaService;
        private readonly ICourseService _courseService;
        private readonly ICourseStudentService _courseStudentService;
        private readonly QuizSettings _quizSettings;
        private readonly UploadSettings _uploadSettings;

        #endregion

        #region Ctor
        public QuizController(
            IQuizService quizService,
            IStudentQuizService studentQuizService,
            IMediaService mediaService,
            ICourseService courseService,
            ICourseStudentService courseStudentService,
            IOptions<QuizSettings> quizSettings,
            IOptions<UploadSettings> uploadSettings)
        {
            _quizService = quizService;
            _studentQuizService = studentQuizService;
            _mediaService = mediaService;
            _courseService = courseService;
            _courseStudentService = courseStudentService;
            _quizSettings = quizSettings.Value;
            _uploadSettings = uploadSettings.Value;
        }
        #endregion

        #region Actions



        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Add(int id)
        {
            ViewBag.id = id;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Quiz quiz)//make id = 0
        {
            //Server side gate: a quiz whose CorrectAnswer points outside its own options can never be
            //graded fairly, and the form fields can be bypassed.
            if (string.IsNullOrWhiteSpace(quiz.Name))
            {
                ModelState.AddModelError(nameof(quiz.Name), "Exam name is required.");
            }

            if (_courseService.GetById(quiz.courseId) == null)
            {
                ModelState.AddModelError(nameof(quiz.courseId), "Course not found.");
            }

            if (quiz.Questions == null || quiz.Questions.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "Add at least one question.");
            }
            else
            {
                for (int i = 0; i < quiz.Questions.Count; i++)
                {
                    var question = quiz.Questions.ElementAt(i);
                    var options = question.Options?.Count ?? 0;
                    if (string.IsNullOrWhiteSpace(question.Text))
                    {
                        ModelState.AddModelError($"Questions[{i}].Text", "Question text is required.");
                    }

                    if (options < 2)
                    {
                        ModelState.AddModelError($"Questions[{i}].Options", "Each question needs at least two options.");
                    }
                    else if (question.CorrectAnswer < 1 || question.CorrectAnswer > options)
                    {
                        ModelState.AddModelError($"Questions[{i}].CorrectAnswer", "The correct answer must be one of the options.");
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.id = quiz.courseId;
                return View(quiz);
            }

            quiz.Id = 0;
            quiz.DurationMinutes = DurationOf(quiz.DurationMinutes);
            await _quizService.Add(quiz);
            return RedirectToAction(nameof(GetAll), new { id = quiz.courseId });
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult AddWritten(int id)
        {
            var obj = new AddWritten()
            {
                CourseId = id
            };
            return View(obj);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddWritten(AddWritten obj)
        {
            if (!PdfFileValidator.TryValidate(obj.file, _uploadSettings.MaxPdfSizeBytes, out var error))
            {
                ModelState.AddModelError(nameof(obj.file), error);
                return View(obj);
            }

            //save pdf
            string path = await _mediaService.UploadImage(obj.file, Constants.Quiz);
            //then save quiz
            var quiz = new Quiz()
            {
                courseId = obj.CourseId,
                Description = obj.description,
                Name = obj.Name,
                FilePath = path,
                DurationMinutes = DurationOf(obj.DurationMinutes)
            };
            await _quizService.Add(quiz);
            return RedirectToAction(nameof(GetAll), new { id = obj.CourseId });
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetAll(int id)//CourseId
        {
            var course = _courseService.GetById(id);
            if (course == null)
            {
                return NotFound();
            }

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
                    Questions = item.Questions,
                    DurationMinutes = item.DurationMinutes
                };
                var submitted = _studentQuizService.GetSubmittedAttempt(userId, item.Id);
                temp.StudentTakedExam = submitted != null;
                if (temp.StudentTakedExam && CanSeeResult())
                {
                    temp.StudentDegree = submitted!.ExamDegree;
                    temp.StudentMaxDegree = submitted.MaxDegree;
                }

                ListOfQuizes.Add(temp);
            }

            ViewBag.CourseId = id;
            var students = _studentQuizService.GetStudents(id);
            obj.students = students;
            obj.quizzes = ListOfQuizes;
            return View(obj);
        }

        [HttpGet]//Quiz --
        [Authorize]
        public async Task<IActionResult> Get(int id) //quiz id
        {
            var quiz = _quizService.Get(id);
            if (quiz == null)
            {
                return NotFound();
            }

            if (!CanTakeExam(quiz.courseId))
            {
                return Forbid();
            }

            string userId = GetUserId();
            var submitted = _studentQuizService.GetSubmittedAttempt(userId, quiz.Id);
            if (submitted != null)
            {
                TempData[MessageKey] = "You have already submitted this exam.";
                return RedirectToAction(nameof(Result), new { id = quiz.Id });
            }

            var nowUtc = DateTime.UtcNow;
            var attempt = _studentQuizService.GetInProgressAttempt(userId, quiz.Id);
            if (attempt == null)
            {
                attempt = new StudentQuiz()
                {
                    CourseId = quiz.courseId,
                    QuizId = quiz.Id,
                    StudentId = userId,
                    ExamDegree = 0,
                    MaxDegree = 0,
                    Status = ExamAttemptStatus.InProgress,
                    StartedAtUtc = nowUtc
                };
                await _studentQuizService.Add(attempt);
            }

            var remainingSeconds = ExamClock.RemainingSeconds(StartedAt(attempt), DurationOf(quiz), nowUtc);
            if (remainingSeconds == 0)
            {
                // The clock ran out while the tab was closed, so the server closes the attempt instead of
                // leaving it open for an unlimited submission.
                return await CloseUnansweredAttempt(attempt, quiz, "Exam time is over, no answer was submitted.");
            }

            return View(new TakeExamVM
            {
                Quiz = quiz,
                AttemptId = attempt.Id,
                RemainingSeconds = remainingSeconds
            });
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(int quizId, List<int?>? selectedAnswers)
        {
            var quiz = _quizService.Get(quizId);//including questions and options
            if (quiz == null || IsWrittenQuiz(quiz))
            {
                return NotFound();
            }

            if (!CanTakeExam(quiz.courseId))
            {
                return Forbid();
            }

            string userId = GetUserId();
            var attempt = _studentQuizService.GetInProgressAttempt(userId, quizId);
            if (attempt == null)
            {
                TempData[MessageKey] = _studentQuizService.StudentTakeExam(userId, quizId)
                    ? "You have already submitted this exam."
                    : "Open the exam before submitting your answers.";
                return RedirectToAction(nameof(Result), new { id = quizId });
            }

            var nowUtc = DateTime.UtcNow;
            if (!ExamClock.IsWithinSubmissionWindow(StartedAt(attempt), DurationOf(quiz), nowUtc))
            {
                return await CloseUnansweredAttempt(attempt, quiz, "Exam time is over, the answers were not accepted.");
            }

            attempt.ExamDegree = _quizService.CalcMcq(quiz, userId, selectedAnswers);
            attempt.MaxDegree = quiz.Questions?.Count ?? 0;
            attempt.Status = ExamAttemptStatus.Submitted;
            attempt.SubmittedAtUtc = nowUtc;
            await _studentQuizService.Update(attempt);

            TempData[MessageKey] = "Your exam was submitted.";
            return RedirectToAction(nameof(Result), new { id = quizId });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitWritten(IFormFile? File, int id)//QuizId
        {
            var quiz = _quizService.Get(id);
            if (quiz == null || !IsWrittenQuiz(quiz))
            {
                return NotFound();
            }

            if (!CanTakeExam(quiz.courseId))
            {
                return Forbid();
            }

            string userId = GetUserId();
            if (_studentQuizService.StudentTakeExam(userId, quiz.Id))
            {
                TempData[MessageKey] = "You have already submitted this exam.";
                return RedirectToAction(nameof(Result), new { id = quiz.Id });
            }

            var attempt = _studentQuizService.GetInProgressAttempt(userId, quiz.Id);
            if (attempt == null)
            {
                TempData[MessageKey] = "Open the exam before submitting your answer.";
                return RedirectToAction(nameof(GetAll), new { id = quiz.courseId });
            }

            var nowUtc = DateTime.UtcNow;
            if (!ExamClock.IsWithinSubmissionWindow(StartedAt(attempt), DurationOf(quiz), nowUtc))
            {
                return await CloseUnansweredAttempt(attempt, quiz, "Exam time is over, the file was not accepted.");
            }

            if (!PdfFileValidator.TryValidate(File, _uploadSettings.MaxPdfSizeBytes, out var error))
            {
                TempData[MessageKey] = error;
                return RedirectToAction(nameof(Get), new { id = quiz.Id });
            }

            string path = await _mediaService.UploadImage(File!, Constants.Quiz);//change to folder for students in quiz folder
            attempt.StudentAnswerPath = path;
            attempt.Status = ExamAttemptStatus.Submitted;
            attempt.SubmittedAtUtc = nowUtc;
            await _studentQuizService.Update(attempt);

            TempData[MessageKey] = "Your answer was submitted and is waiting for correction.";
            return RedirectToAction(nameof(Result), new { id = quiz.Id });
        }

        [HttpGet]
        [Authorize]
        public IActionResult Result(int id)//quiz id
        {
            var quiz = _quizService.Get(id);
            if (quiz == null)
            {
                return NotFound();
            }

            var attempt = _studentQuizService.GetSubmittedAttempt(GetUserId(), id);
            if (attempt == null)
            {
                TempData[MessageKey] = "This exam was not submitted yet.";
                return RedirectToAction(nameof(GetAll), new { id = quiz.courseId });
            }

            if (!CanSeeResult())
            {
                TempData[MessageKey] = (string?)TempData[MessageKey] ?? "Your exam was submitted.";
                return RedirectToAction(nameof(GetAll), new { id = quiz.courseId });
            }

            return View(new ExamResultVM { Quiz = quiz, Attempt = attempt });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult CorrectWritten(int id)//CourseId
        {
            return View(BuildCorrectionVm(id));
        }
        //DisplayPdf
        [HttpGet]
        [Authorize]
        public IActionResult DisplayPdf(int id) //Student Quiz ID
        {
            var studentQuiz = _studentQuizService.GetById(id);
            if (studentQuiz == null)
            {
                return NotFound();
            }

            // A student may only ever reach their own uploaded answer.
            if (!IsStaff() && studentQuiz.StudentId != GetUserId())
            {
                return Forbid();
            }

            return View(studentQuiz);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CorrectWritten(int id, List<StudentQuiz> correctquiz)
        {
            //saving
            TempData[MessageKey] = await _studentQuizService.CorrectingWritten(id, correctquiz);
            return View(BuildCorrectionVm(id));
        }

        #endregion




        #region Private
        private string GetUserId()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var uId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
            if (uId == null) return string.Empty;
            return uId.Value;
        }

        private bool IsStaff()
        {
            return User.IsInRole("Admin") || User.IsInRole("Teacher");
        }

        private bool CanSeeResult()
        {
            return _quizSettings.ShowResultToStudent || IsStaff();
        }

        /// <summary>
        /// A student reaches an exam through the course they are enrolled in only; Admin and Teacher keep access.
        /// </summary>
        private bool CanTakeExam(int courseId)
        {
            if (IsStaff())
            {
                return true;
            }

            var userId = GetUserId();
            return userId != string.Empty && _courseStudentService.StudentInCourse(courseId, userId);
        }

        private static bool IsWrittenQuiz(Quiz quiz)
        {
            return !string.IsNullOrEmpty(quiz.FilePath);
        }

        private static DateTime StartedAt(StudentQuiz attempt)
        {
            return ExamClock.AsUtc(attempt.StartedAtUtc ?? DateTime.UtcNow);
        }

        private int DurationOf(Quiz quiz)
        {
            return DurationOf(quiz.DurationMinutes);
        }

        private int DurationOf(int durationMinutes)
        {
            return durationMinutes > 0 ? durationMinutes : _quizSettings.DefaultDurationMinutes;
        }

        private async Task<IActionResult> CloseUnansweredAttempt(StudentQuiz attempt, Quiz quiz, string message)
        {
            attempt.Status = ExamAttemptStatus.Submitted;
            attempt.SubmittedAtUtc = DateTime.UtcNow;
            if (!IsWrittenQuiz(quiz))
            {
                attempt.MaxDegree = quiz.Questions?.Count ?? 0;
            }

            await _studentQuizService.Update(attempt);

            TempData[MessageKey] = message;
            return RedirectToAction(nameof(Result), new { id = quiz.Id });
        }

        private CorrectWrittenVM BuildCorrectionVm(int courseId)
        {
            return new CorrectWrittenVM
            {
                CourseId = courseId,
                Attempts = _studentQuizService.GetWrittenByCourse(courseId)
                    .Select(x => new GradeableAttempt
                    {
                        Id = x.Id,
                        StudentName = x.Student?.Name ?? string.Empty,
                        QuizName = x.Quiz?.Name ?? string.Empty,
                        StudentAnswerPath = x.StudentAnswerPath,
                        ExamDegree = x.ExamDegree,
                        MaxDegree = x.MaxDegree
                    })
                    .ToList()
            };
        }
        #endregion



    }
}
