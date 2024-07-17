using LMS.Repository.EntityRepos.IRepo;
using LMS.Repository.EntityRepos.Repo;
using LMS.Repository.GenericRepo;

namespace LMS.Repository
{
    public static class RepositoryDependencies
    {
        public static IServiceCollection AddRepositoryDependencies(this IServiceCollection services)
        {
            //entities repos
            services.AddScoped<ICourseRepo, CourseRepo>();
            services.AddScoped<ICourseRequestRepo, CourseRequestRepo>();
            services.AddScoped<ICourseStudentRepo, CourseStudentRepo>();
            services.AddScoped<IMaterialRepo, MaterialRepo>();
            services.AddScoped<IOptionRepo, OptionRepo>();
            services.AddScoped<IQuestionRepo, QuestionRepo>();
            services.AddScoped<IQuizRepo, QuizRepo>();
            services.AddScoped<IStudentQuizRepo, StudentQuizRepo>();
            services.AddScoped<IVideoRepo, VideoRepo>();



            services.AddTransient(typeof(IGenericRepositoryAsync<>), typeof(GenericRepositoryAsync<>));

            return services;
        }
    }
}
