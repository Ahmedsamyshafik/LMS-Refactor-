using LMS.Controllers;
using LMS.Helper;
using LMS.Models.Entities;
using LMS.Models.Enums;
using LMS.Models.Settings;
using LMS.Service.Abstract;
using LMS.ViewModels.QuizVM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using System.Reflection;
using Xunit;

namespace LMS.Tests
{
    public class QuizControllerTests
    {
        private const string StudentId = "student-a";
        private const int CourseId = 7;
        private const int QuizId = 11;

        private readonly Mock<IQuizService> _quizService = new();
        private readonly Mock<IStudentQuizService> _studentQuizService = new();
        private readonly Mock<IMediaService> _mediaService = new();
        private readonly Mock<ICourseService> _courseService = new();
        private readonly Mock<ICourseStudentService> _courseStudentService = new();
        private readonly QuizSettings _quizSettings = new() { ShowResultToStudent = true, DefaultDurationMinutes = 60 };

        public QuizControllerTests()
        {
            _courseService.Setup(x => x.GetById(CourseId)).Returns(new LMS.ViewModels.Course.GetCourseVM { Id = CourseId, Name = "maths", Price = 200 });
        }

        private QuizController Build(string userId = StudentId, params string[] roles)
        {
            var controller = new QuizController(
                _quizService.Object,
                _studentQuizService.Object,
                _mediaService.Object,
                _courseService.Object,
                _courseStudentService.Object,
                Options.Create(_quizSettings),
                Options.Create(new UploadSettings { MaxPdfSizeMb = 5 }));

            controller.AttachUser(userId, roles);
            return controller;
        }

        private static Quiz McqQuiz() => new()
        {
            Id = QuizId,
            Name = "midterm",
            courseId = CourseId,
            DurationMinutes = 60,
            Questions = new List<Question>
            {
                new() { Id = 1, Text = "q1", CorrectAnswer = 1, Options = new List<Option> { new() { Id = 1 }, new() { Id = 2 } } }
            }
        };

        private static Quiz EssayQuiz() => new()
        {
            Id = QuizId,
            Name = "written",
            courseId = CourseId,
            FilePath = "assets/images/Quiz/written.pdf",
            DurationMinutes = 60
        };

        private static StudentQuiz Attempt(ExamAttemptStatus status, DateTime? startedUtc = null) => new()
        {
            Id = 500,
            QuizId = QuizId,
            CourseId = CourseId,
            StudentId = StudentId,
            Status = status,
            StartedAtUtc = startedUtc ?? DateTime.UtcNow
        };

