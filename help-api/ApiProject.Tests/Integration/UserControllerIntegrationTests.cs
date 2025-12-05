using System.Net;
using System.Net.Http.Json;
using ApiProject.Api.Models;
using FluentAssertions;

namespace ApiProject.Tests.Integration;

[TestFixture]
public class UserControllerIntegrationTests : IntegrationTestBase
{
    [Test]
    public async Task CreateUser_ValidRequest_ReturnsCreated()
    {
        // Arrange
        var request = new CreateUserRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Password = "password123",
            Roles = new List<string> { "STUDENT" }
        };

        // Act
        var response = await Client.PostAsJsonAsync("/users", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdUser = await response.Content.ReadFromJsonAsync<UserResponse>();
        createdUser.Should().NotBeNull();
        createdUser!.FirstName.Should().Be("John");
        createdUser.LastName.Should().Be("Doe");
        createdUser.Email.Should().Be("john@example.com");
        createdUser.Roles.Should().Contain("STUDENT");
    }

    [Test]
    public async Task CreateUser_DuplicateEmail_ReturnsBadRequest()
    {
        // Arrange
        var request1 = new CreateUserRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Password = "password123",
            Roles = new List<string> { "STUDENT" }
        };
        var request2 = new CreateUserRequest
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "john@example.com", // Same email
            Password = "password456",
            Roles = new List<string> { "TUTOR" }
        };

        // Act
        var response1 = await Client.PostAsJsonAsync("/users", request1);
        response1.StatusCode.Should().Be(HttpStatusCode.Created);

        var response = await Client.PostAsJsonAsync("/users", request2); // Try to create duplicate

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task GetUsers_ReturnsPaginatedResponse()
    {
        // Arrange
        var request1 = new CreateUserRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Password = "password123",
            Roles = new List<string> { "STUDENT" }
        };
        var request2 = new CreateUserRequest
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane@example.com",
            Password = "password456",
            Roles = new List<string> { "TUTOR" }
        };

        await Client.PostAsJsonAsync("/users", request1);
        await Client.PostAsJsonAsync("/users", request2);

        // Act
        var response = await Client.GetAsync("/users?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PaginatedResponse<UserResponse>>();
        result.Should().NotBeNull();
        result!.Items.Should().HaveCountGreaterThanOrEqualTo(2);
        result.TotalCount.Should().BeGreaterThanOrEqualTo(2);
    }

    [Test]
    public async Task GetUsers_FilterByRole_ReturnsFilteredResults()
    {
        // Arrange
        var request1 = new CreateUserRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Password = "password123",
            Roles = new List<string> { "STUDENT" }
        };
        var request2 = new CreateUserRequest
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane@example.com",
            Password = "password456",
            Roles = new List<string> { "TUTOR" }
        };

        await Client.PostAsJsonAsync("/users", request1);
        await Client.PostAsJsonAsync("/users", request2);

        // Act
        var response = await Client.GetAsync("/users?role=STUDENT");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PaginatedResponse<UserResponse>>();
        result.Should().NotBeNull();
        result!.Items.Should().Contain(u => u.Roles.Contains("STUDENT"));
        result.Items.Should().NotContain(u => u.Roles.Contains("TUTOR"));
    }

    [Test]
    public async Task GetUsers_FilterByEmail_ReturnsFilteredResults()
    {
        // Arrange
        var request1 = new CreateUserRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Password = "password123",
            Roles = new List<string> { "STUDENT" }
        };
        var request2 = new CreateUserRequest
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane@example.com",
            Password = "password456",
            Roles = new List<string> { "TUTOR" }
        };

        await Client.PostAsJsonAsync("/users", request1);
        await Client.PostAsJsonAsync("/users", request2);

        // Act
        var response = await Client.GetAsync("/users?email=john");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PaginatedResponse<UserResponse>>();
        result.Should().NotBeNull();
        result!.Items.Should().Contain(u => u.Email.Contains("john"));
        result.Items.Should().NotContain(u => u.Email.Contains("jane"));
    }
}
