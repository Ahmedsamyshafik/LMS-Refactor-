using LMS.Models.Entities;
using LMS.Models.Enums;
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

        public async Task<string> Update(StudentQuiz studentQuiz)
        {
            await _studentQuizRepo.UpdateAsync(studentQuiz);
            return "";
        }

        /// <summary>
        /// Only a submitted attempt counts as taken. An attempt that is still in progress must keep the
        /// student able to open the exam again and finish it.
        /// </summary>
        public bool StudentTakeExam(string userId, int quizId)
        {
            return GetSubmittedAttempt(userId, quizId) != null;
        }

        public StudentQuiz? GetSubmittedAttempt(string userId, int quizId)
        {
            return _studentQuizRepo.GetTableNoTracking()
                .Where(x => x.StudentId == userId && x.QuizId == quizId && x.Status == ExamAttemptStatus.Submitted)
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();
        }

        public StudentQuiz? GetInProgressAttempt(string userId, int quizId)
        {
            return _studentQuizRepo.GetTableNoTracking()
                .Where(x => x.StudentId == userId && x.QuizId == quizId && x.Status == ExamAttemptStatus.InProgress)
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();
        }

        public List<StudentQuiz> GetStudents(int courseId)
        {
            return SubmittedAttemptsOfCourse(courseId)
                .Include(x => x.Student)
                .Include(x => x.Quiz)
                .OrderBy(x => x.Id)
                .ToList();
        }

        public List<StudentQuiz> GetAll(int courseId)
        {
            return SubmittedAttemptsOfCourse(courseId)
                .Include(x => x.Student)
                .Include(x => x.Quiz)
                .OrderBy(x => x.Id)
                .ToList();
        }

        /// <summary>
        /// Written attempts of a course, kept apart from multiple choice ones so that grading a written
        /// answer cannot land on an auto corrected attempt.
        /// </summary>
        public List<StudentQuiz> GetWrittenByCourse(int courseId)
        {
            return WrittenAttempts(SubmittedAttemptsOfCourse(courseId))
                .Include(x => x.Student)
                .Include(x => x.Quiz)
                .OrderBy(x => x.Id)
                .ToList();
        }

        public StudentQuiz? GetById(int id)
        {
            return _studentQuizRepo.GetTableNoTracking()
                .Where(x => x.Id == id)
                .Include(x => x.Quiz)
                .FirstOrDefault();
        }

        public async Task<string> CorrectingWritten(int courseId, List<StudentQuiz> studentQuizs)
        {
            if (studentQuizs == null || studentQuizs.Count == 0)
            {
                return "Nothing was saved.";
            }

            var postedIds = studentQuizs.Select(x => x.Id).Distinct().ToList();
            var targets = await WrittenAttempts(_studentQuizRepo.GetTableAsTracking())
                .Where(x => postedIds.Contains(x.Id)
                            && x.CourseId == courseId
                            && x.Status == ExamAttemptStatus.Submitted)
                .ToListAsync();

            var saved = 0;
            foreach (var attempt in targets)
            {
                var posted = studentQuizs.First(x => x.Id == attempt.Id);
                if (posted.MaxDegree < 0 || posted.ExamDegree < 0 || posted.ExamDegree > posted.MaxDegree)
                {
                    continue;
                }

                attempt.MaxDegree = posted.MaxDegree;
                attempt.ExamDegree = posted.ExamDegree;
                saved++;
            }

            await _studentQuizRepo.SaveChangesAsync();

            return saved == 0
                ? "No written attempt matched the submitted rows."
                : $"{saved} written attempt(s) saved.";
        }

        public List<StudentQuiz> All()
        {
            return _studentQuizRepo.GetTableNoTracking().Include(x => x.Course).ToList();
        }
        #endregion

        #region Private
        private IQueryable<StudentQuiz> SubmittedAttemptsOfCourse(int courseId)
        {
            return _studentQuizRepo.GetTableNoTracking()
                .Where(x => x.CourseId == courseId && x.Status == ExamAttemptStatus.Submitted);
        }

        private static IQueryable<StudentQuiz> WrittenAttempts(IQueryable<StudentQuiz> attempts)
        {
            return attempts.Where(x => x.Quiz.FilePath != null && x.Quiz.FilePath != "");
        }
        #endregion
    }
}
