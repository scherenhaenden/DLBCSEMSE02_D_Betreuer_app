using ApiProject.ApiLogic.Controllers;
using ApiProject.DatabaseAccess.Context;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net;
using System.Net.Http.Json;
using ApiProject.ApiLogic.Models;
using ApiProject.Constants;

namespace ApiProject.Tests.NUnit.ApiLogic.Controllers;

[TestFixture]
public class TutorControllerTests
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
                        options.UseSqlite("Data Source=TutorControllerTests.db");
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
    public async Task GetTutors_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/api/tutor?page=1&pageSize=10");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task GetTutorById_ExistingId_ReturnsOk()
    {
        // Arrange
        var tutor = _context.Users.FirstOrDefault(u => u.UserRoles.Any(ur => ur.Role.Name == Roles.Tutor));
        if (tutor == null) Assert.Ignore("No tutors seeded");

        // Act
        var response = await _client.GetAsync($"/api/tutor/{tutor.Id}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task GetTutorById_NonExistingId_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync($"/api/tutor/{Guid.NewGuid()}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task UpdateTutor_ValidData_ReturnsOk()
    {
        // Arrange
        var tutor = _context.Users.FirstOrDefault(u => u.UserRoles.Any(ur => ur.Role.Name == Roles.Tutor));
        if (tutor == null) Assert.Ignore("No tutors seeded");

        var updateRequest = new
        {
            FirstName = "Updated First",
            LastName = "Updated Last",
            Email = "updated@example.com"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/tutor/{tutor.Id}", updateRequest);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task UpdateTutor_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var updateRequest = new
        {
            FirstName = "Updated First",
            LastName = "Updated Last",
            Email = "updated@example.com"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/tutor/{Guid.NewGuid()}", updateRequest);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task DeleteTutor_ExistingId_ReturnsNoContent()
    {
        // Arrange
        var tutor = _context.Users.FirstOrDefault(u => u.UserRoles.Any(ur => ur.Role.Name == Roles.Tutor));
        if (tutor == null) Assert.Ignore("No tutors seeded");

        // Act
        var response = await _client.DeleteAsync($"/api/tutor/{tutor.Id}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task DeleteTutor_NonExistingId_ReturnsNotFound()
    {
        // Act
        var response = await _client.DeleteAsync($"/api/tutor/{Guid.NewGuid()}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}
