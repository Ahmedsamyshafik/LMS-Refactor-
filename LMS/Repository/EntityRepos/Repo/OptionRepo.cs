using LMS.Models.Data;
using LMS.Models.Entities;
using LMS.Repository.EntityRepos.IRepo;
using LMS.Repository.GenericRepo;
using Microsoft.EntityFrameworkCore;

namespace LMS.Repository.EntityRepos.Repo
{
    public class OptionRepo : GenericRepositoryAsync<Option>, IOptionRepo
    {
        private readonly DbSet<Option> options;
        public OptionRepo(AppDbContext db) : base(db)
        {
            options = db.Set<Option>();
        }
    }
}
