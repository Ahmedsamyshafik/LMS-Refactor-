using LMS.Service.Abstract;

namespace LMS.Service.Implementation
{
	public class MediaService : IMediaService
	{
		#region Fields
		private readonly IWebHostEnvironment _webHostEnvironment;


		#endregion

		#region Ctor
		public MediaService(IWebHostEnvironment webHostEnvironment)
		{
			_webHostEnvironment = webHostEnvironment;
		}
		#endregion

		#region Handle Functions
		public async Task<string> UploadImage(IFormFile imageFile, string folderName)
		{
			// Create a unique file name for the image
			var fileName = Path.GetFileNameWithoutExtension(imageFile.FileName);
			var extension = Path.GetExtension(imageFile.FileName);
			var uniqueFileName = $"{fileName}_{Guid.NewGuid()}{extension}";

			// Get the path to the "uploads" folder in the wwwroot directory
			var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, $"assets/images/{folderName}");

			// Ensure the "uploads" folder exists
			if (!Directory.Exists(uploadsFolder))
			{
				Directory.CreateDirectory(uploadsFolder);
			}

			// Combine the path to get the full path to the image file
			var filePath = Path.Combine(uploadsFolder, uniqueFileName);

			// Save the image file to the specified path
			using (var fileStream = new FileStream(filePath, FileMode.Create))
			{
				await imageFile.CopyToAsync(fileStream);
			}

			// Return a success response (e.g., the URL of the uploaded image)
			var imageUrl = $"assets/images/{folderName}/{uniqueFileName}";
			return imageUrl;
		}

		public void _DeleteFile(string name, string folderName)
		{
			var ser = _webHostEnvironment.WebRootPath;
			var r2 = ser + $"assets/images/{folderName}/" + name;
			if (System.IO.File.Exists(r2)) System.IO.File.Delete(r2);
		}
		#endregion
	}
}
