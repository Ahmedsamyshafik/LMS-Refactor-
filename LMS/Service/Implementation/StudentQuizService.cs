using LMS.Models.Entities;
using LMS.Repository.EntityRepos.IRepo;
using LMS.Service.Abstract;
using Microsoft.EntityFrameworkCore;

namespace LMS.Service.Implementation
{
	public class StudentQuizService : IStudentQuizService
	{
		#region Fields
		private readonly IStudentQuizRepo _studentQuizRepo;
		#endregion

		#region Ctor
		public StudentQuizService(IStudentQuizRepo studentQuizRepo)
		{
			_studentQuizRepo = studentQuizRepo;
		}
		#endregion

		#region Handle Functions
		public async Task<string> Add(StudentQuiz studentQuiz)
		{
			await _studentQuizRepo.AddAsync(studentQuiz);
			return "";
		}
		public bool StudentTakeExam(string userId, int quizId)
		{
			var obj = _studentQuizRepo.GetTableNoTracking().Where(x => x.StudentId == userId && x.QuizId == quizId).FirstOrDefault();
			if (obj != null)
			{
				return true;
			}
			return false;
		}

		public List<StudentQuiz> GetStudents(int courseId)
		{
			var lst = _studentQuizRepo.GetTableNoTracking().Where(x => x.CourseId == courseId)
				.Include(x => x.Student)
				.Include(x => x.Quiz)
				.ToList();
			return lst;
		}

		public List<StudentQuiz> GetAll(int courseId)
		{
			return _studentQuizRepo.GetTableNoTracking().Where(x => x.CourseId == courseId).Include(x => x.Student).Include(x => x.Quiz).ToList();
		}

		public StudentQuiz GetById(int id)
		{
			return _studentQuizRepo.GetTableNoTracking().Where(x => x.Id == id).FirstOrDefault();
		}

		public async Task<string> CorrectingWritten(List<StudentQuiz> studentQuizs)
		{
			foreach (var item in studentQuizs)
			{
				var q = await _studentQuizRepo.GetByIdAsync(item.Id);
				q.MaxDegree = item.MaxDegree;
				q.ExamDegree = item.ExamDegree;
				await _studentQuizRepo.UpdateAsync(q);
			}
			return "";
		}
		#endregion



	}
}
