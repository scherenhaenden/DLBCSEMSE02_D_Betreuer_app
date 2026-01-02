using ApiProject.BusinessLogic.Services;
using ApiProject.DatabaseAccess.Context;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ApiProject.BusinessLogic.Models;
using ApiProject.Constants;
using ApiProject.DatabaseAccess.Entities;

namespace ApiProject.Tests.NUnit.BusinessLogic.Services;

[TestFixture]
public class ThesisOfferBusinessLogicServiceTests
{
    private ThesisDbContext _context;
    private IThesisOfferBusinessLogicService _thesisOfferService;
    private TestDataSeeder _seeder;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<ThesisDbContext>()
            .UseSqlite("Data Source=ThesisOfferBusinessLogicServiceTests.db")
            .Options;
        _context = new ThesisDbContext(options);
        _context.Database.EnsureCreated();

        _seeder = new TestDataSeeder(_context);
        _seeder.SeedRoles();
        _seeder.SeedThesisOfferStatuses();
        _seeder.SeedRequestStatuses();
        _seeder.SeedSubjectAreas();
        _context.SaveChanges();

        _thesisOfferService = new ThesisOfferBusinessLogicService(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task CanGetAllThesisOffers()
    {
        // Arrange
        var tutor = _seeder.SeedUser("Tutor", "One", "tutor@example.com", "password", Roles.Tutor);
        var subjectArea = _context.SubjectAreas.First();
        var offer = _seeder.SeedThesisOffer("Test Offer", "Description", subjectArea.Id, tutor.Id, 1, DateTime.UtcNow.AddDays(30));

        // Act
        var result = await _thesisOfferService.GetAllAsync(1, 10, Guid.NewGuid(), new List<string> { Roles.Student });

        // Assert
        Assert.That(result.Items.Count, Is.EqualTo(1));
        Assert.That(result.Items.First().Title, Is.EqualTo("Test Offer"));
    }

    [Test]
    public async Task CanGetThesisOffersForTutor()
    {
        // Arrange
        var tutor1 = _seeder.SeedUser("Tutor1", "One", "tutor1@example.com", "password", Roles.Tutor);
        var tutor2 = _seeder.SeedUser("Tutor2", "Two", "tutor2@example.com", "password", Roles.Tutor);
        var subjectArea = _context.SubjectAreas.First();
        var offer1 = _seeder.SeedThesisOffer("Offer1", "Description", subjectArea.Id, tutor1.Id, 1, DateTime.UtcNow.AddDays(30));
        var offer2 = _seeder.SeedThesisOffer("Offer2", "Description", subjectArea.Id, tutor2.Id, 1, DateTime.UtcNow.AddDays(30));

        // Act
        var result = await _thesisOfferService.GetAllAsync(1, 10, tutor1.Id, new List<string> { Roles.Tutor });

        // Assert
        Assert.That(result.Items.Count, Is.EqualTo(1));
        Assert.That(result.Items.First().Title, Is.EqualTo("Offer1"));
    }

    [Test]
    public async Task CanGetApplicationsForUser()
    {
        // Arrange
        var student = _seeder.SeedUser("Student", "One", "student@example.com", "password", Roles.Student);
        var tutor = _seeder.SeedUser("Tutor", "One", "tutor@example.com", "password", Roles.Tutor);
        var subjectArea = _context.SubjectAreas.First();
        var offer = _seeder.SeedThesisOffer("Test Offer", "Description", subjectArea.Id, tutor.Id, 1, DateTime.UtcNow.AddDays(30));
        var application = _seeder.SeedThesisOfferApplication(offer.Id, student.Id, "Message");

        // Act
        var result = await _thesisOfferService.GetApplicationsForUserAsync(student.Id, 1, 10);

        // Assert
        Assert.That(result.Items.Count, Is.EqualTo(1));
        Assert.That(result.Items.First().Message, Is.EqualTo("Message"));
    }

    [Test]
    public async Task CanCreateThesisOffer()
    {
        // Arrange
        var tutor = _seeder.SeedUser("Tutor", "One", "tutor@example.com", "password", Roles.Tutor);
        var subjectArea = _context.SubjectAreas.First();

        var request = new ThesisOfferCreateRequestBusinessLogicModel
        {
            Title = "New Offer",
            Description = "Description",
            SubjectAreaId = subjectArea.Id,
            TutorId = tutor.Id,
            MaxStudents = 2,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };

        // Act
        var createdOffer = await _thesisOfferService.CreateAsync(request);

        // Assert
        Assert.That(createdOffer, Is.Not.Null);
        Assert.That(createdOffer.Title, Is.EqualTo("New Offer"));
        Assert.That(createdOffer.TutorId, Is.EqualTo(tutor.Id));
    }

    [Test]
    public async Task CanCreateApplication()
    {
        // Arrange
        var student = _seeder.SeedUser("Student", "One", "student@example.com", "password", Roles.Student);
        var tutor = _seeder.SeedUser("Tutor", "One", "tutor@example.com", "password", Roles.Tutor);
        var subjectArea = _context.SubjectAreas.First();
        var offer = _seeder.SeedThesisOffer("Test Offer", "Description", subjectArea.Id, tutor.Id, 1, DateTime.UtcNow.AddDays(30));

        var request = new ThesisOfferApplicationCreateRequestBusinessLogicModel
        {
            ThesisOfferId = offer.Id,
            StudentId = student.Id,
            Message = "I am interested."
        };

        // Act
        var (application, error) = await _thesisOfferService.CreateApplicationAsync(request);

        // Assert
        Assert.That(error, Is.Null);
        Assert.That(application, Is.Not.Null);
        Assert.That(application.Message, Is.EqualTo("I am interested."));
    }

    [Test]
    public async Task CreateApplication_FailsIfOfferNotFound()
    {
        // Arrange
        var student = _seeder.SeedUser("Student", "One", "student@example.com", "password", Roles.Student);

        var request = new ThesisOfferApplicationCreateRequestBusinessLogicModel
        {
            ThesisOfferId = Guid.NewGuid(),
            StudentId = student.Id,
            Message = "Message"
        };

        // Act
        var (application, error) = await _thesisOfferService.CreateApplicationAsync(request);

        // Assert
        Assert.That(application, Is.Null);
        Assert.That(error, Is.EqualTo("Thesis offer not found."));
    }

    [Test]
    public async Task CreateApplication_FailsIfOfferNotOpen()
    {
        // Arrange
        var student = _seeder.SeedUser("Student", "One", "student@example.com", "password", Roles.Student);
        var tutor = _seeder.SeedUser("Tutor", "One", "tutor@example.com", "password", Roles.Tutor);
        var subjectArea = _context.SubjectAreas.First();
        var closedStatus = _context.ThesisOfferStatuses.First(s => s.Name == ThesisOfferStatuses.Closed);
        var offer = _seeder.SeedThesisOfferWithStatus("Closed Offer", "Description", subjectArea.Id, tutor.Id, 1, DateTime.UtcNow.AddDays(30), closedStatus.Id);

        var request = new ThesisOfferApplicationCreateRequestBusinessLogicModel
        {
            ThesisOfferId = offer.Id,
            StudentId = student.Id,
            Message = "Message"
        };

        // Act
        var (application, error) = await _thesisOfferService.CreateApplicationAsync(request);

        // Assert
        Assert.That(application, Is.Null);
        Assert.That(error, Is.EqualTo("Thesis offer does not have open state anymore."));
    }
}
