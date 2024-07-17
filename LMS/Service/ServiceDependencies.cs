using LMS.Service.Abstract;
using LMS.Service.Implementation;

namespace LMS.Service
{
    public static class ServiceDependencies
    {
        public static IServiceCollection AddServiceDependencies(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IMediaService, MediaService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<ICourseRequestService, CourseRequestService>();
            services.AddScoped<ICourseStudentService, CourseStudentService>();
            services.AddScoped<IMaterialService, MaterialService>();
            services.AddScoped<IOptionService, OptionService>();
            services.AddScoped<IQuestionService, QuestionService>();
            services.AddScoped<IQuizService, QuizService>();
            services.AddScoped<IStudentQuizService, StudentQuizService>();
            services.AddScoped<IVideoService, VideoService>();
            return services;
        }
    }
}
