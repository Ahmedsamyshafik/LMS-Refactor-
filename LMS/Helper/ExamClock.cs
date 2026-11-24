namespace LMS.Helper
{
    /// <summary>
    /// Server-side exam clock. Every value is UTC; nothing here reads the wall clock of the caller's machine.
    /// </summary>
    public static class ExamClock
    {
        /// <summary>Tolerance for a submission that was already in flight when the clock ran out.</summary>
        public const int SubmissionGraceSeconds = 30;

        public static DateTime AsUtc(DateTime value)
        {
            return value.Kind == DateTimeKind.Utc
                ? value
                : DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        public static DateTime ExpiresAtUtc(DateTime startedAtUtc, int durationMinutes)
        {
            return AsUtc(startedAtUtc).AddMinutes(Math.Max(durationMinutes, 0));
        }

        public static int RemainingSeconds(DateTime startedAtUtc, int durationMinutes, DateTime nowUtc)
        {
            var start = AsUtc(startedAtUtc);
            var elapsedSeconds = (AsUtc(nowUtc) - start).TotalSeconds;
            if (elapsedSeconds < 0)
            {
                elapsedSeconds = 0;
            }

            var remaining = durationMinutes * 60d - elapsedSeconds;
            return remaining <= 0 ? 0 : (int)Math.Ceiling(remaining);
        }

        public static bool IsExpired(DateTime startedAtUtc, int durationMinutes, DateTime nowUtc)
        {
            return RemainingSeconds(startedAtUtc, durationMinutes, nowUtc) == 0;
        }

        /// <summary>
        /// A submission is accepted while the exam runs and for a short grace period afterwards, so that a
        /// student who clicks submit at the very end is not punished for network latency.
        /// </summary>
        public static bool IsWithinSubmissionWindow(DateTime startedAtUtc, int durationMinutes, DateTime nowUtc)
        {
            return AsUtc(nowUtc) <= ExpiresAtUtc(startedAtUtc, durationMinutes).AddSeconds(SubmissionGraceSeconds);
        }

        public static string FormatTimeSpan(int totalSeconds)
        {
            if (totalSeconds < 0)
            {
                totalSeconds = 0;
            }

            return TimeSpan.FromSeconds(totalSeconds).ToString(@"hh\:mm\:ss");
        }
    }
}
