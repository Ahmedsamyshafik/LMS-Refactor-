using LMS.Models.Data;
using LMS.Models.Entities;
using LMS.Repository.EntityRepos.IRepo;
using LMS.Repository.GenericRepo;
using Microsoft.EntityFrameworkCore;

namespace LMS.Repository.EntityRepos.Repo
{
    public class CourseRepo : GenericRepositoryAsync<Course>, ICourseRepo
    {
        private readonly DbSet<Course> courses;
        public CourseRepo(AppDbContext db) : base(db)
        {
            courses = db.Set<Course>();
        }
    }
}
