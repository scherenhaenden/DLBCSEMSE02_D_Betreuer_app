using ApiProject.BusinessLogic.Services;

namespace ApiProject.Extensions
{
    public static class BusinessLogicServiceExtensions
    {
        public static void AddBusinessLogicServices(this IServiceCollection services)
        {
            services.AddScoped<IUserBusinessLogicService, UserBusinessLogicService>();
            services.AddScoped<IThesisService, ThesisService>();
            services.AddScoped<ISubjectAreaService, SubjectAreaService>();
            services.AddScoped<IThesisRequestService, ThesisRequestService>();
        }
    }
}
