using LMS.Models.Data;
using LMS.Models.Entities;
using LMS.Repository.EntityRepos.IRepo;
using LMS.Repository.GenericRepo;
using Microsoft.EntityFrameworkCore;

namespace LMS.Repository.EntityRepos.Repo
{
    public class QuizRepo : GenericRepositoryAsync<Quiz>, IQuizRepo
    {
        private readonly DbSet<Quiz> quizzes;
        public QuizRepo(AppDbContext db) : base(db)
        {
            quizzes = db.Set<Quiz>();
        }
    }
}
