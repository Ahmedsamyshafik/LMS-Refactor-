using LMS.Models.Entities;

namespace LMS.Service.Abstract
{
	public interface IStudentQuizService
	{
		Task<string> Add(StudentQuiz studentQuiz);
		bool StudentTakeExam(string userId, int quizId);
		List<StudentQuiz> GetStudents(int courseId);
		List<StudentQuiz> GetAll(int courseId);
		StudentQuiz GetById(int id);
		Task<string> CorrectingWritten(List<StudentQuiz> studentQuizs);


	}
}
