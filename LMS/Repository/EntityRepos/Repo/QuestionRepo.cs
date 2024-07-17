using LMS.Models.Data;
using LMS.Models.Entities;
using LMS.Repository.EntityRepos.IRepo;
using LMS.Repository.GenericRepo;
using Microsoft.EntityFrameworkCore;

namespace LMS.Repository.EntityRepos.Repo
{
    public class QuestionRepo : GenericRepositoryAsync<Question>, IQuestionRepo
    {
        private readonly DbSet<Question> questions;
        public QuestionRepo(AppDbContext db) : base(db)
        {
            questions = db.Set<Question>();
        }
    }
}
