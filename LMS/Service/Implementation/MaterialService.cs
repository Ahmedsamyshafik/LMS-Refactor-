using LMS.Helper;
using LMS.Models.Entities;
using LMS.Repository.EntityRepos.IRepo;
using LMS.Service.Abstract;
using LMS.ViewModels.Material;

namespace LMS.Service.Implementation
{
	public class MaterialService : IMaterialService
	{
		#region Fields
		private readonly IMaterialRepo _materialRepo;
		private readonly IMediaService _mediaService;
		#endregion

		#region Ctor
		public MaterialService(IMaterialRepo materialRepo, IMediaService mediaService)
		{
			_materialRepo = materialRepo;
			_mediaService = mediaService;
		}
		#endregion

		#region Handle Functions

		public async Task<string> Add(MaterialVM material)
		{
			var res = new Material();
			if (material.file.Length > 0)
			{
				string url = await _mediaService.UploadImage(material.file, Constants.Material);
				res.PdfPath = url;
			}
			res.Name = material.Name;
			res.CourseId = material.CourseId;
			await _materialRepo.AddAsync(res);
			return "";
		}

		public List<Material> GetAllMaterialByCourseId(int courseId)
		{
			var result = _materialRepo.GetTableNoTracking().Where(x => x.CourseId == courseId).ToList();
			return result;
		}

		public Material GetById(int id)
		{
			return _materialRepo.GetTableNoTracking().Where(x => x.Id == id).FirstOrDefault();
		}

		#endregion



	}
}
