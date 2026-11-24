using LMS.Models.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;

namespace LMS.Tests
{
    internal static class TestFactory
    {
        public static AppDbContext Context(string name = "tests")
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(name + Guid.NewGuid())
                .Options;

            return new AppDbContext(options);
        }

        public static AppDbContext SeededContext(string name, IEnumerable<int> courseIds, IEnumerable<string> userIds)
        {
            var db = Context(name);
            foreach (var courseId in courseIds)
            {
                db.Courses.Add(new LMS.Models.Entities.Course { Id = courseId, Name = "course " + courseId, Price = 100 });
            }

            foreach (var userId in userIds)
            {
                db.Users.Add(new LMS.Models.Entities.ApplicationUser { Id = userId, Name = userId });
            }

            db.SaveChanges();
            return db;
        }

        public static IFormFile PdfFile(string fileName = "answer.pdf", string content = "%PDF-1.4 fake body", string contentType = "application/pdf")
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(bytes);
            return new FormFile(stream, 0, bytes.Length, "File", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };
        }

        /// <summary>
        /// Gives a controller an authenticated user with the given name identifier and roles, plus a working
        /// TempData so that redirect based assertions can read the messages.
        /// </summary>
        public static void AttachUser(this Controller controller, string userId, params string[] roles)
        {
            var identity = new ClaimsIdentity(authenticationType: "TestAuth");
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId));
            foreach (var role in roles)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            var httpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) };
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        }
    }
}
