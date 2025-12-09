using ApiProject.BusinessLogic.Services;
using ApiProject.DatabaseAccess.Context;
using ApiProject.DatabaseAccess.Entities;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace ApiProject.Tests.NUnit.BusinessLogic.Services;

[TestFixture]
public class UserBusinessLogicServiceTests
{
    private ThesisDbContext _context;
    private IUserBusinessLogicService _userService;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<ThesisDbContext>()
            .UseSqlite("Data Source=TestUserService.db")
            .Options;
        _context = new ThesisDbContext(options);
        _context.Database.EnsureCreated();

        // Seed roles
        if (!_context.Roles.Any())
        {
            _context.Roles.AddRange(
                new RoleDataAccessModel { Id = Guid.NewGuid(), Name = "STUDENT" },
                new RoleDataAccessModel { Id = Guid.NewGuid(), Name = "TUTOR" },
                new RoleDataAccessModel { Id = Guid.NewGuid(), Name = "ADMIN" }
            );
            _context.SaveChanges();
        }

        _userService = new UserBusinessLogicService(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task CanGetAllUsers()
    {
        // Arrange
        var user1 = await _userService.CreateUserAsync("John", "Doe", "john@example.com", "password123", new[] { "STUDENT" });
        var user2 = await _userService.CreateUserAsync("Jane", "Smith", "jane@example.com", "password456", new[] { "TUTOR" });

        // Act
        var result = await _userService.GetAllAsync(1, 10);

        // Assert
        Assert.That(result.Items.Count, Is.EqualTo(2));
        Assert.That(result.Items.Any(u => u.Email == "john@example.com"), Is.True);
        Assert.That(result.Items.Any(u => u.Email == "jane@example.com"), Is.True);
    }

    [Test]
    public async Task CanGetUserById()
    {
        // Arrange
        var createdUser = await _userService.CreateUserAsync("Alice", "Wonder", "alice@example.com", "password", new[] { "STUDENT" });

        // Act
        var retrievedUser = await _userService.GetByIdAsync(createdUser.Id);

        // Assert
        Assert.That(retrievedUser, Is.Not.Null);
        Assert.That(retrievedUser.Email, Is.EqualTo("alice@example.com"));
        Assert.That(retrievedUser.FirstName, Is.EqualTo("Alice"));
    }

    [Test]
    public async Task CanGetUserByEmail()
    {
        // Arrange
        await _userService.CreateUserAsync("Bob", "Builder", "bob@example.com", "password", new[] { "TUTOR" });

        // Act
        var retrievedUser = await _userService.GetByEmailAsync("bob@example.com");

        // Assert
        Assert.That(retrievedUser, Is.Not.Null);
        Assert.That(retrievedUser.FirstName, Is.EqualTo("Bob"));
    }

    [Test]
    public async Task CanCreateUser()
    {
        // Act
        var createdUser = await _userService.CreateUserAsync("Charlie", "Brown", "charlie@example.com", "password", new[] { "STUDENT", "TUTOR" });

        // Assert
        Assert.That(createdUser, Is.Not.Null);
        Assert.That(createdUser.Email, Is.EqualTo("charlie@example.com"));
        Assert.That(createdUser.Roles.Count, Is.EqualTo(2));
        Assert.That(createdUser.Roles.Contains("STUDENT"), Is.True);
        Assert.That(createdUser.Roles.Contains("TUTOR"), Is.True);
    }

    [Test]
    public async Task CreateUser_FailsIfEmailExists()
    {
        // Arrange
        await _userService.CreateUserAsync("Dave", "Jones", "dave@example.com", "password", new[] { "STUDENT" });

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _userService.CreateUserAsync("Dave2", "Jones2", "dave@example.com", "password2", new[] { "TUTOR" }));
    }

    [Test]
    public async Task CanVerifyPassword()
    {
        // Arrange
        await _userService.CreateUserAsync("Eve", "Adams", "eve@example.com", "secret123", new[] { "ADMIN" });

        // Act
        var isValid = await _userService.VerifyPasswordAsync("eve@example.com", "secret123");
        var isInvalid = await _userService.VerifyPasswordAsync("eve@example.com", "wrongpassword");

        // Assert
        Assert.That(isValid, Is.True);
        Assert.That(isInvalid, Is.False);
    }

    [Test]
    public async Task CanCheckUserHasRole()
    {
        // Arrange
        var user = await _userService.CreateUserAsync("Frank", "Miller", "frank@example.com", "password", new[] { "STUDENT", "TUTOR" });

        // Act
        var hasStudent = await _userService.UserHasRoleAsync(user.Id, "STUDENT");
        var hasAdmin = await _userService.UserHasRoleAsync(user.Id, "ADMIN");
        var hasNonExistent = await _userService.UserHasRoleAsync(Guid.NewGuid(), "STUDENT");

        // Assert
        Assert.That(hasStudent, Is.True);
        Assert.That(hasAdmin, Is.False);
        Assert.That(hasNonExistent, Is.False);
    }

    [Test]
    public async Task GetAllUsers_WithFilters()
    {
        // Arrange
        await _userService.CreateUserAsync("Grace", "Hopper", "grace@example.com", "password", new[] { "TUTOR" });
        await _userService.CreateUserAsync("Hank", "Hill", "hank@example.com", "password", new[] { "STUDENT" });

        // Act
        var tutors = await _userService.GetAllAsync(1, 10, role: "TUTOR");
        var students = await _userService.GetAllAsync(1, 10, role: "STUDENT");

        // Assert
        Assert.That(tutors.Items.Count, Is.EqualTo(1));
        Assert.That(tutors.Items.First().Email, Is.EqualTo("grace@example.com"));
        Assert.That(students.Items.Count, Is.EqualTo(1));
        Assert.That(students.Items.First().Email, Is.EqualTo("hank@example.com"));
    }
}