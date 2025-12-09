/*using ApiProject.DatabaseAccess.entities;
using ApiProject.Logic.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ApiProject.Tests.NUnit.Services;

[TestFixture]
public class UserServiceTests : TestBase
{
    private UserService _userService = null!;
    private Mock<IUserService> _mockUserService = null!;

    [SetUp]
    public void Setup()
    {
        var context = CreateInMemoryContext();
        _mockUserService = new Mock<IUserService>();
        _userService = new UserService(context);
    }

    [Test]
    public async Task GetAllAsync_ReturnsPaginatedResult()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var users = new List<User>
        {
            new User { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", Email = "john@example.com", PasswordHash = "hash1", Salt = "salt1" },
            new User { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith", Email = "jane@example.com", PasswordHash = "hash2", Salt = "salt2" }
        };
        context.Users.AddRange(users);
        await context.SaveChangesAsync();

        var service = new UserService(context);

        // Act
        var result = await service.GetAllAsync(1, 10);

        // Assert
        Assert.That(result.Items.Count, Is.EqualTo(2));
        Assert.That(result.TotalCount, Is.EqualTo(2));
        Assert.That(result.Page, Is.EqualTo(1));
        Assert.That(result.PageSize, Is.EqualTo(10));
    }

    [Test]
    public async Task GetByIdAsync_ExistingUser_ReturnsUser()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, FirstName = "John", LastName = "Doe", Email = "john@example.com", PasswordHash = "hash1", Salt = "salt1" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new UserService(context);

        // Act
        var result = await service.GetByIdAsync(userId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(userId));
    }

    [Test]
    public async Task GetByIdAsync_NonExistingUser_ReturnsNull()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new UserService(context);

        // Act
        var result = await service.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task CreateUserAsync_ValidData_CreatesUser()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new UserService(context);
        var role = new Role { Id = Guid.NewGuid(), Name = "STUDENT" };
        context.Roles.Add(role);
        await context.SaveChangesAsync();

        // Act
        var user = await service.CreateUserAsync("John", "Doe", "john@example.com", "password", new[] { "STUDENT" });

        // Assert
        Assert.That(user, Is.Not.Null);
        Assert.That(user.FirstName, Is.EqualTo("John"));
        Assert.That(user.LastName, Is.EqualTo("Doe"));
        Assert.That(user.Email, Is.EqualTo("john@example.com"));
        Assert.That(user.UserRoles.Count, Is.EqualTo(1));
    }

    [Test]
    public void CreateUserAsync_DuplicateEmail_ThrowsException()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new UserService(context);
        var existingUser = new User { Id = Guid.NewGuid(), FirstName = "Existing", LastName = "User", Email = "john@example.com", PasswordHash = "hash1", Salt = "salt1" };
        context.Users.Add(existingUser);
        context.SaveChanges();

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await service.CreateUserAsync("John", "Doe", "john@example.com", "password", new[] { "STUDENT" }));
    }

    [Test]
    public async Task UserHasRoleAsync_UserHasRole_ReturnsTrue()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new UserService(context);
        var userId = Guid.NewGuid();
        var role = new Role { Id = Guid.NewGuid(), Name = "STUDENT" };
        var user = new User { Id = userId, FirstName = "John", LastName = "Doe", Email = "john@example.com", PasswordHash = "hash1", Salt = "salt1" };
        var userRole = new UserRole { User = user, Role = role };
        user.UserRoles.Add(userRole);
        context.Users.Add(user);
        context.Roles.Add(role);
        await context.SaveChangesAsync();

        // Act
        var result = await service.UserHasRoleAsync(userId, "STUDENT");

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task UserHasRoleAsync_UserDoesNotHaveRole_ReturnsFalse()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new UserService(context);
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, FirstName = "John", LastName = "Doe", Email = "john@example.com", PasswordHash = "hash1", Salt = "salt1" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        var result = await service.UserHasRoleAsync(userId, "TUTOR");

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task EnsureRoleAsync_ExistingRole_ReturnsExisting()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new UserService(context);
        var role = new Role { Id = Guid.NewGuid(), Name = "STUDENT" };
        context.Roles.Add(role);
        await context.SaveChangesAsync();

        // Act
        var result = await service.EnsureRoleAsync("student");

        // Assert
        Assert.That(result.Name, Is.EqualTo("STUDENT"));
        Assert.That(result.Id, Is.EqualTo(role.Id));
    }

    [Test]
    public async Task EnsureRoleAsync_NewRole_CreatesAndReturns()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new UserService(context);

        // Act
        var result = await service.EnsureRoleAsync("tutor");

        // Assert
        Assert.That(result.Name, Is.EqualTo("TUTOR"));
        var dbRole = await context.Roles.SingleOrDefaultAsync(r => r.Name == "TUTOR");
        Assert.That(dbRole, Is.Not.Null);
    }
}*/
