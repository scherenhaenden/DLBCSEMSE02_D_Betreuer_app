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
        // Set main tutor
        thesis.TutorId = mainTutor.Id;
        _context.SaveChanges();

        // Act
        var createdRequest = await _thesisRequestService.CreatedTutorRequestForSecondSupervisor(mainTutor.Id, secondTutor.Id, thesis.Id, "Please co-supervise.");

        // Assert
        Assert.That(createdRequest, Is.Not.Null);
        Assert.That(createdRequest.Message, Is.EqualTo("Please co-supervise."));
        Assert.That(createdRequest.Requester.Id, Is.EqualTo(mainTutor.Id));
        Assert.That(createdRequest.Receiver.Id, Is.EqualTo(secondTutor.Id));
        Assert.That(createdRequest.RequestType, Is.EqualTo(RequestTypes.CoSupervision));
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
        _seeder.SeedUserToSubjectArea(secondTutor.Id, subjectArea2.Id); // Different area
        var status = _context.ThesisStatuses.First(s => s.Name == ThesisStatuses.Registered);
        var billingStatus = _context.BillingStatuses.First();
        var thesis = _seeder.SeedThesis("Thesis for Co-Supervision", student.Id, subjectArea1.Id, status.Id, billingStatus.Id);
        thesis.TutorId = mainTutor.Id;
        _context.SaveChanges();

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _thesisRequestService.CreatedTutorRequestForSecondSupervisor(mainTutor.Id, secondTutor.Id, thesis.Id, "Please co-supervise."));
        Assert.That(ex.Message, Is.EqualTo("The selected tutor does not cover the subject area of this thesis."));
    }

    // New tests for GetRequestsForTutorAsReceiver
    [Test]
    public async Task CanGetRequestsForTutorAsReceiverWithoutStatusFilter()
    {
        // Arrange
        var student = _seeder.SeedUser("Student", "Requester", "studentreq5@example.com", "password", Roles.Student);
        var tutor = _seeder.SeedUser("Tutor", "Receiver", "tutorreceiver5@example.com", "password", Roles.Tutor);
        var subjectArea = _context.SubjectAreas.First();
        var status = _context.ThesisStatuses.First(s => s.Name == ThesisStatuses.Registered);
        var billingStatus = _context.BillingStatuses.First();
        var thesis = _seeder.SeedThesis("Thesis for Receiver Test", student.Id, subjectArea.Id, status.Id, billingStatus.Id);
        var request = _seeder.SeedThesisRequest(student.Id, tutor.Id, thesis.Id, RequestTypes.Supervision, RequestStatuses.Pending, "Receiver test message");

        // Act
        var result = await _thesisRequestService.GetRequestsForTutorAsReceiver(tutor.Id, 1, 10);

        // Assert
        Assert.That(result.Items.Count(), Is.EqualTo(1));
        Assert.That(result.Items.First().Message, Is.EqualTo("Receiver test message"));
        Assert.That(result.TotalCount, Is.EqualTo(1));
        Assert.That(result.Page, Is.EqualTo(1));
        Assert.That(result.PageSize, Is.EqualTo(10));
    }

    [Test]
    public async Task CanGetRequestsForTutorAsReceiverWithStatusFilterPending()
    {
        // Arrange
        var student = _seeder.SeedUser("Student", "Requester", "studentreq6@example.com", "password", Roles.Student);
        var tutor = _seeder.SeedUser("Tutor", "Receiver", "tutorreceiver6@example.com", "password", Roles.Tutor);
        var subjectArea = _context.SubjectAreas.First();
        var status = _context.ThesisStatuses.First(s => s.Name == ThesisStatuses.Registered);
        var billingStatus = _context.BillingStatuses.First();
        var thesis = _seeder.SeedThesis("Thesis for Receiver Status Test", student.Id, subjectArea.Id, status.Id, billingStatus.Id);
        var pendingRequest = _seeder.SeedThesisRequest(student.Id, tutor.Id, thesis.Id, RequestTypes.Supervision, RequestStatuses.Pending, "Pending message");
        var acceptedRequest = _seeder.SeedThesisRequest(student.Id, tutor.Id, thesis.Id, RequestTypes.Supervision, RequestStatuses.Accepted, "Accepted message");

        // Act
        var pendingResult = await _thesisRequestService.GetRequestsForTutorAsReceiver(tutor.Id, 1, 10, "PENDING");
        var acceptedResult = await _thesisRequestService.GetRequestsForTutorAsReceiver(tutor.Id, 1, 10, "ACCEPTED");

        // Assert
        Assert.That(pendingResult.Items.Count(), Is.EqualTo(1));
        Assert.That(pendingResult.Items.First().Message, Is.EqualTo("Pending message"));
        Assert.That(acceptedResult.Items.Count(), Is.EqualTo(1));
        Assert.That(acceptedResult.Items.First().Message, Is.EqualTo("Accepted message"));
    }

    [Test]
    public async Task CanGetRequestsForTutorAsReceiverWithPagination()
    {
        // Arrange
        var student = _seeder.SeedUser("Student", "Requester", "studentreq7@example.com", "password", Roles.Student);
        var tutor = _seeder.SeedUser("Tutor", "Receiver", "tutorreceiver7@example.com", "password", Roles.Tutor);
        var subjectArea = _context.SubjectAreas.First();
        var status = _context.ThesisStatuses.First(s => s.Name == ThesisStatuses.Registered);
        var billingStatus = _context.BillingStatuses.First();
        var thesis1 = _seeder.SeedThesis("Thesis 1 for Pagination", student.Id, subjectArea.Id, status.Id, billingStatus.Id);
        var thesis2 = _seeder.SeedThesis("Thesis 2 for Pagination", student.Id, subjectArea.Id, status.Id, billingStatus.Id);
        var request1 = _seeder.SeedThesisRequest(student.Id, tutor.Id, thesis1.Id, RequestTypes.Supervision, RequestStatuses.Pending, "Message 1");
        var request2 = _seeder.SeedThesisRequest(student.Id, tutor.Id, thesis2.Id, RequestTypes.Supervision, RequestStatuses.Pending, "Message 2");

        // Act
        var result = await _thesisRequestService.GetRequestsForTutorAsReceiver(tutor.Id, 1, 1);

        // Assert
        Assert.That(result.Items.Count(), Is.EqualTo(1));
        Assert.That(result.TotalCount, Is.EqualTo(2));
        Assert.That(result.Page, Is.EqualTo(1));
        Assert.That(result.PageSize, Is.EqualTo(1));
    }

    // New tests for GetRequestsForTutorAsRequester
    [Test]
    public async Task CanGetRequestsForTutorAsRequesterWithoutStatusFilter()
    {
        // Arrange
        var student = _seeder.SeedUser("Student", "Owner", "studentowner3@example.com", "password", Roles.Student);
        var mainTutor = _seeder.SeedUser("Main", "Tutor", "maintutor3@example.com", "password", Roles.Tutor);
        var secondTutor = _seeder.SeedUser("Second", "Tutor", "secondtutor3@example.com", "password", Roles.Tutor);
        var subjectArea = _context.SubjectAreas.First();
        var status = _context.ThesisStatuses.First(s => s.Name == ThesisStatuses.Registered);
        var billingStatus = _context.BillingStatuses.First();
        var thesis = _seeder.SeedThesis("Thesis for Requester Test", student.Id, subjectArea.Id, status.Id, billingStatus.Id);
        thesis.TutorId = mainTutor.Id;
        _context.SaveChanges();
        var request = _seeder.SeedThesisRequest(mainTutor.Id, secondTutor.Id, thesis.Id, RequestTypes.CoSupervision, RequestStatuses.Pending, "Requester test message");

        // Act
        var result = await _thesisRequestService.GetRequestsForTutorAsRequester(mainTutor.Id, 1, 10);

        // Assert
        Assert.That(result.Items.Count(), Is.EqualTo(1));
        Assert.That(result.Items.First().Message, Is.EqualTo("Requester test message"));
        Assert.That(result.TotalCount, Is.EqualTo(1));
        Assert.That(result.Page, Is.EqualTo(1));
        Assert.That(result.PageSize, Is.EqualTo(10));
    }

    [Test]
    public async Task CanGetRequestsForTutorAsRequesterWithStatusFilterPending()
    {
        // Arrange
        var student = _seeder.SeedUser("Student", "Owner", "studentowner4@example.com", "password", Roles.Student);
        var mainTutor = _seeder.SeedUser("Main", "Tutor", "maintutor4@example.com", "password", Roles.Tutor);
        var secondTutor = _seeder.SeedUser("Second", "Tutor", "secondtutor4@example.com", "password", Roles.Tutor);
        var subjectArea = _context.SubjectAreas.First();
        var status = _context.ThesisStatuses.First(s => s.Name == ThesisStatuses.Registered);
        var billingStatus = _context.BillingStatuses.First();
        var thesis = _seeder.SeedThesis("Thesis for Requester Status Test", student.Id, subjectArea.Id, status.Id, billingStatus.Id);
        thesis.TutorId = mainTutor.Id;
        _context.SaveChanges();
        var pendingRequest = _seeder.SeedThesisRequest(mainTutor.Id, secondTutor.Id, thesis.Id, RequestTypes.CoSupervision, RequestStatuses.Pending, "Pending requester message");
        var acceptedRequest = _seeder.SeedThesisRequest(mainTutor.Id, secondTutor.Id, thesis.Id, RequestTypes.CoSupervision, RequestStatuses.Accepted, "Accepted requester message");

        // Act
        var pendingResult = await _thesisRequestService.GetRequestsForTutorAsRequester(mainTutor.Id, 1, 10, "PENDING");
        var acceptedResult = await _thesisRequestService.GetRequestsForTutorAsRequester(mainTutor.Id, 1, 10, "ACCEPTED");

        // Assert
        Assert.That(pendingResult.Items.Count(), Is.EqualTo(1));
        Assert.That(pendingResult.Items.First().Message, Is.EqualTo("Pending requester message"));
        Assert.That(acceptedResult.Items.Count(), Is.EqualTo(1));
        Assert.That(acceptedResult.Items.First().Message, Is.EqualTo("Accepted requester message"));
    }

    [Test]
    public async Task CanGetRequestsForTutorAsRequesterWithPagination()
    {
        // Arrange
        var student = _seeder.SeedUser("Student", "Owner", "studentowner5@example.com", "password", Roles.Student);
        var mainTutor = _seeder.SeedUser("Main", "Tutor", "maintutor5@example.com", "password", Roles.Tutor);
        var secondTutor = _seeder.SeedUser("Second", "Tutor", "secondtutor5@example.com", "password", Roles.Tutor);
        var subjectArea = _context.SubjectAreas.First();
        var status = _context.ThesisStatuses.First(s => s.Name == ThesisStatuses.Registered);
        var billingStatus = _context.BillingStatuses.First();
        var thesis1 = _seeder.SeedThesis("Thesis 1 for Requester Pagination", student.Id, subjectArea.Id, status.Id, billingStatus.Id);
        var thesis2 = _seeder.SeedThesis("Thesis 2 for Requester Pagination", student.Id, subjectArea.Id, status.Id, billingStatus.Id);
        thesis1.TutorId = mainTutor.Id;
        thesis2.TutorId = mainTutor.Id;
        _context.SaveChanges();
        var request1 = _seeder.SeedThesisRequest(mainTutor.Id, secondTutor.Id, thesis1.Id, RequestTypes.CoSupervision, RequestStatuses.Pending, "Requester message 1");
        var request2 = _seeder.SeedThesisRequest(mainTutor.Id, secondTutor.Id, thesis2.Id, RequestTypes.CoSupervision, RequestStatuses.Pending, "Requester message 2");

        // Act
        var result = await _thesisRequestService.GetRequestsForTutorAsRequester(mainTutor.Id, 1, 1);

        // Assert
        Assert.That(result.Items.Count(), Is.EqualTo(1));
        Assert.That(result.TotalCount, Is.EqualTo(2));
        Assert.That(result.Page, Is.EqualTo(1));
        Assert.That(result.PageSize, Is.EqualTo(1));
    }
}
