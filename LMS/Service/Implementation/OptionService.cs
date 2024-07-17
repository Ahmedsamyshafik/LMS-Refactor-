using LMS.Repository.EntityRepos.IRepo;
using LMS.Service.Abstract;

namespace LMS.Service.Implementation
{
    public class OptionService : IOptionService
    {
        #region Fields
        private readonly IOptionRepo _optionRepo;

        #endregion

        #region Ctor
        public OptionService(IOptionRepo optionRepo)
        {
            _optionRepo = optionRepo;
        }
        #endregion

        #region Handle Functions

        #endregion


    }
}
