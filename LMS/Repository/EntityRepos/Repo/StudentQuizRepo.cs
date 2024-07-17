using LMS.Models.Data;
using LMS.Models.Entities;
using LMS.Repository.EntityRepos.IRepo;
using LMS.Repository.GenericRepo;
using Microsoft.EntityFrameworkCore;

namespace LMS.Repository.EntityRepos.Repo
{
    public class StudentQuizRepo : GenericRepositoryAsync<StudentQuiz>, IStudentQuizRepo
    {
        private readonly DbSet<StudentQuiz> studentQuizzes;
        public StudentQuizRepo(AppDbContext db) : base(db)
        {
            studentQuizzes = db.Set<StudentQuiz>();
        }
    }
}
