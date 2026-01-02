using ApiProject.BusinessLogic.Services;

namespace ApiProject.Extensions
{
    public static class BusinessLogicServiceExtensions
    {
        public static void AddBusinessLogicServices(this IServiceCollection services)
        {
            services.AddScoped<IUserBusinessLogicService, UserBusinessLogicService>();
            services.AddScoped<IThesisBusinessLogicService, ThesisBusinessLogicService>();
            services.AddScoped<ISubjectAreaBusinessLogicService, SubjectAreaBusinessLogicService>();
            services.AddScoped<IThesisRequestBusinessLogicService, ThesisRequestBusinessLogicService>();
            services.AddScoped<IThesisOfferBusinessLogicService, ThesisOfferBusinessLogicService>();
        }
    }
}
