using LMS.Helper;
using Xunit;

namespace LMS.Tests
{
    public class UploadGuardTests
    {
        private const long TenMb = 10 * 1024 * 1024;

        [Fact]
        public void Pdf_Accepted()
        {
            Assert.True(PdfFileValidator.TryValidate(TestFactory.PdfFile(), TenMb, out var error));
            Assert.Equal(string.Empty, error);
        }

        [Fact]
        public void MissingFile_Rejected()
        {
            Assert.False(PdfFileValidator.TryValidate(null, TenMb, out var error));
            Assert.False(string.IsNullOrWhiteSpace(error));
        }

        [Theory]
        [InlineData("payload.exe")]
        [InlineData("page.svg")]
        [InlineData("notes.txt")]
        public void OtherExtensions_Rejected(string fileName)
        {
            Assert.False(PdfFileValidator.TryValidate(TestFactory.PdfFile(fileName), TenMb, out _));
        }

        [Fact]
        public void PdfExtensionWithScriptContent_RejectedByMagicBytes()
        {
            var disguised = TestFactory.PdfFile("report.pdf", "<script>alert(1)</script>");

            Assert.False(PdfFileValidator.TryValidate(disguised, TenMb, out var error));
            Assert.Equal("The file content is not a valid PDF.", error);
        }

        [Fact]
        public void PdfWithWrongDeclaredType_Rejected()
        {
            var file = TestFactory.PdfFile(contentType: "image/svg+xml");

            Assert.False(PdfFileValidator.TryValidate(file, TenMb, out _));
        }

        [Fact]
        public void OversizedFile_Rejected()
        {
            var file = TestFactory.PdfFile(content: "%PDF" + new string('a', 2048));

            Assert.False(PdfFileValidator.TryValidate(file, 1024, out var error));
            Assert.Contains("larger", error);
        }

        [Theory]
        [InlineData(0, "Free")]
        [InlineData(-10, "Free")]
        [InlineData(250, "250")]
        public void Price_HasOneRenderingRule(int price, string expected)
        {
            Assert.Equal(expected, PriceFormatter.Format(price));
        }
    }
}
