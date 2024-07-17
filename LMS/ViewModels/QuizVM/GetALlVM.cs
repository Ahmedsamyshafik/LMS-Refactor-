using LMS.Models.Entities;

namespace LMS.ViewModels.QuizVM
{
    public class GetAllVM
    {
        public List<inhertQuiz> quizzes { get; set; }

        public IEnumerable<StudentQuiz> students { get; set; }
    }
    public class inhertQuiz : Quiz
    {
        public bool StudentTakedExam { get; set; }
    }
}
