using LMS.Models.Entities;

namespace LMS.Service.Abstract
{
	public interface IQuizService
	{
		Task<string> Add(Quiz quiz);
		List<Quiz> GetAllByCourseId(int courseId);
		Quiz Get(int Id);
		int CalcMcq(Quiz quiz, string userId, List<int> selectedAnswers);

	}
}
