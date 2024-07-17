using LMS.Repository.EntityRepos.IRepo;
using LMS.Service.Abstract;

namespace LMS.Service.Implementation
{
    public class QuestionService : IQuestionService
    {
        #region Fields
        private readonly IQuestionRepo _questionRepo;
        #endregion

        #region Ctor
        public QuestionService(IQuestionRepo questionRepo)
        {
            _questionRepo = questionRepo;
        }
        #endregion

        #region Handle Functions

        #endregion



    }
}
