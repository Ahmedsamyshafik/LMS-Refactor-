using LMS.Controllers;
using LMS.Models.Entities;
using LMS.Models.Settings;
using LMS.Service.Abstract;
using LMS.ViewModels.Material;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using System.Reflection;
using Xunit;

namespace LMS.Tests
{
    /// <summary>
    /// Locks the role matrix that the audit asked for: administration screens for Admin, upload screens for
    /// staff only, written submission open to the enrolled learner.
    /// </summary>
    public class AuthorizationMatrixTests
    {
        private static T? Attribute<T>(Type controller, string action, string? roles = null) where T : Attribute
        {
            var methods = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.Name.Equals(action, StringComparison.OrdinalIgnoreCase));

            return methods
                .Where(x => roles == null || ((AuthorizeAttribute?)x.GetCustomAttribute<AuthorizeAttribute>())?.Roles == roles)
                .Select(x => x.GetCustomAttribute<T>())
                .FirstOrDefault(x => x != null);
        }

        [Fact]
        public void CourseRequestListIsAdminOnly()
        {
            var authorize = Attribute<AuthorizeAttribute>(typeof(CourseRequestController), "GetAll");

            Assert.NotNull(authorize);
            Assert.Equal("Admin", authorize!.Roles);
        }

        [Theory]
        [InlineData("Accept")]
        [InlineData("Delete")]
        public void CourseRequestStateChangingActionsAreAdminPosts(string action)
        {
            var authorize = Attribute<AuthorizeAttribute>(typeof(CourseRequestController), action);

            Assert.Equal("Admin", authorize?.Roles);
            Assert.NotNull(Attribute<HttpPostAttribute>(typeof(CourseRequestController), action));
            Assert.NotNull(Attribute<ValidateAntiForgeryTokenAttribute>(typeof(CourseRequestController), action));
            Assert.Null(Attribute<HttpGetAttribute>(typeof(CourseRequestController), action));
        }

        [Theory]
        [InlineData("index")]
        [InlineData("Add")]
        public void MaterialUploadScreensAreStaffOnly(string action)
        {
            var authorize = Attribute<AuthorizeAttribute>(typeof(MaterialController), action, "Admin,Teacher");

            Assert.NotNull(authorize);
        }

        [Fact]
        public void WrittenCorrectionStaysWithAdmin()
        {
            var authorize = Attribute<AuthorizeAttribute>(typeof(QuizController), "CorrectWritten", "Admin");

            Assert.NotNull(authorize);
        }

        [Fact]
        public void AdminRoleCannotSubmitOnBehalfOfAStudent()
        {
            // Submitting writes onto the attempt of the calling user, so the endpoint must stay open to the
            // enrolled learner (with an enrolment check) instead of being an Admin action.
            var submitWritten = typeof(QuizController).GetMethods()
                .Single(x => x.Name == nameof(QuizController.SubmitWritten));

            var authorize = submitWritten.GetCustomAttribute<AuthorizeAttribute>();

            Assert.NotNull(authorize);
            Assert.True(string.IsNullOrEmpty(authorize!.Roles));
        }
    }

    public class MaterialControllerTests
    {
        private const int CourseId = 7;
        private const string StudentId = "student-a";

        private readonly Mock<IMaterialService> _materialService = new();
        private readonly Mock<ICourseService> _courseService = new();
        private readonly Mock<ICourseStudentService> _courseStudentService = new();

        private MaterialController Build(string userId = StudentId, params string[] roles)
        {
            _courseService.Setup(x => x.GetById(CourseId))
                .Returns(new LMS.ViewModels.Course.GetCourseVM { Id = CourseId, Name = "maths", Price = 200 });

            var controller = new MaterialController(
                _materialService.Object,
                _courseService.Object,
                _courseStudentService.Object,
                Options.Create(new UploadSettings { MaxPdfSizeMb = 5 }));

            controller.AttachUser(userId, roles);
            return controller;
        }

        [Fact]
        public void UnknownCourse_Answers404()
        {
            _courseService.Setup(x => x.GetById(999)).Returns((LMS.ViewModels.Course.GetCourseVM?)null);

            Assert.IsType<NotFoundResult>(Build("admin", "Admin").index(999));
        }

        [Fact]
        public void LearnerOutsideTheCourse_CannotReadMaterials()
        {
            _courseStudentService.Setup(x => x.StudentInCourse(CourseId, StudentId)).Returns(false);

            Assert.IsType<ForbidResult>(Build().GetAll(CourseId));
        }

        [Fact]
        public void EnrolledLearner_ReadsMaterials()
        {
            _courseStudentService.Setup(x => x.StudentInCourse(CourseId, StudentId)).Returns(true);
            _materialService.Setup(x => x.GetAllMaterialByCourseId(CourseId)).Returns(new List<Material>());

            Assert.IsType<ViewResult>(Build("student-a", "Student").GetAll(CourseId));
        }

        [Fact]
        public void UnknownMaterial_Answers404()
        {
            _materialService.Setup(x => x.GetById(999)).Returns((Material?)null);

            Assert.IsType<NotFoundResult>(Build("admin", "Admin").DisplayMaterial(999));
        }
    }
}