        [Fact]
        public async Task UnknownQuizId_Answers404()
        {
            _quizService.Setup(x => x.Get(999)).Returns((Quiz?)null);

            var result = await Build().Get(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task StudentOutsideTheCourse_IsRefused()
        {
            _quizService.Setup(x => x.Get(QuizId)).Returns(McqQuiz());
            _courseStudentService.Setup(x => x.StudentInCourse(CourseId, StudentId)).Returns(false);

            var result = await Build().Get(QuizId);

            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task AlreadySubmittedExam_CannotBeOpenedAgain()
        {
            _quizService.Setup(x => x.Get(QuizId)).Returns(McqQuiz());
            _courseStudentService.Setup(x => x.StudentInCourse(CourseId, StudentId)).Returns(true);
            _studentQuizService.Setup(x => x.GetSubmittedAttempt(StudentId, QuizId)).Returns(Attempt(ExamAttemptStatus.Submitted));

            var controller = Build();
            var result = await controller.Get(QuizId);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(QuizController.Result), redirect.ActionName);
            Assert.Equal("You have already submitted this exam.", controller.TempData["QuizMessage"]);
            _studentQuizService.Verify(x => x.Add(It.IsAny<StudentQuiz>()), Times.Never);
        }

        [Fact]
        public async Task FreshAttempt_GetsTheSecondsLeftFromTheServer()
        {
            _quizService.Setup(x => x.Get(QuizId)).Returns(McqQuiz());
            _courseStudentService.Setup(x => x.StudentInCourse(CourseId, StudentId)).Returns(true);
            _studentQuizService.Setup(x => x.GetInProgressAttempt(StudentId, QuizId)).Returns((StudentQuiz?)null);

            var result = Assert.IsType<ViewResult>(await Build().Get(QuizId));
            var model = Assert.IsType<TakeExamVM>(result.Model);

            Assert.Equal(3600, model.RemainingSeconds);
            Assert.Equal("01:00:00", model.CountdownText);
            _studentQuizService.Verify(x => x.Add(It.Is<StudentQuiz>(a => a.Status == ExamAttemptStatus.InProgress && a.StartedAtUtc != null)), Times.Once);
        }

        [Fact]
        public async Task AttemptPastTheServerClock_IsClosedInsteadOfStayingOpen()
        {
            _quizService.Setup(x => x.Get(QuizId)).Returns(McqQuiz());
            _courseStudentService.Setup(x => x.StudentInCourse(CourseId, StudentId)).Returns(true);
            _studentQuizService.Setup(x => x.GetInProgressAttempt(StudentId, QuizId))
                .Returns(Attempt(ExamAttemptStatus.InProgress, DateTime.UtcNow.AddHours(-3)));

            var result = await Build().Get(QuizId);

            Assert.IsType<RedirectToActionResult>(result);
            _studentQuizService.Verify(x => x.Update(It.Is<StudentQuiz>(a => a.Status == ExamAttemptStatus.Submitted)), Times.Once);
        }

        [Fact]
        public async Task Submit_WithNoAnswerAtAll_IsGradedZeroAndNotCrashed()
        {
            _quizService.Setup(x => x.Get(QuizId)).Returns(McqQuiz());
            _courseStudentService.Setup(x => x.StudentInCourse(CourseId, StudentId)).Returns(true);
            _studentQuizService.Setup(x => x.GetInProgressAttempt(StudentId, QuizId)).Returns(Attempt(ExamAttemptStatus.InProgress));
            _quizService.Setup(x => x.CalcMcq(It.IsAny<Quiz>(), It.IsAny<string>(), It.IsAny<List<int?>?>())).Returns(0);

            var result = await Build().Submit(QuizId, null);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(QuizController.Result), redirect.ActionName);
            _studentQuizService.Verify(x => x.Update(It.Is<StudentQuiz>(a =>
                a.Status == ExamAttemptStatus.Submitted && a.SubmittedAtUtc != null)), Times.Once);
        }

        [Fact]
        public async Task Submit_AfterTheGracePeriod_AcceptsNothingAndGradesNothing()
        {
            var attempt = Attempt(ExamAttemptStatus.InProgress, DateTime.UtcNow.AddMinutes(-61 - ExamClock.SubmissionGraceSeconds / 60 - 1));
            _quizService.Setup(x => x.Get(QuizId)).Returns(McqQuiz());
            _courseStudentService.Setup(x => x.StudentInCourse(CourseId, StudentId)).Returns(true);
            _studentQuizService.Setup(x => x.GetInProgressAttempt(StudentId, QuizId)).Returns(attempt);

            var controller = Build();
            var result = await controller.Submit(QuizId, new List<int?> { 0 });

            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Exam time is over, the answers were not accepted.", controller.TempData["QuizMessage"]);
            _quizService.Verify(x => x.CalcMcq(It.IsAny<Quiz>(), It.IsAny<string>(), It.IsAny<List<int?>?>()), Times.Never);
        }

        [Fact]
        public async Task SecondSubmit_IsRefusedAndKeepsTheFirstGrade()
        {
            _quizService.Setup(x => x.Get(QuizId)).Returns(McqQuiz());
            _courseStudentService.Setup(x => x.StudentInCourse(CourseId, StudentId)).Returns(true);
            // the first submit already stored the attempt, so no attempt is in progress any more
            _studentQuizService.Setup(x => x.GetInProgressAttempt(StudentId, QuizId)).Returns((StudentQuiz?)null);
            _studentQuizService.Setup(x => x.StudentTakeExam(StudentId, QuizId)).Returns(true);

            var controller = Build();
            var result = await controller.Submit(QuizId, new List<int?> { 0 });

            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("You have already submitted this exam.", controller.TempData["QuizMessage"]);
            _studentQuizService.Verify(x => x.Update(It.IsAny<StudentQuiz>()), Times.Never);
        }

        [Fact]
        public async Task WrittenExam_StudentUploadReachesCorrectionQueue()
        {
            var attempt = Attempt(ExamAttemptStatus.InProgress);
            _quizService.Setup(x => x.Get(QuizId)).Returns(EssayQuiz());
            _courseStudentService.Setup(x => x.StudentInCourse(CourseId, StudentId)).Returns(true);
            _studentQuizService.Setup(x => x.StudentTakeExam(StudentId, QuizId)).Returns(false);
            _studentQuizService.Setup(x => x.GetInProgressAttempt(StudentId, QuizId)).Returns(attempt);
            _mediaService.Setup(x => x.UploadImage(It.IsAny<IFormFile>(), It.IsAny<string>()))
                .ReturnsAsync("assets/images/Quiz/answer_1.pdf");

            var result = await Build(StudentId, "Student").SubmitWritten(TestFactory.PdfFile(), QuizId);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(QuizController.Result), redirect.ActionName);
            Assert.Equal("assets/images/Quiz/answer_1.pdf", attempt.StudentAnswerPath);
            Assert.Equal(ExamAttemptStatus.Submitted, attempt.Status);
        }

