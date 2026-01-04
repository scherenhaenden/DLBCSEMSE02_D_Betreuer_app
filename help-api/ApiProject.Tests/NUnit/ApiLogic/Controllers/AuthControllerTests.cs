using ApiProject.DatabaseAccess.Context;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using ApiProject.ApiLogic.Models;
using Microsoft.Extensions.Configuration;

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
                builder.ConfigureAppConfiguration((context, config) =>
                {
                    config.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        ["Database:SeedJsonPath"] = null
                    });
                });
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

        // Seed data using Seeder
        var seeder = new TestDataSeeder(_context);
        seeder.SeedRoles();


        var gh = _context.Users.ToList();
        var abr = gh.First(u => u.Email == "abraham.student@test.com");
        var fhh = gh.Count;

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
    public async Task Login_MissingPassword_ReturnsOkRequest()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "abraham.student@test.com",
            Password = "password123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/auth/login", loginRequest);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task Login_MissingPassword_ReturnsBadRequest()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "abraham.student@test.com",
            Password = ""
        };

        // Act
        var response = await _client.PostAsJsonAsync("/auth/login", loginRequest);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
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
        var response = await _client.PostAsJsonAsync("/auth/login", loginRequest);

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
        var response = await _client.PostAsJsonAsync("/auth/register", registerRequest);

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
        var response = await _client.PostAsJsonAsync("/auth/register", registerRequest);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }
}
