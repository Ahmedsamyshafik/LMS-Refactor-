using LMS.Service.Abstract;
using LMS.ViewModels.Material;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
	public class MaterialController : Controller
	{
		#region MyRegion
		private readonly IMaterialService _materialService;
		#endregion

		#region MyRegion

		public MaterialController(IMaterialService materialService)
		{
			_materialService = materialService;
		}
		#endregion

		#region MyRegion
		public IActionResult index(int id)
		{
			ViewBag.Courseid = id;
			return View();
		}

		public async Task<IActionResult> Add(MaterialVM met)
		{
			await _materialService.Add(met);

			return RedirectToAction("index", new { id = met.CourseId });
		}

		public async Task<IActionResult> GetAll(int id)//courseId
		{
			var mats = _materialService.GetAllMaterialByCourseId(id);
			return View(mats);
		}
		public async Task<IActionResult> DisplayMaterial(int id)//material id 
		{
			var mat = _materialService.GetById(id);
			return View(mat);
		}
		#endregion

	}
}
