using LMS.Models.Data;
using LMS.Models.Entities;
using LMS.Repository.EntityRepos.IRepo;
using LMS.Repository.GenericRepo;
using Microsoft.EntityFrameworkCore;

namespace LMS.Repository.EntityRepos.Repo
{
    public class MaterialRepo : GenericRepositoryAsync<Material>, IMaterialRepo
    {
        private readonly DbSet<Material> materials;
        public MaterialRepo(AppDbContext db) : base(db)
        {
            materials = db.Set<Material>();
        }
    }
}
