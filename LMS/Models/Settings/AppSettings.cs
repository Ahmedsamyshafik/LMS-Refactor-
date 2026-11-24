namespace LMS.Models.Settings
{
    public class QuizSettings
    {
        public const string SectionName = "Quiz";

        public bool ShowResultToStudent { get; set; }

        public int DefaultDurationMinutes { get; set; } = 60;
    }

    public class UploadSettings
    {
        public const string SectionName = "Uploads";

        public int MaxPdfSizeMb { get; set; } = 20;

        public long MaxPdfSizeBytes => (long)MaxPdfSizeMb * 1024 * 1024;
    }

    public class SiteSettings
    {
        public const string SectionName = "Site";

        public string InstructorName { get; set; } = "أ/أحمد عصام";
    }
}
