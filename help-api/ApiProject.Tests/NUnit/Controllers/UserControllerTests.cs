using ApiProject.Api.Controllers;
using ApiProject.Api.Models;
using ApiProject.Db.Entities;
using ApiProject.Logic.Models;
using ApiProject.Logic.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ApiProject.Tests.NUnit.Controllers;

[TestFixture]
public class UserControllerTests
{
    private UserController _controller = null!;
    private Mock<IUserService> _mockUserService = null!;

    [SetUp]
    public void Setup()
    {
        _mockUserService = new Mock<IUserService>();
        _controller = new UserController(_mockUserService.Object);
    }

    [Test]
    public async Task GetAll_ReturnsOkWithPaginatedResponse()
    {
        // Arrange
        var users = new List<User>
        {
            new User { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", Email = "john@example.com", PasswordHash = "hash1", Salt = "salt1" },
            new User { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith", Email = "jane@example.com", PasswordHash = "hash2", Salt = "salt2" }
        };
        var paginatedResult = new PaginatedResult<User> { Items = users, TotalCount = 2, Page = 1, PageSize = 10 };
        _mockUserService.Setup(s => s.GetAllAsync(1, 10)).ReturnsAsync(paginatedResult);

        // Act
        var result = await _controller.GetAll(1, 10);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        var response = okResult!.Value as PaginatedResponse<UserResponse>;
        Assert.That(response!.Items.Count, Is.EqualTo(2));
        Assert.That(response.TotalCount, Is.EqualTo(2));
    }

    [Test]
    public async Task Create_ValidRequest_ReturnsCreated()
    {
        // Arrange
        var request = new CreateUserRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Password = "password",
            Roles = new List<string> { "STUDENT" }
        };
        var createdUser = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            PasswordHash = "hashedpassword",
            Salt = "salt",
            UserRoles = new List<UserRole>()
        };
        _mockUserService.Setup(s => s.CreateUserAsync("John", "Doe", "john@example.com", "password", It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(createdUser);

        // Act
        var result = await _controller.Create(request);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
        var createdResult = result.Result as CreatedAtActionResult;
        var response = createdResult!.Value as UserResponse;
        Assert.That(response!.FirstName, Is.EqualTo("John"));
    }

    [Test]
    public void Create_InvalidOperationException_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateUserRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "existing@example.com",
            Password = "hashedpassword",
            Roles = new List<string> { "STUDENT" }
        };
        _mockUserService.Setup(s => s.CreateUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>()))
            .ThrowsAsync(new InvalidOperationException("User already exists"));

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(async () => await _controller.Create(request));
    }
}
