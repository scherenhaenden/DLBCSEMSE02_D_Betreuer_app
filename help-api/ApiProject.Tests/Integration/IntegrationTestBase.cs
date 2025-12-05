using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ApiProject.Db.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace ApiProject.Tests.Integration;

/// <summary>
/// Base class for integration tests using WebApplicationFactory.
/// </summary>
public class IntegrationTestBase : WebApplicationFactory<Program>, IAsyncDisposable
{
    protected HttpClient Client { get; private set; } = null!;

    public IntegrationTestBase()
    {
        Client = CreateClient();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["UseSqlite"] = "false"
            });
        });

        builder.ConfigureServices(services =>
        {
            // Remove existing DbContext registrations
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(ThesisDbContext));
            if (dbContextDescriptor != null)
            {
                services.Remove(dbContextDescriptor);
            }

            var optionsDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ThesisDbContext>));
            if (optionsDescriptor != null)
            {
                services.Remove(optionsDescriptor);
            }

            services.AddDbContext<ThesisDbContext>(options =>
            {
                options.UseInMemoryDatabase(Guid.NewGuid().ToString());
            });
        });
    }

    public async ValueTask DisposeAsync()
    {
        Client.Dispose();
        await base.DisposeAsync();
    }
}
