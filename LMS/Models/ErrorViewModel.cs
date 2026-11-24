namespace LMS.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public int StatusCode { get; set; } = 500;

        public bool IsNotFound => StatusCode == 404;

        public string Title => StatusCode switch
        {
            400 => "الطلب غير صالح",
            403 => "ممنوع الوصول",
            404 => "العنصر غير موجود",
            _ => "حدث خطأ غير متوقع"
        };
    }
}
