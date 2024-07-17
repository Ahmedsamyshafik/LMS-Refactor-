using LMS.Models.Entities;

namespace LMS.Service.Abstract
{
	public interface IVideoService
	{
		List<Video> getCourseVideos(int courseId);
		Task<string> Add(Video video);
		Task<string> Delete(int id);

	}
}
