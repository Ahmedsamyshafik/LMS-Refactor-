using LMS.Models.Entities;
using LMS.Repository.EntityRepos.Repo;
using LMS.Service.Implementation;
using Xunit;

namespace LMS.Tests
{
    public class QuizServiceTests
    {
        private static Quiz BuildQuiz(int id, int courseId, params int[] correctAnswers)
        {
            var quiz = new Quiz { Id = id, Name = "quiz " + id, Description = "quiz " + id, courseId = courseId, DurationMinutes = 60 };
            for (int q = 0; q < correctAnswers.Length; q++)
            {
                var question = new Question { Id = q + 1, Text = "q" + q, CorrectAnswer = correctAnswers[q], QuizId = id };
                for (int o = 0; o < 4; o++)
                {
                    question.Options.Add(new Option { Id = q * 10 + o + 1, Text = "option " + o, QuestionId = question.Id });
                }

                quiz.Questions.Add(question);
            }

            return quiz;
        }

        private static QuizService ServiceWith(params Quiz[] quizzes)
        {
            var db = TestFactory.SeededContext("quizzes", new[] { 7, 9 }, Array.Empty<string>());
            db.Quizzes.AddRange(quizzes);
            db.SaveChanges();

            foreach (var entry in db.ChangeTracker.Entries().ToList())
            {
                entry.State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            }

            return new QuizService(new QuizRepo(db));
        }

        [Fact]
        public void GetAllByCourseId_ReturnsOnlyThatCoursesQuizzes()
        {
            var service = ServiceWith(
                BuildQuiz(1, courseId: 7),
                BuildQuiz(2, courseId: 7),
                BuildQuiz(3, courseId: 9));

            var result = service.GetAllByCourseId(7);

            Assert.Equal(new[] { 1, 2 }, result.Select(x => x.Id));
        }

        [Fact]
        public void Get_UnknownId_ReturnsNullInsteadOfThrowing()
        {
            var service = ServiceWith(BuildQuiz(1, courseId: 7));

            Assert.Null(service.Get(999));
        }

        [Fact]
        public void CalcMcq_NoAnswers_ScoresZeroWithoutThrowing()
        {
            var service = ServiceWith();
            var quiz = BuildQuiz(1, 7, 1, 2);

            Assert.Equal(0, service.CalcMcq(quiz, "user", null));
            Assert.Equal(0, service.CalcMcq(quiz, "user", new List<int?>()));
            Assert.Equal(0, service.CalcMcq(quiz, "user", new List<int?> { null, null }));
        }

        [Fact]
        public void CalcMcq_BlankAnswersMixedWithRealOnes_ScoreOnlyAnswered()
        {
            var service = ServiceWith();
            // Correct options are 1st and 3rd (stored 1 based).
            var quiz = BuildQuiz(1, 7, 1, 3, 2);

            var answers = new List<int?> { 0, null, 1 };

            Assert.Equal(2, service.CalcMcq(quiz, "user", answers));
        }

        [Fact]
        public void CalcMcq_ShorterAnswerList_DoesNotRunOffTheEnd()
        {
            var service = ServiceWith();
            var quiz = BuildQuiz(1, 7, 1, 1);

            Assert.Equal(1, service.CalcMcq(quiz, "user", new List<int?> { 0 }));
        }
    }
}
