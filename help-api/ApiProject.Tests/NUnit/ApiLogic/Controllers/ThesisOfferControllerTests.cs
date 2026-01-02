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
public class ThesisOfferControllerTests
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
                        options.UseSqlite("Data Source=ThesisOfferControllerTests.db");
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
    public async Task GetThesisOffers_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/api/thesisoffer?page=1&pageSize=10");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task CreateThesisOffer_ValidData_ReturnsCreated()
    {
        // Arrange
        var tutor = _context.Users.FirstOrDefault(u => u.UserRoles.Any(ur => ur.Role.Name == Roles.Tutor));
        var subjectArea = _context.SubjectAreas.FirstOrDefault();
        if (tutor == null || subjectArea == null) Assert.Ignore("No tutors or subject areas seeded");

        var createRequest = new CreateThesisOfferRequest
        {
            Title = "Test Offer",
            Description = "Description",
            SubjectAreaId = subjectArea.Id,
            TutorId = tutor.Id,
            MaxStudents = 2,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/thesisoffer", createRequest);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }

    [Test]
    public async Task CreateThesisOffer_InvalidData_ReturnsBadRequest()
    {
        // Arrange
        var createRequest = new CreateThesisOfferRequest
        {
            Title = "",
            Description = "Description",
            SubjectAreaId = Guid.NewGuid(),
            TutorId = Guid.NewGuid(),
            MaxStudents = 0,
            ExpiresAt = DateTime.UtcNow.AddDays(-1)
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/thesisoffer", createRequest);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }
}
