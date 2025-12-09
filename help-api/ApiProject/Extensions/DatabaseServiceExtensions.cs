using ApiProject.DatabaseAccess.Context;
using Microsoft.EntityFrameworkCore;
using System;

namespace ApiProject.Extensions
{
    public static class DatabaseServiceExtensions
    {
        public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var dbType = configuration["Database:Type"];
            var connectionString = configuration["Database:ConnectionString"];

            switch (dbType?.ToLower())
            {
                case "sqlite":
                    services.AddDbContext<ThesisDbContext>(options =>
                        options.UseSqlite(connectionString));
                    break;
                // Add other database providers here if needed
                // case "sqlserver":
                //     services.AddDbContext<ThesisDbContext>(options =>
                //         options.UseSqlServer(connectionString));
                //     break;
                default:
                    throw new NotSupportedException($"Database type '{dbType}' is not supported.");
            }
        }
    }
}
