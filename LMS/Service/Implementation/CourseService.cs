using LMS.Helper;
using LMS.Models.Entities;
using LMS.Repository.EntityRepos.IRepo;
using LMS.Service.Abstract;
using LMS.ViewModels.Course;

namespace LMS.Service.Implementation
{
    public class CourseService : ICourseService
    {
        #region Fields
        private readonly ICourseRepo _courseRepo;
        private readonly IMediaService _mediaService;
        #endregion

        #region Ctor
        public CourseService(ICourseRepo courseRepo, IMediaService mediaService)
        {
            _courseRepo = courseRepo;
            _mediaService = mediaService;
        }
        #endregion

        #region Handle Functions

        public async Task AddCourse(AddCourseVM courseVM)
        {
            var Course = new Course();
            if (courseVM.file.Length > 0)
            {
                string imgname = await _mediaService.UploadImage(courseVM.file, Constants.Course);
                Course.ImgPath = imgname;

            }
            Course.Name = courseVM.Name;
            Course.Price = courseVM.Price;
            Course.Description = courseVM.Description;
            if (courseVM.Price == 0)
            {
                Course.free = true;
            }
            await _courseRepo.AddAsync(Course);
        }
        public List<GetCourseVM> GetAll()
        {
            var courses = _courseRepo.GetTableAsList().Select(course => new GetCourseVM
            {
                Id = course.Id,
                Name = course.Name,
                imgPath = course.ImgPath,
                Price = course.Price,
                Description = course.Description,
            }).ToList();
            return courses;
        }

        public GetCourseVM GetById(int id)
        {
            var course = _courseRepo.GetTableNoTracking().Where(x => x.Id == id).FirstOrDefault();
            if (course != null)
            {
                var response = new GetCourseVM()
                {
                    Id = course.Id,
                    imgPath = course.ImgPath,
                    Name = course.Name,
                    Price = course.Price,
                    Description = course.Description,
                };
                return response;
            }
            return null;
        }

        public async Task<string> Delete(int id)
        {
            var course = _courseRepo.GetTableNoTracking().Where(x => x.Id == id).FirstOrDefault();
            if (course != null)
            {
                await _courseRepo.DeleteAsync(course);

            }
            return "";
        }

        #endregion





    }
}
