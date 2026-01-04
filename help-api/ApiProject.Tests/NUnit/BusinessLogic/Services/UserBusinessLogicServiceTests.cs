using ApiProject.BusinessLogic.Services;
using ApiProject.Constants;
using ApiProject.DatabaseAccess.Context;
using Microsoft.EntityFrameworkCore;

namespace ApiProject.Tests.NUnit.BusinessLogic.Services;

[TestFixture]
public class UserBusinessLogicServiceTests
{
    private ThesisDbContext _context;
    private IUserBusinessLogicService _userService;
    private TestDataSeeder _seeder;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<ThesisDbContext>()
            .UseSqlite("Data Source=TestUserService.db")
            .Options;
        _context = new ThesisDbContext(options);
        _context.Database.EnsureCreated();

        _seeder = new TestDataSeeder(_context);
        _seeder.SeedRoles();

        _userService = new UserBusinessLogicService(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    /// <summary>
    /// Tests the ability to retrieve all users from the database.
    /// This ensures the GetAllAsync method works correctly for basic pagination.
    /// Why: Verifies that the service can fetch users without filters.
    /// How: Creates two users, calls GetAllAsync with page 1 and size 10, asserts the count and presence of both users.
    /// </summary>
    [Test]
    public async Task CanGetAllUsers()
    {
        // Arrange
        var user1 = await _userService.CreateUserAsync("John", "Doe", "john@example.com", "password123", new[] { Roles.Student });
        var user2 = await _userService.CreateUserAsync("Jane", "Smith", "jane@example.com", "password456", new[] { Roles.Tutor });

        // Act
        var result = await _userService.GetAllAsync(1, 10);

        // Assert
        Assert.That(result.Items.Count, Is.EqualTo(2));
        Assert.That(result.Items.Any(u => u.Email == "john@example.com"), Is.True);
        Assert.That(result.Items.Any(u => u.Email == "jane@example.com"), Is.True);
    }

    /// <summary>
    /// Tests retrieving a user by their unique ID.
    /// Why: Ensures the GetByIdAsync method correctly fetches a specific user.
    /// How: Creates a user, retrieves it by ID, and verifies the details match.
    /// </summary>
    [Test]
    public async Task CanGetUserById()
    {
        // Arrange
        var createdUser = await _userService.CreateUserAsync("Alice", "Wonder", "alice@example.com", "password", new[] { Roles.Student });

        // Act
        var retrievedUser = await _userService.GetByIdAsync(createdUser.Id);

        // Assert
        Assert.That(retrievedUser, Is.Not.Null);
        Assert.That(retrievedUser.Email, Is.EqualTo("alice@example.com"));
        Assert.That(retrievedUser.FirstName, Is.EqualTo("Alice"));
    }

    /// <summary>
    /// Tests retrieving a user by their email address.
    /// Why: Ensures the GetByEmailAsync method works for authentication and user lookup.
    /// How: Creates a user, retrieves it by email, and checks the first name matches.
    /// </summary>
    [Test]
    public async Task CanGetUserByEmail()
    {
        // Arrange
        await _userService.CreateUserAsync("Bob", "Builder", "bob@example.com", "password", new[] { Roles.Tutor });

        // Act
        var retrievedUser = await _userService.GetByEmailAsync("bob@example.com");

        // Assert
        Assert.That(retrievedUser, Is.Not.Null);
        Assert.That(retrievedUser.FirstName, Is.EqualTo("Bob"));
    }

    /// <summary>
    /// Tests the creation of a new user with multiple roles.
    /// Why: Ensures the CreateUserAsync method handles user creation and role assignment correctly.
    /// How: Calls CreateUserAsync with two roles, verifies the user is created and has the correct roles.
    /// </summary>
    [Test]
    public async Task CanCreateUser()
    {
        // Act
        var createdUser = await _userService.CreateUserAsync("Charlie", "Brown", "charlie@example.com", "password", new[] { Roles.Student, Roles.Tutor });

        // Assert
        Assert.That(createdUser, Is.Not.Null);
        Assert.That(createdUser.Email, Is.EqualTo("charlie@example.com"));
        Assert.That(createdUser.Roles.Count, Is.EqualTo(2));
        Assert.That(createdUser.Roles.Contains(Roles.Student), Is.True);
        Assert.That(createdUser.Roles.Contains(Roles.Tutor), Is.True);
    }

    /// <summary>
    /// Tests that creating a user with an existing email throws an exception.
    /// Why: Ensures email uniqueness is enforced to prevent duplicate accounts.
    /// How: Creates a user, attempts to create another with the same email, expects InvalidOperationException.
    /// </summary>
    [Test]
    public async Task CreateUser_FailsIfEmailExists()
    {
        // Arrange
        await _userService.CreateUserAsync("Dave", "Jones", "dave@example.com", "password", new[] { Roles.Student });

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _userService.CreateUserAsync("Dave2", "Jones2", "dave@example.com", "password2", new[] { Roles.Tutor }));
    }

