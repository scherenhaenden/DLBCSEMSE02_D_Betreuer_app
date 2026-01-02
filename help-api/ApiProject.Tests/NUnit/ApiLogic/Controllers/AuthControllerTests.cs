using ApiProject.ApiLogic.Controllers;
using ApiProject.DatabaseAccess.Context;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ApiProject.ApiLogic.Models;
using ApiProject.Constants;

namespace ApiProject.Tests.NUnit.ApiLogic.Controllers;

[TestFixture]
public class AuthControllerTests
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
                        options.UseSqlite("Data Source=AuthControllerTests.db");
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
    public async Task Login_ValidCredentials_ReturnsOk()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "test@example.com",
            Password = "password"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "invalid@example.com",
            Password = "wrongpassword"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task Register_ValidData_ReturnsCreated()
    {
        // Arrange
        var registerRequest = new CreateUserRequest
        {
            FirstName = "New",
            LastName = "User",
            Email = "newuser@example.com",
            Password = "password123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }

    [Test]
    public async Task Register_InvalidData_ReturnsBadRequest()
    {
        // Arrange
        var registerRequest = new CreateUserRequest
        {
            FirstName = "",
            LastName = "User",
            Email = "invalidemail",
            Password = "pass"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }
}
