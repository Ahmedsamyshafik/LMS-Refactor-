namespace LMS.ViewModels.Material
{
    public class MaterialVM
    {
        public string Name { get; set; }
        public int CourseId { get; set; }
        public IFormFile file { get; set; }
    }
}
