using System.Net.Http.Json;
using ApiProject.ApiLogic.Models;
using ApiProject.DatabaseAccess.Context;
using ApiProject.DatabaseAccess.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace ApiProject.Tests.NUnit.ApiLogic.Controllers;

[TestFixture]
public class UserControllerTests
{
    private WebApplicationFactory<Program> _factory;
    private HttpClient _client;

    [SetUp]
    public void SetUp()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove all existing DbContext registrations
                    var descriptors = services.Where(d => d.ServiceType.FullName.Contains("ThesisDbContext") || d.ServiceType.FullName.Contains("DbContextOptions")).ToList();
                    foreach (var d in descriptors)
                    {
                        services.Remove(d);
                    }

                    // Add SQLite DbContext for testing
                    services.AddDbContext<ThesisDbContext>(options =>
                        options.UseSqlite("Data Source=TestUserController.db"));
                });
            });

        _client = _factory.CreateClient();

        // Seed the database with test data
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ThesisDbContext>();
            db.Database.EnsureCreated();

            // Seed roles if not exist
            if (!db.Roles.Any())
            {
                db.Roles.Add(new RoleDataAccessModel { Id = Guid.NewGuid(), Name = "STUDENT" });
                db.Roles.Add(new RoleDataAccessModel { Id = Guid.NewGuid(), Name = "TUTOR" });
                db.SaveChanges();
            }

            // Seed a test user
            if (!db.Users.Any(u => u.Email == "test@example.com"))
            {
                var user = new UserDataAccessModel
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Test",
                    LastName = "User",
                    Email = "test@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("password")
                };
                db.Users.Add(user);

                // Add role
                var role = db.Roles.First(r => r.Name == "STUDENT");
                db.UserRoles.Add(new UserRoleDataAccessModel { UserId = user.Id, RoleId = role.Id });

                db.SaveChanges();
            }
        }
    }

    [TearDown]
    public void TearDown()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ThesisDbContext>();
            db.Database.EnsureDeleted();
        }

        _client.Dispose();
        _factory.Dispose();
    }

    [Test]
    public async Task GetUsers_ReturnsOkAndList()
    {
        // Act
        var response = await _client.GetAsync("/users");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<PaginatedResponse<UserResponse>>();
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Items.Count, Is.GreaterThanOrEqualTo(1));
        Assert.That(result.Items.Any(u => u.Email == "test@example.com"), Is.True);
    }

    [Test]
    public async Task GetUserById_ReturnsOkAndUser()
    {
        // Arrange
        Guid userId;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ThesisDbContext>();
            userId = db.Users.First(u => u.Email == "test@example.com").Id;
        }

        // Act
        var response = await _client.GetAsync($"/users/{userId}");

        // Assert
        response.EnsureSuccessStatusCode();
        var user = await response.Content.ReadFromJsonAsync<UserResponse>();
        Assert.That(user, Is.Not.Null);
        Assert.That(user.Email, Is.EqualTo("test@example.com"));
        Assert.That(user.FirstName, Is.EqualTo("Test"));
    }

    [Test]
    public async Task GetUserById_ReturnsNotFound_ForInvalidId()
    {
        // Act
        var response = await _client.GetAsync($"/users/{Guid.NewGuid()}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NotFound));
    }

    [Test]
    public async Task CreateUser_ReturnsCreatedAndUser()
    {
        // Arrange
        var createRequest = new CreateUserRequest
        {
            FirstName = "New",
            LastName = "User",
            Email = "new@example.com",
            Password = "newpassword",
            Roles = new List<string> { "STUDENT" }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/users", createRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        var user = await response.Content.ReadFromJsonAsync<UserResponse>();
        Assert.That(user, Is.Not.Null);
        Assert.That(user.Email, Is.EqualTo("new@example.com"));
        Assert.That(user.FirstName, Is.EqualTo("New"));
    }

    [Test]
    public async Task CreateUser_ReturnsBadRequest_ForDuplicateEmail()
    {
        // Arrange
        var createRequest = new CreateUserRequest
        {
            FirstName = "Duplicate",
            LastName = "User",
            Email = "test@example.com", // Existing email
            Password = "password",
            Roles = new List<string> { "STUDENT" }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/users", createRequest);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Login_ReturnsOk_ForValidCredentials()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "test@example.com",
            Password = "password"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/auth/login", loginRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.That(loginResponse, Is.Not.Null);
        Assert.That(loginResponse.Token, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public async Task Login_ReturnsUnauthorized_ForInvalidCredentials()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "test@example.com",
            Password = "wrongpassword"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/auth/login", loginRequest);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.Unauthorized));
    }
}