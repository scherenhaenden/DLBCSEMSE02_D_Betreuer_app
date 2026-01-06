using ApiProject.DatabaseAccess.Context;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using ApiProject.ApiLogic.Models;
using ApiProject.Constants;

namespace ApiProject.Tests.NUnit.ApiLogic.Controllers;

[TestFixture]
public class ThesisControllerTests
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
                        options.UseSqlite("Data Source=ThesisControllerTests.db");
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
    public async Task GetTheses_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/api/thesis?page=1&pageSize=10");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task GetThesisById_ExistingId_ReturnsOk()
    {
        // Arrange
        var thesis = _context.Theses.FirstOrDefault();
        if (thesis == null) Assert.Ignore("No theses seeded");

        // Act
        var response = await _client.GetAsync($"/api/thesis/{thesis.Id}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task GetThesisById_NonExistingId_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync($"/api/thesis/{Guid.NewGuid()}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task CreateThesis_ValidData_ReturnsCreated()
    {
        // Arrange
        var createRequest = new CreateThesisRequest
        {
            Title = "Test Thesis",
            SubjectAreaId = _context.SubjectAreas.First().Id,
            OwnerId = _context.Users.First(u => u.UserRoles.Any(ur => ur.Role.Name == Roles.Student)).Id
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/thesis", createRequest);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }

    [Test]
    public async Task CreateThesis_InvalidData_ReturnsBadRequest()
    {
        // Arrange
        var createRequest = new CreateThesisRequest
        {
            Title = "",
            SubjectAreaId = Guid.NewGuid(),
            OwnerId = Guid.NewGuid()
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/thesis", createRequest);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task UpdateThesis_ValidData_ReturnsOk()
    {
        // Arrange
        var thesis = _context.Theses.FirstOrDefault();
        if (thesis == null) Assert.Ignore("No theses seeded");

        var updateRequest = new UpdateThesisRequest
        {
            Title = "Updated Thesis Title"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/thesis/{thesis.Id}", updateRequest);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task UpdateThesis_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var updateRequest = new UpdateThesisRequest
        {
            Title = "Updated Title"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/thesis/{Guid.NewGuid()}", updateRequest);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task DeleteThesis_ExistingId_ReturnsNoContent()
    {
        // Arrange
        var thesis = _context.Theses.FirstOrDefault();
        if (thesis == null) Assert.Ignore("No theses seeded");

        // Act
        var response = await _client.DeleteAsync($"/api/thesis/{thesis.Id}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task DeleteThesis_NonExistingId_ReturnsNotFound()
    {
        // Act
        var response = await _client.DeleteAsync($"/api/thesis/{Guid.NewGuid()}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}
