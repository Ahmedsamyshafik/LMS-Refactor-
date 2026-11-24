using LMS.Models.Entities;
using LMS.Repository.EntityRepos.IRepo;
using LMS.Service.Abstract;
using Microsoft.EntityFrameworkCore;

namespace LMS.Service.Implementation
{
	public class QuizService : IQuizService
	{
		#region Fields
		private readonly IQuizRepo _quizRepo;
		#endregion

		#region Ctor
		public QuizService(IQuizRepo quizRepo)
		{
			_quizRepo = quizRepo;
		}
		#endregion

		#region Handle Functions
		public async Task<string> Add(Quiz quiz)
		{
			await _quizRepo.AddAsync(quiz);
			return "";
		}

		public List<Quiz> GetAllByCourseId(int courseId)
		{
			return _quizRepo.GetTableNoTracking()
				.Include(q => q.StudentQuizzes)
				.Where(q => q.courseId == courseId)
				.OrderBy(q => q.Id)
				.ToList();
		}

		public Quiz? Get(int Id)
		{
			var quiz = _quizRepo.GetTableNoTracking()
				.Where(x => x.Id == Id)
				.Include(x => x.Questions).ThenInclude(x => x.Options)
				.FirstOrDefault();

			if (quiz == null)
			{
				return null;
			}

			// The answer form posts selectedAnswers by position, so questions and options have to come
			// back in the same order the view renders them.
			foreach (var question in quiz.Questions)
			{
				question.Options = question.Options.OrderBy(x => x.Id).ToList();
			}
			quiz.Questions = quiz.Questions.OrderBy(x => x.Id).ToList();

			return quiz;
		}

		/// <summary>
		/// Positions the student left unanswered stay null and score nothing, instead of being read as option zero.
		/// </summary>
		public int CalcMcq(Quiz quiz, string userId, List<int?>? selectedAnswers)
		{
			if (quiz?.Questions == null || quiz.Questions.Count == 0 || selectedAnswers == null)
			{
				return 0;
			}

			int score = 0;
			var questions = quiz.Questions.ToList();
			for (int i = 0; i < questions.Count && i < selectedAnswers.Count; i++)
			{
				var answer = selectedAnswers[i];
				if (answer.HasValue && answer.Value >= 0 && answer.Value == questions[i].CorrectAnswer - 1)
				{
					score++;
				}
			}

			return score;
		}
		#endregion



	}
}
