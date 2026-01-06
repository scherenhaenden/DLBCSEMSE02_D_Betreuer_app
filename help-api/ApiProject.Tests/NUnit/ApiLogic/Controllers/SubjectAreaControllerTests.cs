using ApiProject.DatabaseAccess.Context;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using ApiProject.ApiLogic.Models;

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
                builder.ConfigureServices(services =>
                {
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

        // Seed data if needed
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
        var response = await _client.GetAsync("/api/subjectarea?page=1&pageSize=10");

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
        var response = await _client.GetAsync($"/api/subjectarea/{subjectArea.Id}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task GetSubjectAreaById_NonExistingId_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync($"/api/subjectarea/{Guid.NewGuid()}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task SearchSubjectAreas_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/api/subjectarea/search?searchTerm=test&page=1&pageSize=10");

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
        var response = await _client.PostAsJsonAsync("/api/subjectarea", createRequest);

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
        var response = await _client.PostAsJsonAsync("/api/subjectarea", createRequest);

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
        var response = await _client.PutAsJsonAsync($"/api/subjectarea/{subjectArea.Id}", updateRequest);

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
        var response = await _client.PutAsJsonAsync($"/api/subjectarea/{Guid.NewGuid()}", updateRequest);

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
        var response = await _client.DeleteAsync($"/api/subjectarea/{subjectArea.Id}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task DeleteSubjectArea_NonExistingId_ReturnsNotFound()
    {
        // Act
        var response = await _client.DeleteAsync($"/api/subjectarea/{Guid.NewGuid()}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}
