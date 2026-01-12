using ApiProject.BusinessLogic.Services;
using ApiProject.DatabaseAccess.Context;
using Microsoft.EntityFrameworkCore;
using ApiProject.Constants;

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

        _thesisRequestService = new ThesisRequestBusinessLogicService(_context);
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
        var result = await _thesisRequestService.GetRequestsForUserAsync(student.Id, 1, 10);

        // Assert
        Assert.That(result.Items.Count(), Is.EqualTo(1));
        Assert.That(result.Items.First().Message, Is.EqualTo("Request message"));
        Assert.That(result.TotalCount, Is.EqualTo(1));
        Assert.That(result.Page, Is.EqualTo(1));
        Assert.That(result.PageSize, Is.EqualTo(10));
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

    [Test]
    public async Task StudentCreatesRequestSupervisionShouldPass()
    {
        // Arrange
        var student = _seeder.SeedUser("Student", "Requester", "studentreq@example.com", "password", Roles.Student);
        var tutor = _seeder.SeedUser("Tutor", "Receiver", "tutorreceiver@example.com", "password", Roles.Tutor);
        var subjectArea = _context.SubjectAreas.First();
        _seeder.SeedUserToSubjectArea(tutor.Id, subjectArea.Id);
        var status = _context.ThesisStatuses.First(s => s.Name == ThesisStatuses.Registered);
        var billingStatus = _context.BillingStatuses.First();
        var thesis = _seeder.SeedThesis("Thesis for Request", student.Id, subjectArea.Id, status.Id, billingStatus.Id);

        // Act
        var createdRequest = await _thesisRequestService.CreatedStudentRequestForTutor(student.Id, tutor.Id, thesis.Id, "Please supervise my thesis.");

        // Assert
        Assert.That(createdRequest, Is.Not.Null);
        Assert.That(createdRequest.Message, Is.EqualTo("Please supervise my thesis."));
        Assert.That(createdRequest.Requester.Id, Is.EqualTo(student.Id));
        Assert.That(createdRequest.Receiver.Id, Is.EqualTo(tutor.Id));
        Assert.That(createdRequest.RequestType, Is.EqualTo(RequestTypes.Supervision));

        // Verify the request exists in the database
        var retrievedRequest = await _thesisRequestService.GetRequestByIdAsync(createdRequest.Id);
        Assert.That(retrievedRequest, Is.Not.Null);
        Assert.That(retrievedRequest.Message, Is.EqualTo("Please supervise my thesis."));
    }

    [Test]
    public async Task StudentCreatesRequestSupervisionForTutorWithDifferentSubjectAreaShouldNotPass()
    {
        // Arrange
        var student = _seeder.SeedUser("Student", "Requester", "studentreq2@example.com", "password", Roles.Student);
        var tutor = _seeder.SeedUser("Tutor", "Receiver", "tutorreceiver2@example.com", "password", Roles.Tutor);
        var subjectArea1 = _context.SubjectAreas.First();
        var subjectArea2 = _context.SubjectAreas.Skip(1).First();
        _seeder.SeedUserToSubjectArea(tutor.Id, subjectArea2.Id); // Assign tutor to different subject area
        var status = _context.ThesisStatuses.First(s => s.Name == ThesisStatuses.Registered);
        var billingStatus = _context.BillingStatuses.First();
        var thesis = _seeder.SeedThesis("Thesis for Request", student.Id, subjectArea1.Id, status.Id, billingStatus.Id); // Thesis has subjectArea1

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _thesisRequestService.CreatedStudentRequestForTutor(student.Id, tutor.Id, thesis.Id, "Please supervise my thesis."));
        Assert.That(ex.Message, Is.EqualTo("The selected tutor does not cover the subject area of this thesis."));
    }

    [Test]
    public async Task StudentCreatesRequestsWithNonExistingTutorShouldNotPass()
    {
        // Arrange
        var student = _seeder.SeedUser("Student", "Requester", "studentreq3@example.com", "password", Roles.Student);
        var subjectArea = _context.SubjectAreas.First();
        var status = _context.ThesisStatuses.First(s => s.Name == ThesisStatuses.Registered);
        var billingStatus = _context.BillingStatuses.First();
        var thesis = _seeder.SeedThesis("Thesis for Request", student.Id, subjectArea.Id, status.Id, billingStatus.Id);
        var nonExistingTutorId = Guid.NewGuid();

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _thesisRequestService.CreatedStudentRequestForTutor(student.Id, nonExistingTutorId, thesis.Id, "Please supervise."));
        Assert.That(ex.Message, Does.Contain("Sequence contains no elements")); // Or similar
    }

    [Test]
    public async Task StudentCreatesRequestsWithExistingButNotTutorTutorShouldNotPass()
    {
        // Arrange
        var student = _seeder.SeedUser("Student", "Requester", "studentreq4@example.com", "password", Roles.Student);
        var nonTutor = _seeder.SeedUser("NonTutor", "User", "nontutor@example.com", "password", Roles.Student); // Not a tutor
        var subjectArea = _context.SubjectAreas.First();
        var status = _context.ThesisStatuses.First(s => s.Name == ThesisStatuses.Registered);
        var billingStatus = _context.BillingStatuses.First();
        var thesis = _seeder.SeedThesis("Thesis for Request", student.Id, subjectArea.Id, status.Id, billingStatus.Id);

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _thesisRequestService.CreatedStudentRequestForTutor(student.Id, nonTutor.Id, thesis.Id, "Please supervise."));
        Assert.That(ex.Message, Is.EqualTo("The receiver of a request must be a TUTOR."));
    }

    [Test]
    public async Task TutorCreatesRequestToAnotherTutorForRevisionShouldPass()
    {
        // Arrange
        var student = _seeder.SeedUser("Student", "Owner", "studentowner@example.com", "password", Roles.Student);
        var mainTutor = _seeder.SeedUser("Main", "Tutor", "maintutor@example.com", "password", Roles.Tutor);
        var secondTutor = _seeder.SeedUser("Second", "Tutor", "secondtutor@example.com", "password", Roles.Tutor);
        var subjectArea = _context.SubjectAreas.First();
        _seeder.SeedUserToSubjectArea(mainTutor.Id, subjectArea.Id);
        _seeder.SeedUserToSubjectArea(secondTutor.Id, subjectArea.Id);
        var status = _context.ThesisStatuses.First(s => s.Name == ThesisStatuses.Registered);
        var billingStatus = _context.BillingStatuses.First();
        var thesis = _seeder.SeedThesis("Thesis for Co-Supervision", student.Id, subjectArea.Id, status.Id, billingStatus.Id);

        // Create and accept original supervision request
        var originalRequest = await _thesisRequestService.CreateRequestAsync(student.Id, thesis.Id, mainTutor.Id, RequestTypes.Supervision, "Original supervision request", new DateTime(2026, 1, 1), new DateTime(2026, 7, 1));
        await _thesisRequestService.RespondToRequestAsync(originalRequest.Id, mainTutor.Id, true, null);

        // Act
        var createdRequest = await _thesisRequestService.CreatedTutorRequestForSecondSupervisor(mainTutor.Id, secondTutor.Id, thesis.Id, "Please co-supervise.");

        // Assert
        Assert.That(createdRequest, Is.Not.Null);
        Assert.That(createdRequest.Message, Is.EqualTo("Please co-supervise."));
        Assert.That(createdRequest.Requester.Id, Is.EqualTo(mainTutor.Id));
        Assert.That(createdRequest.Receiver.Id, Is.EqualTo(secondTutor.Id));
        Assert.That(createdRequest.RequestType, Is.EqualTo(RequestTypes.CoSupervision));
        Assert.That(createdRequest.PlannedStartOfSupervision, Is.EqualTo(new DateTime(2026, 1, 1)));
        Assert.That(createdRequest.PlannedEndOfSupervision, Is.EqualTo(new DateTime(2026, 7, 1)));
    }

    [Test]
    public async Task TutorCreatesRequestToAnotherTutorForRevisionWithDifferentAreaShouldNotPass()
    {
        // Arrange
        var student = _seeder.SeedUser("Student", "Owner", "studentowner2@example.com", "password", Roles.Student);
        var mainTutor = _seeder.SeedUser("Main", "Tutor", "maintutor2@example.com", "password", Roles.Tutor);
        var secondTutor = _seeder.SeedUser("Second", "Tutor", "secondtutor2@example.com", "password", Roles.Tutor);
        var subjectArea1 = _context.SubjectAreas.First();
        var subjectArea2 = _context.SubjectAreas.Skip(1).First();
        _seeder.SeedUserToSubjectArea(mainTutor.Id, subjectArea1.Id);
        _seeder.SeedUserToSubjectArea(secondTutor.Id, subjectArea2.Id); // Different subject area
        var status = _context.ThesisStatuses.First(s => s.Name == ThesisStatuses.Registered);
        var billingStatus = _context.BillingStatuses.First();
        var thesis = _seeder.SeedThesis("Thesis for Co-Supervision", student.Id, subjectArea1.Id, status.Id, billingStatus.Id);

        // Create and accept original supervision request
        var originalRequest = await _thesisRequestService.CreateRequestAsync(student.Id, thesis.Id, mainTutor.Id, RequestTypes.Supervision, "Original supervision request", new DateTime(2026, 1, 1), new DateTime(2026, 7, 1));
        await _thesisRequestService.RespondToRequestAsync(originalRequest.Id, mainTutor.Id, true, null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _thesisRequestService.CreatedTutorRequestForSecondSupervisor(mainTutor.Id, secondTutor.Id, thesis.Id, "Please co-supervise."));
        Assert.That(ex.Message, Is.EqualTo("The selected tutor does not cover the subject area of this thesis."));
    }

    [Test]
    public async Task TutorCreatesRequestToSelfShouldNotPass()
    {
        // Arrange
        var student = _seeder.SeedUser("Student", "Owner", "studentowner3@example.com", "password", Roles.Student);
        var mainTutor = _seeder.SeedUser("Main", "Tutor", "maintutor3@example.com", "password", Roles.Tutor);
        var subjectArea = _context.SubjectAreas.First();
        _seeder.SeedUserToSubjectArea(mainTutor.Id, subjectArea.Id);
        var status = _context.ThesisStatuses.First(s => s.Name == ThesisStatuses.Registered);
        var billingStatus = _context.BillingStatuses.First();
        var thesis = _seeder.SeedThesis("Thesis for Co-Supervision", student.Id, subjectArea.Id, status.Id, billingStatus.Id);

        // Create and accept original supervision request
        var originalRequest = await _thesisRequestService.CreateRequestAsync(student.Id, thesis.Id, mainTutor.Id, RequestTypes.Supervision, "Original supervision request", new DateTime(2026, 1, 1), new DateTime(2026, 7, 1));
        await _thesisRequestService.RespondToRequestAsync(originalRequest.Id, mainTutor.Id, true, null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _thesisRequestService.CreatedTutorRequestForSecondSupervisor(mainTutor.Id, mainTutor.Id, thesis.Id, "Please co-supervise."));
        Assert.That(ex.Message, Is.EqualTo("The second supervisor cannot be the same as the main supervisor."));
    }

    [Test]
    public async Task TutorCreatesRequestToAnotherTutorForCoSupervisionUsingOriginalDatesShouldPass()
    {
        // Arrange
        var student = _seeder.SeedUser("Student", "Owner", "studentowner@example.com", "password", Roles.Student);
        var mainTutor = _seeder.SeedUser("Main", "Tutor", "maintutor@example.com", "password", Roles.Tutor);
        var secondTutor = _seeder.SeedUser("Second", "Tutor", "secondtutor@example.com", "password", Roles.Tutor);
        var subjectArea = _context.SubjectAreas.First();
        _seeder.SeedUserToSubjectArea(mainTutor.Id, subjectArea.Id);
        _seeder.SeedUserToSubjectArea(secondTutor.Id, subjectArea.Id);
        var status = _context.ThesisStatuses.First(s => s.Name == ThesisStatuses.Registered);
        var billingStatus = _context.BillingStatuses.First();
        var thesis = _seeder.SeedThesis("Thesis for Co-Supervision", student.Id, subjectArea.Id, status.Id, billingStatus.Id);

        // Create and accept original supervision request
        var originalRequest = await _thesisRequestService.CreateRequestAsync(student.Id, thesis.Id, mainTutor.Id, RequestTypes.Supervision, "Original supervision request", new DateTime(2026, 1, 1), new DateTime(2026, 7, 1));
        await _thesisRequestService.RespondToRequestAsync(originalRequest.Id, mainTutor.Id, true, null);

        // Act
        var createdRequest = await _thesisRequestService.CreatedTutorRequestForSecondSupervisor(mainTutor.Id, secondTutor.Id, thesis.Id, "Please co-supervise.");

        // Assert
        Assert.That(createdRequest, Is.Not.Null);
        Assert.That(createdRequest.Message, Is.EqualTo("Please co-supervise."));
        Assert.That(createdRequest.Requester.Id, Is.EqualTo(mainTutor.Id));
        Assert.That(createdRequest.Receiver.Id, Is.EqualTo(secondTutor.Id));
        Assert.That(createdRequest.RequestType, Is.EqualTo(RequestTypes.CoSupervision));
        Assert.That(createdRequest.PlannedStartOfSupervision, Is.EqualTo(new DateTime(2026, 1, 1)));
        Assert.That(createdRequest.PlannedEndOfSupervision, Is.EqualTo(new DateTime(2026, 7, 1)));
    }

    [Test]
    public async Task StudentCannotCreateMultipleSupervisionRequestsForSameThesis()
    {
        // Arrange
        var student = _seeder.SeedUser("Student", "Requester", "studentreq5@example.com", "password", Roles.Student);
        var tutor1 = _seeder.SeedUser("Tutor", "One", "tutorone@example.com", "password", Roles.Tutor);
        var tutor2 = _seeder.SeedUser("Tutor", "Two", "tutortwo@example.com", "password", Roles.Tutor);
        var subjectArea = _context.SubjectAreas.First();
        _seeder.SeedUserToSubjectArea(tutor1.Id, subjectArea.Id);
        _seeder.SeedUserToSubjectArea(tutor2.Id, subjectArea.Id);
        var status = _context.ThesisStatuses.First(s => s.Name == ThesisStatuses.Registered);
        var billingStatus = _context.BillingStatuses.First();
        var thesis = _seeder.SeedThesis("Thesis for Multiple Requests", student.Id, subjectArea.Id, status.Id, billingStatus.Id);

        // Create first supervision request
        var firstRequest = await _thesisRequestService.CreatedStudentRequestForTutor(student.Id, tutor1.Id, thesis.Id, "First request.");

        // Assert that the first request was created successfully
        Assert.That(firstRequest, Is.Not.Null);
        Assert.That(firstRequest.Message, Is.EqualTo("First request."));
        Assert.That(firstRequest.Requester.Id, Is.EqualTo(student.Id));
        Assert.That(firstRequest.Receiver.Id, Is.EqualTo(tutor1.Id));
        Assert.That(firstRequest.RequestType, Is.EqualTo(RequestTypes.Supervision));

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _thesisRequestService.CreatedStudentRequestForTutor(student.Id, tutor2.Id, thesis.Id, "Second request."));
        Assert.That(ex.Message, Is.EqualTo("A supervision request already exists for this thesis."));
    }

    [Test]
    public async Task TutorCannotCreateMultipleCoSupervisionRequestsForSameThesis()
    {
        // Arrange
        var student = _seeder.SeedUser("Student", "Owner", "studentowner6@example.com", "password", Roles.Student);
        var mainTutor = _seeder.SeedUser("Main", "Tutor", "maintutor6@example.com", "password", Roles.Tutor);
        var secondTutor1 = _seeder.SeedUser("Second", "Tutor", "secondtutor6a@example.com", "password", Roles.Tutor);
        var secondTutor2 = _seeder.SeedUser("Third", "Tutor", "secondtutor6b@example.com", "password", Roles.Tutor);
        var subjectArea = _context.SubjectAreas.First();
        _seeder.SeedUserToSubjectArea(mainTutor.Id, subjectArea.Id);
        _seeder.SeedUserToSubjectArea(secondTutor1.Id, subjectArea.Id);
        _seeder.SeedUserToSubjectArea(secondTutor2.Id, subjectArea.Id);
        var status = _context.ThesisStatuses.First(s => s.Name == ThesisStatuses.Registered);
        var billingStatus = _context.BillingStatuses.First();
        var thesis = _seeder.SeedThesis("Thesis for Multiple Co-Supervision", student.Id, subjectArea.Id, status.Id, billingStatus.Id);

        // Create and accept original supervision request
        var originalRequest = await _thesisRequestService.CreateRequestAsync(student.Id, thesis.Id, mainTutor.Id, RequestTypes.Supervision, "Original supervision", new DateTime(2026, 1, 1), new DateTime(2026, 7, 1));
        await _thesisRequestService.RespondToRequestAsync(originalRequest.Id, mainTutor.Id, true, null);

        // Create first co-supervision request
        var firstCoRequest = await _thesisRequestService.CreatedTutorRequestForSecondSupervisor(mainTutor.Id, secondTutor1.Id, thesis.Id, "First co-supervision.");

        // Assert that the first co-supervision request was created successfully
        Assert.That(firstCoRequest, Is.Not.Null);
        Assert.That(firstCoRequest.Message, Is.EqualTo("First co-supervision."));
        Assert.That(firstCoRequest.Requester.Id, Is.EqualTo(mainTutor.Id));
        Assert.That(firstCoRequest.Receiver.Id, Is.EqualTo(secondTutor1.Id));
        Assert.That(firstCoRequest.RequestType, Is.EqualTo(RequestTypes.CoSupervision));
        Assert.That(firstCoRequest.PlannedStartOfSupervision, Is.EqualTo(new DateTime(2026, 1, 1)));
        Assert.That(firstCoRequest.PlannedEndOfSupervision, Is.EqualTo(new DateTime(2026, 7, 1)));

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _thesisRequestService.CreatedTutorRequestForSecondSupervisor(mainTutor.Id, secondTutor2.Id, thesis.Id, "Second co-supervision."));
        Assert.That(ex.Message, Is.EqualTo("A co-supervision request already exists for this thesis."));
    }
}
