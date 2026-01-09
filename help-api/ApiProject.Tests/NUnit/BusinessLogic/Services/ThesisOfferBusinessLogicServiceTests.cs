using ApiProject.BusinessLogic.Services;
using ApiProject.DatabaseAccess.Context;
using Microsoft.EntityFrameworkCore;
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
    public async Task TutorCreatesThesisOfferShouldPass()
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
    public async Task StudentCreatesThesisOfferShouldNotPass()
    {
        // Arrange
        var student = _seeder.SeedUser("Student", "One", "student@example.com", "password", Roles.Student);
        var subjectArea = _context.SubjectAreas.First();

        var request = new ThesisOfferCreateRequestBusinessLogicModel
        {
            Title = "Offer by Student",
            Description = "Description",
            SubjectAreaId = subjectArea.Id,
            TutorId = student.Id, // Student trying to create offer
            MaxStudents = 2,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _thesisOfferService.CreateAsync(request));
        Assert.That(ex.Message, Is.EqualTo("Only tutors can create thesis offers."));
    }

    [Test]
    public async Task TutorsForIntotalCreateFiveOthersEachAndEachOneGetsBackOnlyItsOwnOffersWhenOffersGetRequestedByOwnerIDShouldPass()
    {
        // Arrange
        var tutors = new List<UserDataAccessModel>();
        var offers = new List<ThesisOfferDataAccessModel>();
        var subjectArea = _context.SubjectAreas.First();

        for (int i = 1; i <= 5; i++)
        {
            var tutor = _seeder.SeedUser($"Tutor{i}", "Test", $"tutor{i}@example.com", "password", Roles.Tutor);
            tutors.Add(tutor);

            var offer = _seeder.SeedThesisOffer($"Offer{i}", "Description", subjectArea.Id, tutor.Id, 1, DateTime.UtcNow.AddDays(30));
            offers.Add(offer);
        }

        // Act & Assert
        for (int i = 0; i < 5; i++)
        {
            var result = await _thesisOfferService.GetAllAsync(1, 10, tutors[i].Id, new List<string> { Roles.Tutor });
            Assert.That(result.Items.Count, Is.EqualTo(1));
            Assert.That(result.Items.First().Title, Is.EqualTo($"Offer{i + 1}"));
        }
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

    [Test]
    public async Task UserCreatesApplicationOnExistingThesisOfferShouldPass()
    {
        // Arrange
        var student = _seeder.SeedUser("Student", "Applicant", "studentapp@example.com", "password", Roles.Student);
        var tutor = _seeder.SeedUser("Tutor", "Offerer", "tutoroffer@example.com", "password", Roles.Tutor);
        var subjectArea = _context.SubjectAreas.First();
        var offer = _seeder.SeedThesisOffer("Open Offer", "Description", subjectArea.Id, tutor.Id, 2, DateTime.UtcNow.AddDays(30));

        var request = new ThesisOfferApplicationCreateRequestBusinessLogicModel
        {
            ThesisOfferId = offer.Id,
            StudentId = student.Id,
            Message = "I am very interested in this topic."
        };

        // Act
        var (application, error) = await _thesisOfferService.CreateApplicationAsync(request);

        // Assert
        Assert.That(error, Is.Null);
        Assert.That(application, Is.Not.Null);
        Assert.That(application.Message, Is.EqualTo("I am very interested in this topic."));
        Assert.That(application.StudentId, Is.EqualTo(student.Id));
    }

    [Test]
    public async Task UserCreatesApplicationOnNonExistingThesisOfferShouldPass()
    {
        // Arrange
        var student = _seeder.SeedUser("Student", "Applicant2", "studentapp2@example.com", "password", Roles.Student);

        var request = new ThesisOfferApplicationCreateRequestBusinessLogicModel
        {
            ThesisOfferId = Guid.NewGuid(), // Non-existing offer
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
    public async Task UserCreatesApplicationOnExistingButClosedThesisOfferShouldNotPass()
    {
        // Arrange
        var student = _seeder.SeedUser("Student", "Applicant3", "studentapp3@example.com", "password", Roles.Student);
        var tutor = _seeder.SeedUser("Tutor", "Offerer2", "tutoroffer2@example.com", "password", Roles.Tutor);
        var subjectArea = _context.SubjectAreas.First();
        var closedStatus = _context.ThesisOfferStatuses.First(s => s.Name == ThesisOfferStatuses.Closed);
        var offer = _seeder.SeedThesisOfferWithStatus("Closed Offer", "Description", subjectArea.Id, tutor.Id, 1, DateTime.UtcNow.AddDays(30), closedStatus.Id);

        var request = new ThesisOfferApplicationCreateRequestBusinessLogicModel
        {
            ThesisOfferId = offer.Id,
            StudentId = student.Id,
            Message = "I want to apply."
        };

        // Act
        var (application, error) = await _thesisOfferService.CreateApplicationAsync(request);

        // Assert
        Assert.That(application, Is.Null);
        Assert.That(error, Is.EqualTo("Thesis offer does not have open state anymore."));
    }

    [Test]
    public async Task UserCreatesApplicationOnExistingButArchivedThesisOfferShouldNotPass()
    {
        // Arrange
        var student = _seeder.SeedUser("Student", "Applicant4", "studentapp4@example.com", "password", Roles.Student);
        var tutor = _seeder.SeedUser("Tutor", "Offerer3", "tutoroffer3@example.com", "password", Roles.Tutor);
        var subjectArea = _context.SubjectAreas.First();
        var archivedStatus = _context.ThesisOfferStatuses.First(s => s.Name == ThesisOfferStatuses.Archived);
        var offer = _seeder.SeedThesisOfferWithStatus("Archived Offer", "Description", subjectArea.Id, tutor.Id, 1, DateTime.UtcNow.AddDays(30), archivedStatus.Id);

        var request = new ThesisOfferApplicationCreateRequestBusinessLogicModel
        {
            ThesisOfferId = offer.Id,
            StudentId = student.Id,
            Message = "I want to apply."
        };

        // Act
        var (application, error) = await _thesisOfferService.CreateApplicationAsync(request);

        // Assert
        Assert.That(application, Is.Null);
        Assert.That(error, Is.EqualTo("Thesis offer does not have open state anymore."));
    }

    [Test]
    public async Task UpdateThesisOffer_SuccessfulUpdateByTutor()
    {
        // Arrange
        var tutor = _seeder.SeedUser("Tutor", "Updater", "tutorupdate@example.com", "password", Roles.Tutor);
        var subjectArea = _context.SubjectAreas.First();
        var offer = _seeder.SeedThesisOffer("Original Title", "Original Description", subjectArea.Id, tutor.Id, 1, DateTime.UtcNow.AddDays(30));

        var updateRequest = new ThesisOfferUpdateRequestBusinessLogicModel
        {
            Title = "Updated Title",
            Description = "Updated Description",
            MaxStudents = 5,
            ExpiresAt = DateTime.UtcNow.AddDays(60)
        };

        // Act
        var updatedOffer = await _thesisOfferService.UpdateAsync(offer.Id, updateRequest, tutor.Id);

        // Assert
        Assert.That(updatedOffer, Is.Not.Null);
        Assert.That(updatedOffer.Title, Is.EqualTo("Updated Title"));
        Assert.That(updatedOffer.Description, Is.EqualTo("Updated Description"));
        Assert.That(updatedOffer.MaxStudents, Is.EqualTo(5));
        Assert.That(updatedOffer.ExpiresAt, Is.EqualTo(updateRequest.ExpiresAt.Value).Within(TimeSpan.FromSeconds(1)));
    }

    [Test]
    public async Task UpdateThesisOffer_PartialUpdate()
    {
        // Arrange
        var tutor = _seeder.SeedUser("Tutor", "Partial", "tutorpartial@example.com", "password", Roles.Tutor);
        var subjectArea = _context.SubjectAreas.First();
        var offer = _seeder.SeedThesisOffer("Original Title", "Original Description", subjectArea.Id, tutor.Id, 1, DateTime.UtcNow.AddDays(30));

        var updateRequest = new ThesisOfferUpdateRequestBusinessLogicModel
        {
            Title = "New Title" // Only update title
        };

        // Act
        var updatedOffer = await _thesisOfferService.UpdateAsync(offer.Id, updateRequest, tutor.Id);

        // Assert
        Assert.That(updatedOffer.Title, Is.EqualTo("New Title"));
        Assert.That(updatedOffer.Description, Is.EqualTo("Original Description")); // Unchanged
        Assert.That(updatedOffer.MaxStudents, Is.EqualTo(1)); // Unchanged
    }

    [Test]
    public async Task UpdateThesisOffer_FailsIfOfferNotFound()
    {
        // Arrange
        var tutor = _seeder.SeedUser("Tutor", "NotFound", "tutornf@example.com", "password", Roles.Tutor);

        var updateRequest = new ThesisOfferUpdateRequestBusinessLogicModel
        {
            Title = "New Title"
        };

        // Act & Assert
        var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            await _thesisOfferService.UpdateAsync(Guid.NewGuid(), updateRequest, tutor.Id));
        Assert.That(ex.Message, Is.EqualTo("Thesis offer not found."));
    }

    [Test]
    public async Task UpdateThesisOffer_FailsIfUserIsNotTutor()
    {
        // Arrange
        var tutor = _seeder.SeedUser("Tutor", "Owner", "tutorowner@example.com", "password", Roles.Tutor);
        var otherTutor = _seeder.SeedUser("Tutor", "Other", "tutorother@example.com", "password", Roles.Tutor);
        var subjectArea = _context.SubjectAreas.First();
        var offer = _seeder.SeedThesisOffer("Test Offer", "Description", subjectArea.Id, tutor.Id, 1, DateTime.UtcNow.AddDays(30));

        var updateRequest = new ThesisOfferUpdateRequestBusinessLogicModel
        {
            Title = "New Title"
        };

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _thesisOfferService.UpdateAsync(offer.Id, updateRequest, otherTutor.Id));
        Assert.That(ex.Message, Is.EqualTo("Only the tutor who created the offer can update it."));
    }

    [Test]
    public async Task UpdateThesisOffer_FailsIfOfferNotOpen()
    {
        // Arrange
        var tutor = _seeder.SeedUser("Tutor", "Closed", "tutorclosed@example.com", "password", Roles.Tutor);
        var subjectArea = _context.SubjectAreas.First();
        var closedStatus = _context.ThesisOfferStatuses.First(s => s.Name == ThesisOfferStatuses.Closed);
        var offer = _seeder.SeedThesisOfferWithStatus("Closed Offer", "Description", subjectArea.Id, tutor.Id, 1, DateTime.UtcNow.AddDays(30), closedStatus.Id);

        var updateRequest = new ThesisOfferUpdateRequestBusinessLogicModel
        {
            Title = "New Title"
        };

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _thesisOfferService.UpdateAsync(offer.Id, updateRequest, tutor.Id));
        Assert.That(ex.Message, Is.EqualTo("Thesis offer can only be updated if it is open."));
    }

    [Test]
    public async Task GetByUserId_AdminCanViewAnyTutorsOffers()
    {
        // Arrange
        var admin = _seeder.SeedUser("Admin", "User", "admin@example.com", "password", Roles.Admin);
        var tutor = _seeder.SeedUser("Tutor", "One", "tutor@example.com", "password", Roles.Tutor);
        var subjectArea = _context.SubjectAreas.First();
        var offer = _seeder.SeedThesisOffer("Test Offer", "Description", subjectArea.Id, tutor.Id, 1, DateTime.UtcNow.AddDays(30));

        // Act
        var result = await _thesisOfferService.GetByUserIdAsync(tutor.Id, admin.Id, new List<string> { Roles.Admin }, 1, 10);

        // Assert
        Assert.That(result.Items.Count, Is.EqualTo(1));
        Assert.That(result.Items.First().Title, Is.EqualTo("Test Offer"));
    }

    [Test]
    public async Task GetByUserId_StudentCanViewTutorsOffers()
    {
        // Arrange
        var student = _seeder.SeedUser("Student", "One", "student@example.com", "password", Roles.Student);
        var tutor = _seeder.SeedUser("Tutor", "One", "tutor@example.com", "password", Roles.Tutor);
        var subjectArea = _context.SubjectAreas.First();
        var offer = _seeder.SeedThesisOffer("Test Offer", "Description", subjectArea.Id, tutor.Id, 1, DateTime.UtcNow.AddDays(30));

        // Act
        var result = await _thesisOfferService.GetByUserIdAsync(tutor.Id, student.Id, new List<string> { Roles.Student }, 1, 10);

        // Assert
        Assert.That(result.Items.Count, Is.EqualTo(1));
        Assert.That(result.Items.First().Title, Is.EqualTo("Test Offer"));
    }

    [Test]
    public async Task GetByUserId_TutorCanViewOwnOffers()
    {
        // Arrange
        var tutor = _seeder.SeedUser("Tutor", "One", "tutor@example.com", "password", Roles.Tutor);
        var subjectArea = _context.SubjectAreas.First();
        var offer = _seeder.SeedThesisOffer("Test Offer", "Description", subjectArea.Id, tutor.Id, 1, DateTime.UtcNow.AddDays(30));

        // Act
        var result = await _thesisOfferService.GetByUserIdAsync(tutor.Id, tutor.Id, new List<string> { Roles.Tutor }, 1, 10);

        // Assert
        Assert.That(result.Items.Count, Is.EqualTo(1));
        Assert.That(result.Items.First().Title, Is.EqualTo("Test Offer"));
    }

    [Test]
    public async Task GetByUserId_TutorCannotViewOtherTutorsOffers()
    {
        // Arrange
        var tutor1 = _seeder.SeedUser("Tutor1", "One", "tutor1@example.com", "password", Roles.Tutor);
        var tutor2 = _seeder.SeedUser("Tutor2", "Two", "tutor2@example.com", "password", Roles.Tutor);
        var subjectArea = _context.SubjectAreas.First();
        var offer = _seeder.SeedThesisOffer("Test Offer", "Description", subjectArea.Id, tutor2.Id, 1, DateTime.UtcNow.AddDays(30));

        // Act
        var result = await _thesisOfferService.GetByUserIdAsync(tutor2.Id, tutor1.Id, new List<string> { Roles.Tutor }, 1, 10);

        // Assert
        Assert.That(result.Items.Count, Is.EqualTo(1));
        Assert.That(result.Items.First().Title, Is.EqualTo("Test Offer"));
    }

    [Test]
    public async Task GetByUserId_ReturnsEmptyIfNoOffers()
    {
        // Arrange
        var student = _seeder.SeedUser("Student", "One", "student@example.com", "password", Roles.Student);
        var tutor = _seeder.SeedUser("Tutor", "One", "tutor@example.com", "password", Roles.Tutor);
        // No offers created for this tutor

        // Act
        var result = await _thesisOfferService.GetByUserIdAsync(tutor.Id, student.Id, new List<string> { Roles.Student }, 1, 10);

        // Assert
        Assert.That(result.Items.Count, Is.EqualTo(0));
        Assert.That(result.TotalCount, Is.EqualTo(0));
    }

    [Test]
    public async Task GetStatusesAsync_ReturnsAllStatuses()
    {
        // Act
        var result = await _thesisOfferService.GetStatusesAsync();

        // Assert
        Assert.That(result.Count, Is.EqualTo(3));
        Assert.That(result.Any(s => s.Name == ThesisOfferStatuses.Open), Is.True);
        Assert.That(result.Any(s => s.Name == ThesisOfferStatuses.Closed), Is.True);
        Assert.That(result.Any(s => s.Name == ThesisOfferStatuses.Archived), Is.True);
    }

    [Test]
    public async Task ShouldCreateThesisOfferAndUpdateStatusAfterWardsShouldPass()
    {
        // Arrange
        var tutor = _seeder.SeedUser("Tutor", "StatusUpdate", "tutorstatus@example.com", "password", Roles.Tutor);
        var subjectArea = _context.SubjectAreas.First();
        var offer = _seeder.SeedThesisOffer("Test Offer", "Description", subjectArea.Id, tutor.Id, 1, DateTime.UtcNow.AddDays(30));
        var closedStatus = _context.ThesisOfferStatuses.First(s => s.Name == ThesisOfferStatuses.Closed);

        var updateRequest = new ThesisOfferUpdateRequestBusinessLogicModel
        {
            ThesisOfferStatusId = closedStatus.Id
        };

        // Act
        var updatedOffer = await _thesisOfferService.UpdateAsync(offer.Id, updateRequest, tutor.Id);

        // Assert
        Assert.That(updatedOffer, Is.Not.Null);
        Assert.That(updatedOffer.Status, Is.EqualTo(ThesisOfferStatuses.Closed));
    }

    [Test]
    public async Task ShouldCreateThesisOfferAndTryUpdateStatusToNonExistantStatusAfterWardsShouldNotPass()
    {
        // Arrange
        var tutor = _seeder.SeedUser("Tutor", "InvalidStatus", "tutorinvalid@example.com", "password", Roles.Tutor);
        var subjectArea = _context.SubjectAreas.First();
        var offer = _seeder.SeedThesisOffer("Test Offer", "Description", subjectArea.Id, tutor.Id, 1, DateTime.UtcNow.AddDays(30));
        var nonExistentStatusId = Guid.NewGuid(); // Non-existent status ID

        var updateRequest = new ThesisOfferUpdateRequestBusinessLogicModel
        {
            ThesisOfferStatusId = nonExistentStatusId
        };

        // Act & Assert
        Assert.ThrowsAsync<DbUpdateException>(async () =>
            await _thesisOfferService.UpdateAsync(offer.Id, updateRequest, tutor.Id));
    }
}
