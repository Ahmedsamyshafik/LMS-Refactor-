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

			var quizzes2 = _quizRepo.GetTableNoTracking()
				.Include(q => q.StudentQuizzes)
				.ToList();

			var filteredQuizzes = quizzes2
				.Where(q => q.StudentQuizzes.Any(sq => sq.CourseId == courseId))
				.ToList();
			var quizzes = _quizRepo.GetTableNoTracking()
				.Include(q => q.StudentQuizzes)
				//  .Where(q => q.StudentQuizzes.Any(sq => sq.CourseId == courseId))
				.ToList();

			return quizzes;
		}

		public Quiz Get(int Id)
		{
			return _quizRepo.GetTableNoTracking().Where(x => x.Id == Id).Include(x => x.Questions).ThenInclude(x => x.Options).FirstOrDefault();

		}

		public int CalcMcq(Quiz quiz, string userId, List<int> selectedAnswers)
		{
			int score = 0;
			for (int i = 0; i < quiz.Questions.Count; i++)
			{

				if (selectedAnswers[i] == (quiz.Questions.ToList()[i].CorrectAnswer - 1))
				{
					score++;
				}
			}
			return score;

			// why i do all this now?


			//var x = quiz.Id;
			//all quiz question
			////	List<Question> Question = quizServes.GetAllQuestions(x);
			//	var Question = _quizRepo.GetTableNoTracking().Where(x => x.Id == quiz.Id).Include(x=>x.Questions).ThenInclude(x=>x.Options).ToList();
			//	int Count = Question.Count; //VB
			//	var Q = Question;		//vb
			//	var U = userId; //vb
			//	var O = quizServes.GetAllOptions();
			//	ViewBag.Score = score;
			//	quizServes.AddingUE(CurrentUser.Id, quiz.Id, CurrentUser.UserName, quiz.Title, 0, score, quiz.Questions.Count, quiz.Semester);
			//	quizServes.Save();
			//	var Answers = new List<string>();
			//	//Getting Correct Answer
			//	foreach (var Q in Question)
			//	{
			//		var alloption = new List<string>();
			//		var k = @Q.Text; // Question
			//		var Delete = @Q.CorrectAnswer;
			//		var y = @Q.CorrectAnswer - 1; // Number of Soluation?
			//		for (var option = 0; option < O.Count; option++)
			//		{
			//			if (O[option].QuestionId == Q.Id) // Options To Question
			//			{
			//				alloption.Add(O[option].Text);
			//			}
			//			else
			//			{
			//				continue;
			//			}
			//		}
			//		for (var ans = 0; ans < alloption.Count; ans++)
			//		{
			//			if (ans == @Q.CorrectAnswer - 1)
			//			{
			//				Answers.Add(alloption[ans]);
			//			}
			//		}
			//	}

		}

		#endregion



	}
}
