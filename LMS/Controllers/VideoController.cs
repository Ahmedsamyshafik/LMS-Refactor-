using LMS.Models.Entities;
using LMS.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
    [Authorize]
    public class VideoController : Controller
    {
        #region Fields
        private readonly IVideoService _videoService;
        #endregion

        #region Ctor
        public VideoController(IVideoService videoService)
        {
            _videoService = videoService;
        }
        #endregion

        #region Actions
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add([Bind("Name,VideoPath,CourseId")] Video video)
        {
            await _videoService.Add(video);
            return RedirectToAction("Index", "Course", new { id = video.CourseId });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _videoService.Delete(id);
            return RedirectToAction("Index", "Course", new { id = id });
        }

        #endregion

    }
}
