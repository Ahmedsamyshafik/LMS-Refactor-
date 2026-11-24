namespace LMS.Helper
{
    /// <summary>
    /// Server-side checks for uploaded PDF files. Browsers and the accept attribute can be bypassed,
    /// so the extension, the declared content type and the leading bytes all have to agree.
    /// </summary>
    public static class PdfFileValidator
    {
        private const string PdfContentType = "application/pdf";
        private static readonly byte[] PdfMagicBytes = { 0x25, 0x50, 0x44, 0x46 }; // %PDF

        public static bool TryValidate(IFormFile? file, long maxSizeBytes, out string error)
        {
            if (file is null || file.Length == 0)
            {
                error = "Please choose a PDF file.";
                return false;
            }

            if (file.Length > maxSizeBytes)
            {
                var limitInMb = maxSizeBytes / (1024 * 1024);
                error = $"The file is larger than the allowed {limitInMb} MB.";
                return false;
            }

            if (!string.Equals(Path.GetExtension(file.FileName), ".pdf", StringComparison.OrdinalIgnoreCase))
            {
                error = "Only PDF files are allowed.";
                return false;
            }

            if (!string.Equals(file.ContentType, PdfContentType, StringComparison.OrdinalIgnoreCase))
            {
                error = "The file type is not a valid PDF.";
                return false;
            }

            if (!HasPdfSignature(file))
            {
                error = "The file content is not a valid PDF.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        private static bool HasPdfSignature(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            var header = new byte[PdfMagicBytes.Length];
            var read = stream.Read(header, 0, header.Length);
            return read == PdfMagicBytes.Length && header.AsSpan().SequenceEqual(PdfMagicBytes);
        }
    }
}
