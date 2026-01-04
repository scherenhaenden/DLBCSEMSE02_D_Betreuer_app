using ApiProject.BusinessLogic.Services;
using ApiProject.DatabaseAccess.Context;
using Microsoft.EntityFrameworkCore;
using ApiProject.BusinessLogic.Models;

namespace ApiProject.Tests.NUnit.BusinessLogic.Services;

[TestFixture]
public class SubjectAreaBusinessLogicServiceTests
{
    private ThesisDbContext _context;
    private ISubjectAreaBusinessLogicService _subjectAreaService;
    private TestDataSeeder _seeder;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<ThesisDbContext>()
            .UseSqlite("Data Source=SubjectAreaBusinessLogicServiceTests.db")
            .Options;
        _context = new ThesisDbContext(options);
        _context.Database.EnsureCreated();

        _seeder = new TestDataSeeder(_context);
        _seeder.SeedSubjectAreas();
        _context.SaveChanges();

        var userService = new UserBusinessLogicService(_context);
        _subjectAreaService = new SubjectAreaBusinessLogicService(_context, userService);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task CanGetAllSubjectAreas()
    {
        // Act
        var result = await _subjectAreaService.GetAllAsync(1, 10);

        // Assert
        Assert.That(result.Items.Count, Is.GreaterThan(0));
        Assert.That(result.Items.First().Title, Is.Not.Null);
    }

    [Test]
    public async Task CanGetSubjectAreaById()
    {
        // Arrange
        var subjectArea = _context.SubjectAreas.First();

        // Act
        var retrieved = await _subjectAreaService.GetByIdAsync(subjectArea.Id);

        // Assert
        Assert.That(retrieved, Is.Not.Null);
        Assert.That(retrieved.Title, Is.EqualTo(subjectArea.Title));
    }

    [Test]
    public async Task CanCreateSubjectArea()
    {
        // Arrange
        var request = new SubjectAreaCreateRequestBusinessLogicModel
        {
            Title = "New Subject Area",
            Description = "Description of new subject area"
        };

        // Act
        var created = await _subjectAreaService.CreateSubjectAreaAsync(request);

        // Assert
        Assert.That(created, Is.Not.Null);
        Assert.That(created.Title, Is.EqualTo("New Subject Area"));
        Assert.That(created.Description, Is.EqualTo("Description of new subject area"));
    }

    [Test]
    public async Task CanUpdateSubjectArea()
    {
        // Arrange
        var subjectArea = _context.SubjectAreas.First();
        var updateRequest = new SubjectAreaUpdateRequestBusinessLogicModel
        {
            Title = "Updated Title",
            Description = "Updated Description"
        };

        // Act
        var updated = await _subjectAreaService.UpdateSubjectAreaAsync(subjectArea.Id, updateRequest);

        // Assert
        Assert.That(updated.Title, Is.EqualTo("Updated Title"));
        Assert.That(updated.Description, Is.EqualTo("Updated Description"));
    }

    [Test]
    public async Task CanDeleteSubjectArea()
    {
        // Arrange
        var subjectArea = _context.SubjectAreas.First();

        // Act
        var deleted = await _subjectAreaService.DeleteSubjectAreaAsync(subjectArea.Id);

        // Assert
        Assert.That(deleted, Is.True);
        var retrieved = await _subjectAreaService.GetByIdAsync(subjectArea.Id);
        Assert.That(retrieved, Is.Null);
    }

    [Test]
    public async Task CanSearchSubjectAreas()
    {
        // Arrange
        var subjectArea = _context.SubjectAreas.First();

        // Act
        var result = await _subjectAreaService.SearchAsync(subjectArea.Title, 1, 10);

        // Assert
        Assert.That(result.Items.Count, Is.GreaterThan(0));
        Assert.That(result.Items.First().Title, Is.EqualTo(subjectArea.Title));
    }

    [Test]
    public async Task CreateSubjectAreaWithEmptyTitleShouldNotPass()
    {
        // Arrange
        var request = new SubjectAreaCreateRequestBusinessLogicModel
        {
            Title = "",
            Description = "Description"
        };

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await _subjectAreaService.CreateSubjectAreaAsync(request));
        Assert.That(ex.Message, Does.Contain("Title"));
    }

    [Test]
    public async Task CreateSubjectAreaWithNullTitleShouldNotPass()
    {
        // Arrange
        var request = new SubjectAreaCreateRequestBusinessLogicModel
        {
            Title = null,
            Description = "Description"
        };

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await _subjectAreaService.CreateSubjectAreaAsync(request));
        Assert.That(ex.Message, Does.Contain("Title"));
    }

    [Test]
    public async Task UpdateSubjectAreaWithEmptyTitleShouldNotPass()
    {
        // Arrange
        var subjectArea = _context.SubjectAreas.First();
        var updateRequest = new SubjectAreaUpdateRequestBusinessLogicModel
        {
            Title = "",
            Description = "Updated Description"
        };

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await _subjectAreaService.UpdateSubjectAreaAsync(subjectArea.Id, updateRequest));
        Assert.That(ex.Message, Does.Contain("Title"));
    }

    [Test]
    public async Task UpdateNonExistentSubjectAreaShouldNotPass()
    {
        // Arrange
        var updateRequest = new SubjectAreaUpdateRequestBusinessLogicModel
        {
            Title = "Updated Title",
            Description = "Updated Description"
        };

        // Act & Assert
        var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            await _subjectAreaService.UpdateSubjectAreaAsync(Guid.NewGuid(), updateRequest));
    }

    [Test]
    public async Task DeleteNonExistentSubjectAreaShouldNotPass()
    {
        // Act
        var result = await _subjectAreaService.DeleteSubjectAreaAsync(Guid.NewGuid());

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task SearchSubjectAreasWithEmptyTermShouldReturnAll()
    {
        // Act
        var result = await _subjectAreaService.SearchAsync("", 1, 10);

        // Assert
        Assert.That(result.Items.Count, Is.GreaterThan(0));
    }

    [Test]
    public async Task SearchSubjectAreasWithNonMatchingTermShouldReturnEmpty()
    {
        // Act
        var result = await _subjectAreaService.SearchAsync("NonExistentTerm12345", 1, 10);

        // Assert
        Assert.That(result.Items.Count, Is.EqualTo(0));
    }

    [Test]
    public async Task GetSubjectAreaByIdNonExistentShouldReturnNull()
    {
        // Act
        var result = await _subjectAreaService.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetAllSubjectAreasWithPaginationShouldWork()
    {
        // Act
        var result = await _subjectAreaService.GetAllAsync(1, 1);

        // Assert
        Assert.That(result.Items.Count, Is.EqualTo(1));
        Assert.That(result.TotalCount, Is.GreaterThan(0));
    }
}
