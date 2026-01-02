using ApiProject.BusinessLogic.Services;
using ApiProject.DatabaseAccess.Context;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ApiProject.BusinessLogic.Models;
using ApiProject.Constants;
using ApiProject.DatabaseAccess.Entities;

namespace ApiProject.Tests.NUnit.BusinessLogic.Services;

[TestFixture]
public class ThesisBusinessLogicRequestServiceTests
{
    private ThesisDbContext _context;
    private IThesisBusinessLogicRequestService _thesisRequestService;
    private TestDataSeeder _seeder;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<ThesisDbContext>()
            .UseSqlite("Data Source=ThesisBusinessLogicRequestServiceTests.db")
            .Options;
        _context = new ThesisDbContext(options);
        _context.Database.EnsureCreated();

        _seeder = new TestDataSeeder(_context);
        _seeder.SeedRoles();
        _seeder.SeedRequestStatuses();
        _seeder.SeedRequestTypes();
        _seeder.SeedSubjectAreas();
        _seeder.SeedThesisStatuses();
        _seeder.SeedBillingStatuses();
        _context.SaveChanges();

        _thesisRequestService = new ThesisBusinessLogicRequestService(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task CanGetRequestsForUser()
    {
        // Arrange
        var student = _seeder.SeedUser("Student", "One", "student@example.com", "password", Roles.Student);
        var tutor = _seeder.SeedUser("Tutor", "One", "tutor@example.com", "password", Roles.Tutor);
        var subjectArea = _context.SubjectAreas.First();
        var status = _context.ThesisStatuses.First(s => s.Name == ThesisStatuses.Registered);
        var billingStatus = _context.BillingStatuses.First();
        var thesis = _seeder.SeedThesis("Test Thesis", student.Id, subjectArea.Id, status.Id, billingStatus.Id);
        var request = _seeder.SeedThesisRequest(student.Id, tutor.Id, thesis.Id, RequestTypes.Supervision, RequestStatuses.Pending, "Request message");

        // Act
        var result = await _thesisRequestService.GetRequestsForUserAsync(student.Id);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.First().Message, Is.EqualTo("Request message"));
    }

    [Test]
    public async Task CanCreateThesisRequest()
    {
        // Arrange
        var student = _seeder.SeedUser("Student", "One", "student@example.com", "password", Roles.Student);
        var tutor = _seeder.SeedUser("Tutor", "One", "tutor@example.com", "password", Roles.Tutor);
        var subjectArea = _context.SubjectAreas.First();
        var status = _context.ThesisStatuses.First(s => s.Name == ThesisStatuses.Registered);
        var billingStatus = _context.BillingStatuses.First();
        var thesis = _seeder.SeedThesis("Test Thesis", student.Id, subjectArea.Id, status.Id, billingStatus.Id);

        // Act
        var createdRequest = await _thesisRequestService.CreateRequestAsync(student.Id, thesis.Id, tutor.Id, "SUPERVISION", "I would like supervision.");

        // Assert
        Assert.That(createdRequest, Is.Not.Null);
        Assert.That(createdRequest.Message, Is.EqualTo("I would like supervision."));
        Assert.That(createdRequest.Requester.Id, Is.EqualTo(student.Id));
        Assert.That(createdRequest.Receiver.Id, Is.EqualTo(tutor.Id));
    }

    [Test]
    public async Task CanRespondToRequest()
    {
        // Arrange
        var student = _seeder.SeedUser("Student", "One", "student@example.com", "password", Roles.Student);
        var tutor = _seeder.SeedUser("Tutor", "One", "tutor@example.com", "password", Roles.Tutor);
        var subjectArea = _context.SubjectAreas.First();
        var status = _context.ThesisStatuses.First(s => s.Name == ThesisStatuses.Registered);
        var billingStatus = _context.BillingStatuses.First();
        var thesis = _seeder.SeedThesis("Test Thesis", student.Id, subjectArea.Id, status.Id, billingStatus.Id);
        var request = _seeder.SeedThesisRequest(student.Id, tutor.Id, thesis.Id, RequestTypes.Supervision, RequestStatuses.Pending, "Request message");

        // Act
        await _thesisRequestService.RespondToRequestAsync(request.Id, tutor.Id, true, "Accepted");

        // Assert
        var updatedRequest = await _thesisRequestService.GetRequestByIdAsync(request.Id);
        Assert.That(updatedRequest.Status, Is.EqualTo("ACCEPTED"));
    }

    [Test]
    public async Task CanGetRequestById()
    {
        // Arrange
        var student = _seeder.SeedUser("Student", "One", "student@example.com", "password", Roles.Student);
        var tutor = _seeder.SeedUser("Tutor", "One", "tutor@example.com", "password", Roles.Tutor);
        var subjectArea = _context.SubjectAreas.First();
        var status = _context.ThesisStatuses.First(s => s.Name == ThesisStatuses.Registered);
        var billingStatus = _context.BillingStatuses.First();
        var thesis = _seeder.SeedThesis("Test Thesis", student.Id, subjectArea.Id, status.Id, billingStatus.Id);
        var request = _seeder.SeedThesisRequest(student.Id, tutor.Id, thesis.Id, RequestTypes.Supervision, RequestStatuses.Pending, "Request message");

        // Act
        var retrievedRequest = await _thesisRequestService.GetRequestByIdAsync(request.Id);

        // Assert
        Assert.That(retrievedRequest, Is.Not.Null);
        Assert.That(retrievedRequest.Message, Is.EqualTo("Request message"));
    }
}
