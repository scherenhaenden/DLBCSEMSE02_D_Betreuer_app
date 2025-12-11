
using Microsoft.EntityFrameworkCore;
using ApiProject.DatabaseAccess.Context;

namespace ApiProject.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void EnsureDatabaseCreated(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ThesisDbContext>();
                db.Database.EnsureCreated();
            }
        }
    }
}
