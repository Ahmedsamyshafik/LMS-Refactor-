using LMS.Models.Data;
using LMS.Models.Entities;
using LMS.Models.Enums;
using LMS.Repository.EntityRepos.Repo;
using LMS.Service.Implementation;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LMS.Tests
{
    /// <summary>
    /// The written correction screen used to post whole entities and land the degree on whichever row the
    /// provider returned first. These tests pin the new contract: only submitted written attempts of the
    /// routed course can be graded, and nothing else moves.
    /// </summary>
    public class WrittenCorrectionTests
    {
        private static Quiz Essay(int id, int courseId) => new()
        {
            Id = id,
            Name = "essay " + id,
            Description = "essay " + id,
            courseId = courseId,
            FilePath = $"assets/images/Quiz/essay{id}.pdf",
            DurationMinutes = 60
        };

        private static Quiz Mcq(int id, int courseId) => new()
        {
            Id = id,
            Name = "mcq " + id,
            Description = "mcq " + id,
            courseId = courseId,
            DurationMinutes = 60
        };

        private static StudentQuiz Attempt(int id, int quizId, int courseId, string studentId,
            ExamAttemptStatus status, int degree, int maxDegree) => new()
        {
            Id = id,
            QuizId = quizId,
            CourseId = courseId,
            StudentId = studentId,
            Status = status,
            ExamDegree = degree,
            MaxDegree = maxDegree,
            StudentAnswerPath = $"assets/images/Quiz/answer{id}.pdf"
        };

        private static (AppDbContext Db, StudentQuizService Service) Seed()
        {
            var db = TestFactory.SeededContext("correction", new[] { 5, 6 }, new[] { "student-a", "student-b", "student-c", "student-d" });
            db.Quizzes.AddRange(Essay(20, 5), Mcq(10, 5), Essay(21, 6));
            db.StudentQuizzes.AddRange(
                Attempt(1, 20, 5, "student-a", ExamAttemptStatus.Submitted, 0, 0),
                Attempt(2, 10, 5, "student-a", ExamAttemptStatus.Submitted, 4, 5),
                Attempt(3, 21, 6, "student-b", ExamAttemptStatus.Submitted, 2, 10));
            db.SaveChanges();

            foreach (var entry in db.ChangeTracker.Entries().ToList())
            {
                entry.State = EntityState.Detached;
            }

            return (db, new StudentQuizService(new StudentQuizRepo(db)));
        }

        private static StudentQuiz Posted(int id, int degree, int maxDegree) => new()
        {
            Id = id,
            ExamDegree = degree,
            MaxDegree = maxDegree
        };

        [Fact]
        public async Task GradingEssay_WritesOnlyTheWrittenAttempt()
        {
            var (db, service) = Seed();

            var message = await service.CorrectingWritten(5, new List<StudentQuiz>
            {
                Posted(1, 18, 20),
                Posted(2, 99, 99)
            });

            Assert.Contains("1 written attempt", message);

            var essay = await db.StudentQuizzes.FirstAsync(x => x.Id == 1);
            var mcq = await db.StudentQuizzes.FirstAsync(x => x.Id == 2);
            var otherCourse = await db.StudentQuizzes.FirstAsync(x => x.Id == 3);

            Assert.Equal(18, essay.ExamDegree);
            Assert.Equal(20, essay.MaxDegree);

            // the mcq attempt of the same student and the essay of another course stay untouched
            Assert.Equal(4, mcq.ExamDegree);
            Assert.Equal(5, mcq.MaxDegree);
            Assert.Equal(2, otherCourse.ExamDegree);
        }

        [Fact]
        public async Task Grading_CourseOfTheRouteWinsOverThePostedId()
        {
            var (db, service) = Seed();

            // attempt 3 belongs to course 6 while the screen is routed for course 5
            await service.CorrectingWritten(5, new List<StudentQuiz> { Posted(3, 9, 10) });

            var otherCourse = await db.StudentQuizzes.FirstAsync(x => x.Id == 3);
            Assert.Equal(2, otherCourse.ExamDegree);
        }

        [Fact]
        public async Task Grading_ImpossibleDegree_IsSkipped()
        {
            var (db, service) = Seed();

            var message = await service.CorrectingWritten(5, new List<StudentQuiz> { Posted(1, 40, 20) });

            var essay = await db.StudentQuizzes.FirstAsync(x => x.Id == 1);
            Assert.Equal(0, essay.ExamDegree);
            Assert.Contains("No written attempt matched", message);
        }

        [Fact]
        public void ListingForCorrection_SkipsMcqAndUnsubmittedAttempts()
        {
            var (db, service) = Seed();

            db.StudentQuizzes.Add(Attempt(4, 20, 5, "student-c", ExamAttemptStatus.InProgress, 0, 0));
            db.SaveChanges();
            foreach (var entry in db.ChangeTracker.Entries().ToList())
            {
                entry.State = EntityState.Detached;
            }

            var rows = service.GetWrittenByCourse(5);

            Assert.Equal(new[] { 1 }, rows.Select(x => x.Id));
        }

        [Fact]
        public void AnAttemptInProgress_StillCountsAsNotTaken()
        {
            var (db, service) = Seed();

            db.StudentQuizzes.Add(Attempt(5, 10, 5, "student-d", ExamAttemptStatus.InProgress, 0, 0));
            db.SaveChanges();

            Assert.False(service.StudentTakeExam("student-d", 10));
            Assert.NotNull(service.GetInProgressAttempt("student-d", 10));

            Assert.True(service.StudentTakeExam("student-a", 10));
            Assert.Null(service.GetInProgressAttempt("student-a", 10));
        }
    }
}
