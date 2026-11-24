using LMS.Models.Entities;

namespace LMS.Service.Abstract
{
    public interface IStudentQuizService
    {
        Task<string> Add(StudentQuiz studentQuiz);
        Task<string> Update(StudentQuiz studentQuiz);
        bool StudentTakeExam(string userId, int quizId);
        StudentQuiz? GetSubmittedAttempt(string userId, int quizId);
        StudentQuiz? GetInProgressAttempt(string userId, int quizId);
        List<StudentQuiz> GetStudents(int courseId);
        List<StudentQuiz> GetAll(int courseId);
        List<StudentQuiz> GetWrittenByCourse(int courseId);
        StudentQuiz? GetById(int id);
        Task<string> CorrectingWritten(int courseId, List<StudentQuiz> studentQuizs);
        List<StudentQuiz> All();


    }
}
