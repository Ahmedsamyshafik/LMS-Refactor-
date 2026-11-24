using LMS.Helper;
using LMS.Models.Entities;

namespace LMS.ViewModels.QuizVM
{
    /// <summary>
    /// What the browser needs to run the countdown. The remaining time is a plain number of seconds
    /// measured by the server, so no device clock or time zone can shorten or lengthen the exam.
    /// </summary>
    public class TakeExamVM
    {
        public Quiz Quiz { get; set; } = null!;

        public int AttemptId { get; set; }

        public int RemainingSeconds { get; set; }

        public bool IsWrittenExam => !string.IsNullOrEmpty(Quiz?.FilePath);

        public string CountdownText => ExamClock.FormatTimeSpan(RemainingSeconds);
    }

    public class ExamResultVM
    {
        public Quiz Quiz { get; set; } = null!;

        public StudentQuiz Attempt { get; set; } = null!;

        public bool IsWrittenExam => !string.IsNullOrEmpty(Quiz?.FilePath);

        public bool IsAwaitingCorrection => IsWrittenExam && Attempt.MaxDegree == 0;
    }
}
