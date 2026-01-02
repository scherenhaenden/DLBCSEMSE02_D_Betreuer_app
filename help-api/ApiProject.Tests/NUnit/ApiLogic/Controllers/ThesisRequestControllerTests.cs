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
public class ThesisRequestControllerTests
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
                        options.UseSqlite("Data Source=ThesisRequestControllerTests.db");
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
    public async Task GetRequestsForUser_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/api/thesisrequest?page=1&pageSize=10");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task GetRequestById_ExistingId_ReturnsOk()
    {
        // Arrange
        var request = _context.ThesisRequests.FirstOrDefault();
        if (request == null) Assert.Ignore("No requests seeded");

        // Act
        var response = await _client.GetAsync($"/api/thesisrequest/{request.Id}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task GetRequestById_NonExistingId_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync($"/api/thesisrequest/{Guid.NewGuid()}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task CreateRequest_ValidData_ReturnsCreated()
    {
        // Arrange
        var student = _context.Users.FirstOrDefault(u => u.UserRoles.Any(ur => ur.Role.Name == Roles.Student));
        var tutor = _context.Users.FirstOrDefault(u => u.UserRoles.Any(ur => ur.Role.Name == Roles.Tutor));
        var thesis = _context.Theses.FirstOrDefault();
        if (student == null || tutor == null || thesis == null) Assert.Ignore("No users or theses seeded");

        var createRequest = new CreateThesisRequestRequest
        {
            ReceiverId = tutor.Id,
            ThesisId = thesis.Id,
            RequestType = "SUPERVISION",
            Message = "Please supervise my thesis."
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/thesisrequest", createRequest);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }

    [Test]
    public async Task CreateRequest_InvalidData_ReturnsBadRequest()
    {
        // Arrange
        var createRequest = new CreateThesisRequestRequest
        {
            ReceiverId = Guid.NewGuid(),
            ThesisId = Guid.NewGuid(),
            RequestType = "",
            Message = ""
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/thesisrequest", createRequest);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task RespondToRequest_ValidData_ReturnsOk()
    {
        // Arrange
        var requestEntity = _context.ThesisRequests.FirstOrDefault();
        if (requestEntity == null) Assert.Ignore("No requests seeded");

        var respondRequest = new RespondToThesisRequestRequest
        {
            Accepted = true,
            Message = "Accepted"
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/thesisrequest/{requestEntity.Id}/respond", respondRequest);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task RespondToRequest_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var respondRequest = new RespondToThesisRequestRequest
        {
            Accepted = false,
            Message = "Rejected"
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/thesisrequest/{Guid.NewGuid()}/respond", respondRequest);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}
