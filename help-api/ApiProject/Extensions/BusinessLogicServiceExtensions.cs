using ApiProject.BusinessLogic.Services;

namespace ApiProject.Extensions
{
    public static class BusinessLogicServiceExtensions
    {
        public static void AddBusinessLogicServices(this IServiceCollection services)
        {
            services.AddScoped<IUserBusinessLogicService, UserBusinessLogicService>();
            services.AddScoped<IThesisService, ThesisService>();
            services.AddScoped<ISubjectAreaBusinessLogicService, SubjectAreaBusinessLogicService>();
            services.AddScoped<IThesisRequestService, ThesisRequestService>();
        }
    }
}
