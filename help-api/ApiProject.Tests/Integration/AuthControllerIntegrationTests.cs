using System.Net;
using System.Net.Http.Json;
using ApiProject.Api.Models;
using FluentAssertions;

namespace ApiProject.Tests.Integration;

[TestFixture]
public class AuthControllerIntegrationTests : IntegrationTestBase
{
    [Test]
    public async Task Login_ValidCredentials_ReturnsToken()
    {
        // Arrange
        var createRequest = new CreateUserRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Password = "password123",
            Roles = new List<string> { "STUDENT" }
        };

        await Client.PostAsJsonAsync("/users", createRequest);

        var loginRequest = new LoginRequest
        {
            Email = "john@example.com",
            Password = "password123"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
        loginResponse.Should().NotBeNull();
        loginResponse!.Token.Should().NotBeNullOrEmpty();
        loginResponse.User.Should().NotBeNull();
        loginResponse.User.FirstName.Should().Be("John");
        loginResponse.User.Email.Should().Be("john@example.com");
        loginResponse.User.Roles.Should().Contain("STUDENT");
    }

    [Test]
    public async Task Login_InvalidEmail_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "nonexistent@example.com",
            Password = "password123"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task Login_InvalidPassword_ReturnsUnauthorized()
    {
        // Arrange
        var createRequest = new CreateUserRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Password = "password123",
            Roles = new List<string> { "STUDENT" }
        };

        await Client.PostAsJsonAsync("/users", createRequest);

        var loginRequest = new LoginRequest
        {
            Email = "john@example.com",
            Password = "wrongpassword"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task Login_UserWithMultipleRoles_ReturnsAllRoles()
    {
        // Arrange
        var createRequest = new CreateUserRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Password = "password123",
            Roles = new List<string> { "STUDENT", "TUTOR" }
        };

        await Client.PostAsJsonAsync("/users", createRequest);

        var loginRequest = new LoginRequest
        {
            Email = "john@example.com",
            Password = "password123"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
        loginResponse.Should().NotBeNull();
        loginResponse!.User.Roles.Should().Contain("STUDENT");
        loginResponse.User.Roles.Should().Contain("TUTOR");
        loginResponse.User.Roles.Should().HaveCount(2);
    }
}
