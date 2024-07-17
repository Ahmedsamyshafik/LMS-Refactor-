using LMS.Models.Data;
using LMS.Models.Entities;
using LMS.Repository.EntityRepos.IRepo;
using LMS.Repository.GenericRepo;
using Microsoft.EntityFrameworkCore;

namespace LMS.Repository.EntityRepos.Repo
{
    public class VideoRepo : GenericRepositoryAsync<Video>, IVideoRepo
    {
        private readonly DbSet<Video> videos;
        public VideoRepo(AppDbContext db) : base(db)
        {
            videos = db.Set<Video>();
        }
    }
}
