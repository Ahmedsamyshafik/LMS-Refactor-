using LMS.Helper;
using LMS.Models.Settings;
using LMS.Service.Abstract;
using LMS.ViewModels.Material;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LMS.Controllers
{
    [Authorize]
    public class MaterialController : Controller
    {
        #region MyRegion
        private readonly IMaterialService _materialService;
        private readonly ICourseService _courseService;
        private readonly ICourseStudentService _courseStudentService;
        private readonly UploadSettings _uploadSettings;
        #endregion

        #region MyRegion

        public MaterialController(IMaterialService materialService, ICourseService courseService,
            ICourseStudentService courseStudentService, IOptions<UploadSettings> uploadSettings)
        {
            _materialService = materialService;
            _courseService = courseService;
            _courseStudentService = courseStudentService;
            _uploadSettings = uploadSettings.Value;
        }
        #endregion

        #region MyRegion
        [HttpGet]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult index(int id)
        {
            if (_courseService.GetById(id) == null)
            {
                return NotFound();
            }

            ViewBag.Courseid = id;
            return View(new MaterialVM { CourseId = id });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(MaterialVM met)
        {
            if (!PdfFileValidator.TryValidate(met.file, _uploadSettings.MaxPdfSizeBytes, out var error))
            {
                ModelState.AddModelError(nameof(met.file), error);
            }

            if (_courseService.GetById(met.CourseId) == null)
            {
                ModelState.AddModelError(nameof(met.CourseId), " Course not found.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Courseid = met.CourseId;
                return View("index", met);
            }

            await _materialService.Add(met);

            return RedirectToAction("index", new { id = met.CourseId });
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetAll(int id)//courseId
        {
            if (_courseService.GetById(id) == null)
            {
                return NotFound();
            }

            if (!CanReadCourse(id))
            {
                return Forbid();
            }

            var mats = _materialService.GetAllMaterialByCourseId(id);
            return View(mats);
        }

        [HttpGet]
        [Authorize]
        public IActionResult DisplayMaterial(int id)//material id
        {
            var mat = _materialService.GetById(id);
            if (mat == null)
            {
                return NotFound();
            }

            if (!CanReadCourse(mat.CourseId))
            {
                return Forbid();
            }

            return View(mat);
        }
        #endregion

        #region Private
        /// <summary>
        /// Materials belong to the course they were uploaded for, so only staff and learners enrolled in that
        /// course may read them.
        /// </summary>
        private bool CanReadCourse(int courseId)
        {
            if (User.IsInRole("Admin") || User.IsInRole("Teacher"))
            {
                return true;
            }

            var userId = User.Identity?.IsAuthenticated == true
                ? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                : null;

            return !string.IsNullOrEmpty(userId) && _courseStudentService.StudentInCourse(courseId, userId);
        }
        #endregion
    }
}
