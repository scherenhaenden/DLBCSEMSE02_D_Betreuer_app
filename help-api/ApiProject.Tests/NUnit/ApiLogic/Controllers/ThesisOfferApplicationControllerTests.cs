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
public class ThesisOfferApplicationControllerTests
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
                        options.UseSqlite("Data Source=ThesisOfferApplicationControllerTests.db");
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
    public async Task GetApplicationsForUser_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/api/thesisofferapplication?page=1&pageSize=10");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task CreateApplication_ValidData_ReturnsCreated()
    {
        // Arrange
        var offer = _context.ThesisOffers.FirstOrDefault();
        var student = _context.Users.FirstOrDefault(u => u.UserRoles.Any(ur => ur.Role.Name == Roles.Student));
        if (offer == null || student == null) Assert.Ignore("No offers or students seeded");

        var createRequest = new CreateThesisOfferApplicationRequest
        {
            ThesisOfferId = offer.Id,
            StudentId = student.Id,
            Message = "I am interested in this offer."
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/thesisofferapplication", createRequest);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }

    [Test]
    public async Task CreateApplication_InvalidData_ReturnsBadRequest()
    {
        // Arrange
        var createRequest = new CreateThesisOfferApplicationRequest
        {
            ThesisOfferId = Guid.NewGuid(),
            StudentId = Guid.NewGuid(),
            Message = ""
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/thesisofferapplication", createRequest);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }
}
