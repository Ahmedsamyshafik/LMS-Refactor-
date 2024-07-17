using LMS.Models.Entities;
using LMS.ViewModels.Material;

namespace LMS.Service.Abstract
{
	public interface IMaterialService
	{
		Task<string> Add(MaterialVM material);
		List<Material> GetAllMaterialByCourseId(int courseId);
		Material GetById(int id);

	}
}