        [Fact]
        public async Task WrittenExam_RejectsAFakePdf()
        {
            _quizService.Setup(x => x.Get(QuizId)).Returns(EssayQuiz());
            _courseStudentService.Setup(x => x.StudentInCourse(CourseId, StudentId)).Returns(true);
            _studentQuizService.Setup(x => x.GetInProgressAttempt(StudentId, QuizId)).Returns(Attempt(ExamAttemptStatus.InProgress));

            var controller = Build(StudentId, "Student");
            await controller.SubmitWritten(TestFactory.PdfFile("payload.pdf", "<script>bad</script>"), QuizId);

            Assert.Equal("The file content is not a valid PDF.", controller.TempData["QuizMessage"]);
            _mediaService.Verify(x => x.UploadImage(It.IsAny<IFormFile>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task McqExam_CannotBeSubmittedThroughTheWrittenEndpoint()
        {
            _quizService.Setup(x => x.Get(QuizId)).Returns(McqQuiz());

            var result = await Build(StudentId, "Student").SubmitWritten(TestFactory.PdfFile(), QuizId);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void WrittenSubmissionIsNotLockedToAdminAnymore()
        {
            var method = typeof(QuizController).GetMethods()
                .Single(x => x.Name == nameof(QuizController.SubmitWritten));

            var authorize = method.GetCustomAttribute<AuthorizeAttribute>();

            Assert.NotNull(authorize);
            Assert.True(string.IsNullOrEmpty(authorize!.Roles), "students must be able to submit their written exam");
            Assert.NotNull(method.GetCustomAttribute<ValidateAntiForgeryTokenAttribute>());
            Assert.NotNull(method.GetCustomAttribute<HttpPostAttribute>());
        }

        [Fact]
        public void EveryStateChangingQuizActionNeedsAPostAndAToken()
        {
            foreach (var name in new[] { "Submit", "SubmitWritten", "CorrectWritten", "Add", "AddWritten" })
            {
                var method = typeof(QuizController).GetMethods()
                    .Single(x => x.Name == name && x.GetCustomAttribute<HttpPostAttribute>() != null);

                Assert.NotNull(method.GetCustomAttribute<ValidateAntiForgeryTokenAttribute>());
                Assert.NotNull(method.GetCustomAttribute<AuthorizeAttribute>());
            }
        }
    }
}
