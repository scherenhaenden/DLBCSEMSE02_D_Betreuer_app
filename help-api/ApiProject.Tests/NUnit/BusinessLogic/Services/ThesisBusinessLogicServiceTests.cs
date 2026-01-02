using ApiProject.BusinessLogic.Services;
using ApiProject.DatabaseAccess.Context;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ApiProject.BusinessLogic.Models;

namespace ApiProject.Tests.NUnit.BusinessLogic.Services;

[TestFixture]
public class ThesisBusinessLogicServiceTests
{
    private ThesisDbContext _context;
    private IThesisBusinessLogicService _thesisService;
    private IUserBusinessLogicService _userService;
    private TestDataSeeder _seeder;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<ThesisDbContext>()
            .UseSqlite("Data Source=TestThesisService.db")
            .Options;
        _context = new ThesisDbContext(options);
        _context.Database.EnsureCreated();

        _seeder = new TestDataSeeder(_context);
        _seeder.SeedRoles();
        _seeder.SeedBillingStatuses();
        _seeder.SeedThesisStatuses();
        _seeder.SeedSubjectAreas();

        _userService = new UserBusinessLogicService(_context);
        _thesisService = new ThesisBusinessLogicService(_context, _userService);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task CanGetAllTheses()
    {
        // Arrange
        var student = _seeder.SeedUser("John", "Doe", "john@example.com", "password", "STUDENT");
        var tutor = _seeder.SeedUser("Jane", "Smith", "jane@example.com", "password", "TUTOR");
        var subjectArea = _context.SubjectAreas.First();
        var status = _context.ThesisStatuses.First();
        var billingStatus = _context.BillingStatuses.First();

        var thesis1 = _seeder.SeedThesis("Thesis 1", student.Id, subjectArea.Id, status.Id, billingStatus.Id);
        var thesis2 = _seeder.SeedThesis("Thesis 2", student.Id, subjectArea.Id, status.Id, billingStatus.Id);

        // Act
        var result = await _thesisService.GetAllAsync(1, 10, student.Id, new List<string> { "STUDENT" });

        // Assert
        Assert.That(result.Items.Count, Is.EqualTo(2));
        Assert.That(result.Items.Any(t => t.Title == "Thesis 1"), Is.True);
        Assert.That(result.Items.Any(t => t.Title == "Thesis 2"), Is.True);
    }

    [Test]
    public async Task CanGetThesisById()
    {
        // Arrange
        var student = _seeder.SeedUser("Alice", "Wonder", "alice@example.com", "password", "STUDENT");
        var subjectArea = _context.SubjectAreas.First();
        var status = _context.ThesisStatuses.First();
        var billingStatus = _context.BillingStatuses.First();

        var thesis = _seeder.SeedThesis("Test Thesis", student.Id, subjectArea.Id, status.Id, billingStatus.Id);

        // Act
        var retrievedThesis = await _thesisService.GetByIdAsync(thesis.Id);

        // Assert
        Assert.That(retrievedThesis, Is.Not.Null);
        Assert.That(retrievedThesis.Title, Is.EqualTo("Test Thesis"));
        Assert.That(retrievedThesis.OwnerId, Is.EqualTo(student.Id));
    }

    [Test]
    public async Task CanCreateThesis()
    {
        // Arrange
        var student = _seeder.SeedUser("Bob", "Builder", "bob@example.com", "password", "STUDENT");
        var tutor = _seeder.SeedUser("Tutor", "One", "tutor@example.com", "password", "TUTOR");
        var subjectArea = _context.SubjectAreas.First();

        var request = new ThesisCreateRequestBusinessLogicModel
        {
            Title = "New Thesis",
            SubjectArea = subjectArea.Title,
            OwnerId = student.Id,
            TutorId = tutor.Id,
            SubjectAreaId = subjectArea.Id
        };

        // Act
        var createdThesis = await _thesisService.CreateThesisAsync(request);

        // Assert
        Assert.That(createdThesis, Is.Not.Null);
        Assert.That(createdThesis.Title, Is.EqualTo("New Thesis"));
        Assert.That(createdThesis.OwnerId, Is.EqualTo(student.Id));
        Assert.That(createdThesis.TutorId, Is.EqualTo(tutor.Id));
    }

    [Test]
    public async Task CanUpdateThesis()
    {
        // Arrange
        var student = _seeder.SeedUser("Charlie", "Brown", "charlie@example.com", "password", "STUDENT");
        var subjectArea = _context.SubjectAreas.First();
        var status = _context.ThesisStatuses.First();
        var billingStatus = _context.BillingStatuses.First();

        var thesis = _seeder.SeedThesis("Original Title", student.Id, subjectArea.Id, status.Id, billingStatus.Id);

        var updateRequest = new ThesisUpdateRequestBusinessLogicModel
        {
            Title = "Updated Title"
        };

        // Act
        var updatedThesis = await _thesisService.UpdateThesisAsync(thesis.Id, updateRequest);

        // Assert
        Assert.That(updatedThesis.Title, Is.EqualTo("Updated Title"));
    }

    [Test]
    public async Task CanDeleteThesis()
    {
        // Arrange
        var student = _seeder.SeedUser("Dave", "Jones", "dave@example.com", "password", "STUDENT");
        var subjectArea = _context.SubjectAreas.First();
        var status = _context.ThesisStatuses.First();
        var billingStatus = _context.BillingStatuses.First();

        var thesis = _seeder.SeedThesis("Thesis to Delete", student.Id, subjectArea.Id, status.Id, billingStatus.Id);

        // Act
        var deleted = await _thesisService.DeleteThesisAsync(thesis.Id);

        // Assert
        Assert.That(deleted, Is.True);
        var retrieved = await _thesisService.GetByIdAsync(thesis.Id);
        Assert.That(retrieved, Is.Null);
    }
}