    /// <summary>
    /// Tests password verification for user authentication.
    /// Why: Ensures the VerifyPasswordAsync method correctly validates passwords for login.
    /// How: Creates a user, verifies with correct and incorrect passwords, checks true/false results.
    /// </summary>
    [Test]
    public async Task CanVerifyPassword()
    {
        // Arrange
        await _userService.CreateUserAsync("Eve", "Adams", "eve@example.com", "secret123", new[] { Roles.Admin });

        // Act
        var isValid = await _userService.VerifyPasswordAsync("eve@example.com", "secret123");
        var isInvalid = await _userService.VerifyPasswordAsync("eve@example.com", "wrongpassword");

        // Assert
        Assert.That(isValid, Is.True);
        Assert.That(isInvalid, Is.False);
    }

    /// <summary>
    /// Tests checking if a user has a specific role.
    /// Why: Ensures the UserHasRoleAsync method correctly determines role membership for authorization.
    /// How: Creates a user with roles, checks for existing and non-existing roles, including a non-existent user.
    /// </summary>
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

    /// <summary>
    /// Tests filtering users by role in the GetAllAsync method.
    /// Why: Ensures role-based filtering works for user management and access control.
    /// How: Creates users with different roles, filters by each role, verifies only matching users are returned.
    /// </summary>
    [Test]
    public async Task GetAllUsers_WithFilters()
    {
        // Arrange
        await _userService.CreateUserAsync("Grace", "Hopper", "grace@example.com", "password", new[] { Roles.Tutor });
        await _userService.CreateUserAsync("Hank", "Hill", "hank@example.com", "password", new[] { Roles.Student });
        await _userService.CreateUserAsync("Ian", "Admin", "ian@example.com", "password", new[] { Roles.Admin });

        // Act
        var tutors = await _userService.GetAllAsync(1, 10, role: Roles.Tutor);
        var students = await _userService.GetAllAsync(1, 10, role: Roles.Student);
        var admins = await _userService.GetAllAsync(1, 10, role: Roles.Admin);

        // Assert
        Assert.That(tutors.Items.Count, Is.EqualTo(1));
        Assert.That(tutors.Items.First().Email, Is.EqualTo("grace@example.com"));
        Assert.That(students.Items.Count, Is.EqualTo(1));
        Assert.That(students.Items.First().Email, Is.EqualTo("hank@example.com"));
        Assert.That(admins.Items.Count, Is.EqualTo(1));
        Assert.That(admins.Items.First().Email, Is.EqualTo("ian@example.com"));
    }

    /// <summary>
    /// Tests retrieving a paginated list of tutors.
    /// Why: Ensures the GetTutorsAsync method correctly filters and returns only users with TUTOR role.
    /// How: Creates tutors and a student, calls GetTutorsAsync, verifies only tutors are returned.
    /// </summary>
    [Test]
    public async Task CanGetTutors()
    {
        // Arrange
        var tutor1 = await _userService.CreateUserAsync("Tutor1", "One", "tutor1@example.com", "password", new[] { Roles.Tutor });
        var tutor2 = await _userService.CreateUserAsync("Tutor2", "Two", "tutor2@example.com", "password", new[] { Roles.Tutor });
        await _userService.CreateUserAsync("Student1", "One", "student1@example.com", "password", new[] { Roles.Student });

        // Act
        var tutors = await _userService.GetTutorsAsync(null, null, null, 1, 10);

        // Assert
        Assert.That(tutors.Items.Count, Is.EqualTo(2));
        Assert.That(tutors.Items.Any(t => t.Email == "tutor1@example.com"), Is.True);
        Assert.That(tutors.Items.Any(t => t.Email == "tutor2@example.com"), Is.True);
    }

    /// <summary>
    /// Tests retrieving a specific tutor by their ID.
    /// Why: Ensures the GetTutorByIdAsync method works for detailed tutor information retrieval.
    /// How: Creates a tutor, retrieves it by ID, verifies the details are correct.
    /// </summary>
    [Test]
    public async Task CanGetTutorById()
    {
        // Arrange
        var tutor = await _userService.CreateUserAsync("Tutor", "Test", "tutor@example.com", "password", new[] { Roles.Tutor });

        // Act
        var retrievedTutor = await _userService.GetTutorByIdAsync(tutor.Id);

        // Assert
        Assert.That(retrievedTutor, Is.Not.Null);
        Assert.That(retrievedTutor.Email, Is.EqualTo("tutor@example.com"));
        Assert.That(retrievedTutor.FirstName, Is.EqualTo("Tutor"));
    }
}