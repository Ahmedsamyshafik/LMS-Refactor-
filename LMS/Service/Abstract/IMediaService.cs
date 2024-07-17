namespace LMS.Service.Abstract
{
	public interface IMediaService
	{
		Task<string> UploadImage(IFormFile imageFile, string folderName);
		void _DeleteFile(string name, string folderName);
	}
}
