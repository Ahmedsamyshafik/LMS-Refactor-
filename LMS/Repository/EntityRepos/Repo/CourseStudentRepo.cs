using LMS.Models.Data;
using LMS.Models.Entities;
using LMS.Repository.EntityRepos.IRepo;
using LMS.Repository.GenericRepo;
using Microsoft.EntityFrameworkCore;

namespace LMS.Repository.EntityRepos.Repo
{
    public class CourseStudentRepo : GenericRepositoryAsync<CourseStudent>, ICourseStudentRepo
    {
        private readonly DbSet<CourseStudent> courseStudents;
        public CourseStudentRepo(AppDbContext db) : base(db)
        {
            courseStudents = db.Set<CourseStudent>();
        }
    }
}
