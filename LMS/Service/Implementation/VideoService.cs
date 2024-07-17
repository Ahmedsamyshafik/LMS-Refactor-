using LMS.Models.Entities;
using LMS.Repository.EntityRepos.IRepo;
using LMS.Service.Abstract;

namespace LMS.Service.Implementation
{
	public class VideoService : IVideoService
	{
		#region Fields
		private readonly IVideoRepo _Videorepo;

		#endregion

		#region Ctor
		public VideoService(IVideoRepo videorepo)
		{
			_Videorepo = videorepo;
		}
		#endregion

		#region Handle Functions
		public List<Video> getCourseVideos(int courseId)
		{
			return _Videorepo.GetTableNoTracking().Where(x => x.CourseId == courseId).ToList();
		}

		public async Task<string> Add(Video video)
		{
			await _Videorepo.AddAsync(video);
			return "";
		}
		public async Task<string> Delete(int id)
		{
			var dlt = await _Videorepo.GetByIdAsync(id);
			if (dlt != null)
			{
				await _Videorepo.DeleteAsync(dlt);
			}
			return "";
		}
		#endregion


	}
}
