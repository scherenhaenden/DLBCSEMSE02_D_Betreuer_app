using ApiProject.DatabaseAccess.Context;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using ApiProject.ApiLogic.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace ApiProject.Tests.NUnit.ApiLogic.Controllers;

[TestFixture]
public class SubjectAreaControllerTests
{
    private WebApplicationFactory<Program> _factory;
    private HttpClient _client;
    private ThesisDbContext _context;

    [SetUp]
    public void SetUp()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, config) =>
                {
                    config.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        ["Database:SeedJsonPath"] = null
                    });
                });
                builder.ConfigureServices(services =>
                {
                    // Service Container Modification
                    // Purpose: Replaces the production database configuration with a test-specific SQLite database.
                    // Service Descriptor Removal:
                    // - Production registers DbContextOptions<ThesisDbContext> for the actual database (e.g., SQL Server).
                    // - SingleOrDefault finds the existing registration to avoid conflicts.
                    // - services.Remove(descriptor) eliminates the production configuration.
                    // SQLite Database Addition:
                    // - AddDbContext<ThesisDbContext> registers EF Core with SQLite provider.
                    // - "Data Source=SubjectAreaControllerTests.db" specifies a file-based SQLite database.
                    // - Unique filename prevents interference between test classes.
                    // Why SQLite?
                    // - Isolation: File-based database ensures each test class has its own data store.
                    // - Performance: Faster than network databases, no connection overhead.
                    // - Compatibility: Same EF Core LINQ queries work with SQLite as with production databases.
                    // - Cleanup: Easy to delete the file after tests.
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ThesisDbContext>));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }
                    services.AddDbContext<ThesisDbContext>(options =>
                    {
                        options.UseSqlite("Data Source=SubjectAreaControllerTests.db");
                    });
                });
            });

        _client = _factory.CreateClient();
        var scope = _factory.Services.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<ThesisDbContext>();
        _context.Database.EnsureCreated();

        // Seed the database with test data
        var seeder = new TestDataSeeder(_context);
        seeder.SeedRoles();

        _context.SaveChanges();
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        _client.Dispose();
        _factory.Dispose();
    }

    [Test]
    public async Task GetSubjectAreas_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/subject-areas?page=1&pageSize=10");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task GetSubjectAreaById_ExistingId_ReturnsOk()
    {
        // Arrange
        var subjectArea = _context.SubjectAreas.FirstOrDefault();
        if (subjectArea == null) Assert.Ignore("No subject areas seeded");

        // Act
        var response = await _client.GetAsync($"/subject-areas/{subjectArea.Id}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task GetSubjectAreaById_NonExistingId_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync($"/subject-areas/{Guid.NewGuid()}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task SearchSubjectAreas_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/subject-areas/search?q=test&page=1&pageSize=10");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task CreateSubjectArea_ValidData_ReturnsCreated()
    {
        // Arrange
        var createRequest = new CreateSubjectAreaRequest
        {
            Title = "Test Subject Area",
            Description = "Description",
            TutorIds = new List<Guid>() // Empty for simplicity
        };

        // Act
        var response = await _client.PostAsJsonAsync("/subject-areas", createRequest);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }

    [Test]
    public async Task CreateSubjectArea_InvalidData_ReturnsBadRequest()
    {
        // Arrange
        var createRequest = new CreateSubjectAreaRequest
        {
            Title = "",
            Description = "Description",
            TutorIds = new List<Guid>()
        };

        // Act
        var response = await _client.PostAsJsonAsync("/subject-areas", createRequest);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task UpdateSubjectArea_ValidData_ReturnsOk()
    {
        // Arrange
        var subjectArea = _context.SubjectAreas.FirstOrDefault();
        if (subjectArea == null) Assert.Ignore("No subject areas seeded");

        var updateRequest = new UpdateSubjectAreaRequest
        {
            Title = "Updated Title",
            Description = "Updated Description"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/subject-areas/{subjectArea.Id}", updateRequest);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task UpdateSubjectArea_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var updateRequest = new UpdateSubjectAreaRequest
        {
            Title = "Updated Title",
            Description = "Updated Description"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/subject-areas/{Guid.NewGuid()}", updateRequest);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task DeleteSubjectArea_ExistingId_ReturnsNoContent()
    {
        // Arrange
        var subjectArea = _context.SubjectAreas.FirstOrDefault();
        if (subjectArea == null) Assert.Ignore("No subject areas seeded");

        // Act
        var response = await _client.DeleteAsync($"/subject-areas/{subjectArea.Id}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task DeleteSubjectArea_NonExistingId_ReturnsNotFound()
    {
        // Act
        var response = await _client.DeleteAsync($"/subject-areas/{Guid.NewGuid()}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task SearchThreeDifferentSubjectAreasByContainingAndShouldGiveMeBackAllTheSubjectsContainingSimilarNamesShouldPass()
    {
        // Arrange
        // Seed 20 subject areas
        var subjectAreas = new List<DatabaseAccess.Entities.SubjectAreaDataAccessModel>();
        for (int i = 1; i <= 20; i++)
        {
            string title = $"Subject Area {i}";
            if (i <= 3)
            {
                title = $"Computer {i}";
            }
            subjectAreas.Add(new DatabaseAccess.Entities.SubjectAreaDataAccessModel
            {
                Id = Guid.NewGuid(),
                Title = title,
                Description = $"Description for {title}"
            });
        }
        _context.SubjectAreas.AddRange(subjectAreas);
        _context.SaveChanges();

        // Act
        var response = await _client.GetAsync("/subject-areas/search?q=Computer&page=1&pageSize=10");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var content = await response.Content.ReadFromJsonAsync<PaginatedResponse<SubjectAreaResponse>>();
        Assert.That(content.Items.Count, Is.EqualTo(3));
        Assert.That(content.TotalCount, Is.EqualTo(3));
        Assert.That(content.Items.All(sa => sa.Title.Contains("Computer")), Is.True);
    }
}
