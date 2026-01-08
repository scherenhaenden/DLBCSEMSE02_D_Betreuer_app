using ApiProject.BusinessLogic.Services;
using ApiProject.DatabaseAccess.Context;
using Microsoft.EntityFrameworkCore;
using ApiProject.BusinessLogic.Models;
using System.Collections.Generic;
using ApiProject.DatabaseAccess.Entities;

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

    [Test]
    public async Task SearchThreeDifferentSubjectAreasByContainingAndShouldGiveMeBackAllTheSubjectsContainingSimilarNamesShouldPass()
    {
        // Arrange: Seed 20 different subject areas
        var subjectAreasToAdd = new List<SubjectAreaDataAccessModel>
        {
            new SubjectAreaDataAccessModel { Id = Guid.NewGuid(), Title = "Computer Science", Description = "Study of computers" },
            new SubjectAreaDataAccessModel { Id = Guid.NewGuid(), Title = "Computer Engineering", Description = "Engineering with computers" },
            new SubjectAreaDataAccessModel { Id = Guid.NewGuid(), Title = "Computer Applications", Description = "Applications of computers" },
            new SubjectAreaDataAccessModel { Id = Guid.NewGuid(), Title = "Mathematics", Description = "Study of numbers" },
            new SubjectAreaDataAccessModel { Id = Guid.NewGuid(), Title = "Physics", Description = "Study of matter" },
            new SubjectAreaDataAccessModel { Id = Guid.NewGuid(), Title = "Chemistry", Description = "Study of substances" },
            new SubjectAreaDataAccessModel { Id = Guid.NewGuid(), Title = "Biology", Description = "Study of life" },
            new SubjectAreaDataAccessModel { Id = Guid.NewGuid(), Title = "History", Description = "Study of past events" },
            new SubjectAreaDataAccessModel { Id = Guid.NewGuid(), Title = "Geography", Description = "Study of earth" },
            new SubjectAreaDataAccessModel { Id = Guid.NewGuid(), Title = "Literature", Description = "Study of written works" },
            new SubjectAreaDataAccessModel { Id = Guid.NewGuid(), Title = "Philosophy", Description = "Study of knowledge" },
            new SubjectAreaDataAccessModel { Id = Guid.NewGuid(), Title = "Psychology", Description = "Study of mind" },
            new SubjectAreaDataAccessModel { Id = Guid.NewGuid(), Title = "Sociology", Description = "Study of society" },
            new SubjectAreaDataAccessModel { Id = Guid.NewGuid(), Title = "Economics", Description = "Study of economy" },
            new SubjectAreaDataAccessModel { Id = Guid.NewGuid(), Title = "Political Science", Description = "Study of politics" },
            new SubjectAreaDataAccessModel { Id = Guid.NewGuid(), Title = "Anthropology", Description = "Study of humans" },
            new SubjectAreaDataAccessModel { Id = Guid.NewGuid(), Title = "Linguistics", Description = "Study of language" },
            new SubjectAreaDataAccessModel { Id = Guid.NewGuid(), Title = "Art History", Description = "Study of art" },
            new SubjectAreaDataAccessModel { Id = Guid.NewGuid(), Title = "Music Theory", Description = "Study of music" },
            new SubjectAreaDataAccessModel { Id = Guid.NewGuid(), Title = "Theater Studies", Description = "Study of theater" }
        };

        _context.SubjectAreas.AddRange(subjectAreasToAdd);
        await _context.SaveChangesAsync();

        // Act: Search for "Computer" which should match three subject areas
        var result = await _subjectAreaService.SearchAsync("Computer", 1, 10);

        // Assert: Should return all three subject areas containing "Computer"
        Assert.That(result.Items.Count, Is.AtLeast(3));
        var titles = result.Items.Select(sa => sa.Title).ToList();
        Assert.That(titles, Does.Contain("Computer Science"));
        Assert.That(titles, Does.Contain("Computer Engineering"));
        Assert.That(titles, Does.Contain("Computer Applications"));
        Assert.That(result.TotalCount, Is.AtLeast(3));
    }
}
