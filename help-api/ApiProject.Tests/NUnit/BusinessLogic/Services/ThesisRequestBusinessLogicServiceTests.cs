using ApiProject.BusinessLogic.Services;
using ApiProject.DatabaseAccess.Context;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ApiProject.BusinessLogic.Models;
using ApiProject.Constants;
using ApiProject.DatabaseAccess.Entities;

namespace ApiProject.Tests.NUnit.BusinessLogic.Services;

[TestFixture]
public class ThesisRequestBusinessLogicServiceTests
{
    private ThesisDbContext _context;
    private IThesisRequestBusinessLogicService _thesisRequestService;
    private TestDataSeeder _seeder;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<ThesisDbContext>()
            .UseSqlite("Data Source=ThesisRequestBusinessLogicServiceTests.db")
            .Options;
        _context = new ThesisDbContext(options);
        _context.Database.EnsureCreated();

        _seeder = new TestDataSeeder(_context);
        _seeder.SeedRoles();
        _seeder.SeedRequestStatuses();
        _seeder.SeedSubjectAreas();
        _context.SaveChanges();

        _thesisRequestService = new ThesisRequestBusinessLogicService(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task CanGetAllThesisRequests()
    {
        // Arrange
        var student = _seeder.SeedUser("Student", "One", "student@example.com", "password", Roles.Student);
        var tutor = _seeder.SeedUser("Tutor", "One", "tutor@example.com", "password", Roles.Tutor);
        var subjectArea = _context.SubjectAreas.First();
        var request = _seeder.SeedThesisRequest(student.Id, tutor.Id, subjectArea.Id, "Request message");

        // Act
        var result = await _thesisRequestService.GetAllAsync(1, 10, Guid.NewGuid(), new List<string> { Roles.Admin });

        // Assert
        Assert.That(result.Items.Count, Is.EqualTo(1));
        Assert.That(result.Items.First().Message, Is.EqualTo("Request message"));
    }

    [Test]
    public async Task CanGetThesisRequestsForStudent()
    {
        // Arrange
        var student1 = _seeder.SeedUser("Student1", "One", "student1@example.com", "password", Roles.Student);
        var student2 = _seeder.SeedUser("Student2", "Two", "student2@example.com", "password", Roles.Student);
        var tutor = _seeder.SeedUser("Tutor", "One", "tutor@example.com", "password", Roles.Tutor);
        var subjectArea = _context.SubjectAreas.First();
        var request1 = _seeder.SeedThesisRequest(student1.Id, tutor.Id, subjectArea.Id, "Message1");
        var request2 = _seeder.SeedThesisRequest(student2.Id, tutor.Id, subjectArea.Id, "Message2");

        // Act
        var result = await _thesisRequestService.GetRequestsForUserAsync(student1.Id, 1, 10);

        // Assert
        Assert.That(result.Items.Count, Is.EqualTo(1));
        Assert.That(result.Items.First().Message, Is.EqualTo("Message1"));
    }

    [Test]
    public async Task CanCreateThesisRequest()
    {
        // Arrange
        var student = _seeder.SeedUser("Student", "One", "student@example.com", "password", Roles.Student);
        var tutor = _seeder.SeedUser("Tutor", "One", "tutor@example.com", "password", Roles.Tutor);
        var subjectArea = _context.SubjectAreas.First();

        var request = new ThesisRequestCreateRequestBusinessLogicModel
        {
            StudentId = student.Id,
            TutorId = tutor.Id,
            SubjectAreaId = subjectArea.Id,
            Message = "I would like supervision."
        };

        // Act
        var createdRequest = await _thesisRequestService.CreateAsync(request);

        // Assert
        Assert.That(createdRequest, Is.Not.Null);
        Assert.That(createdRequest.Message, Is.EqualTo("I would like supervision."));
        Assert.That(createdRequest.StudentId, Is.EqualTo(student.Id));
        Assert.That(createdRequest.TutorId, Is.EqualTo(tutor.Id));
    }

    [Test]
    public async Task CanUpdateThesisRequest()
    {
        // Arrange
        var student = _seeder.SeedUser("Student", "One", "student@example.com", "password", Roles.Student);
        var tutor = _seeder.SeedUser("Tutor", "One", "tutor@example.com", "password", Roles.Tutor);
        var subjectArea = _context.SubjectAreas.First();
        var thesisRequest = _seeder.SeedThesisRequest(student.Id, tutor.Id, subjectArea.Id, "Original message");

        var updateRequest = new ThesisRequestUpdateRequestBusinessLogicModel
        {
            Message = "Updated message"
        };

        // Act
        var updatedRequest = await _thesisRequestService.UpdateAsync(thesisRequest.Id, updateRequest);

        // Assert
        Assert.That(updatedRequest.Message, Is.EqualTo("Updated message"));
    }

    [Test]
    public async Task CanDeleteThesisRequest()
    {
        // Arrange
        var student = _seeder.SeedUser("Student", "One", "student@example.com", "password", Roles.Student);
        var tutor = _seeder.SeedUser("Tutor", "One", "tutor@example.com", "password", Roles.Tutor);
        var subjectArea = _context.SubjectAreas.First();
        var thesisRequest = _seeder.SeedThesisRequest(student.Id, tutor.Id, subjectArea.Id, "Message");

        // Act
        var deleted = await _thesisRequestService.DeleteAsync(thesisRequest.Id);

        // Assert
        Assert.That(deleted, Is.True);
        var retrieved = await _thesisRequestService.GetByIdAsync(thesisRequest.Id);
        Assert.That(retrieved, Is.Null);
    }
}
