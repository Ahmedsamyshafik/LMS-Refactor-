using LMS.Helper;
using Xunit;

namespace LMS.Tests
{
    public class ExamClockTests
    {
        private static readonly DateTime Start = new(2026, 11, 23, 10, 0, 0, DateTimeKind.Utc);

        [Fact]
        public void RemainingSeconds_HalfElapsed_ReturnsServerMinutes()
        {
            var now = Start.AddMinutes(30);

            Assert.Equal(1800, ExamClock.RemainingSeconds(Start, 60, now));
        }

        [Fact]
        public void RemainingSeconds_PastEndTime_ReturnsZeroInsteadOfNegative()
        {
            var now = Start.AddMinutes(61);

            Assert.Equal(0, ExamClock.RemainingSeconds(Start, 60, now));
            Assert.True(ExamClock.IsExpired(Start, 60, now));
        }

        [Fact]
        public void RemainingSeconds_NonUtcStart_IsTreatedAsUtc()
        {
            // A DateTime coming from SQL has Kind Unspecified; it must not be shifted by the local zone.
            var unspecified = DateTime.SpecifyKind(Start, DateTimeKind.Unspecified);

            Assert.Equal(3600, ExamClock.RemainingSeconds(unspecified, 60, Start));
        }

        [Fact]
        public void RemainingSeconds_ClockSkewIntoTheFuture_DoesNotThrow()
        {
            Assert.Equal(3600, ExamClock.RemainingSeconds(Start, 60, Start.AddMinutes(-5)));
        }

        [Fact]
        public void SubmissionWindow_ClosesAfterTheGracePeriod()
        {
            var justAfter = Start.AddMinutes(60).AddSeconds(ExamClock.SubmissionGraceSeconds - 1);
            var tooLate = Start.AddMinutes(60).AddSeconds(ExamClock.SubmissionGraceSeconds + 1);

            Assert.True(ExamClock.IsWithinSubmissionWindow(Start, 60, justAfter));
            Assert.False(ExamClock.IsWithinSubmissionWindow(Start, 60, tooLate));
        }

        [Theory]
        [InlineData(0, "00:00:00")]
        [InlineData(3661, "01:01:01")]
        [InlineData(-5, "00:00:00")]
        public void FormatTimeSpan_PadsToClockFormat(int seconds, string expected)
        {
            Assert.Equal(expected, ExamClock.FormatTimeSpan(seconds));
        }
    }
}
