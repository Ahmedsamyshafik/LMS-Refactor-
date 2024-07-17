using LMS.Models.Data;
using LMS.Models.Entities;
using LMS.Repository.EntityRepos.IRepo;
using LMS.Repository.GenericRepo;
using Microsoft.EntityFrameworkCore;

namespace LMS.Repository.EntityRepos.Repo
{
    public class CourseRequestRepo : GenericRepositoryAsync<CourseRequest>, ICourseRequestRepo
    {
        private readonly DbSet<CourseRequest> courseRequests;
        public CourseRequestRepo(AppDbContext db) : base(db)
        {
            courseRequests = db.Set<CourseRequest>();
        }
    }
}
