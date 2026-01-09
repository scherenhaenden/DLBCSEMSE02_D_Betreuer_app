using ApiProject.ApiLogic.Mappers;

namespace ApiProject.Extensions
{
    public static class ApiMapperExtensions
    {
        public static void AddApiMappers(this IServiceCollection services)
        {
            services.AddScoped<IThesisApiMapper, ThesisApiMapper>();
            services.AddScoped<IThesisOfferApiMapper, ThesisOfferApiMapper>();
            services.AddScoped<IThesisOfferApplicationApiMapper, ThesisOfferApplicationApiMapper>();
            services.AddScoped<IThesisDocumentApiMapper, ThesisDocumentApiMapper>();
        }
    }
}
