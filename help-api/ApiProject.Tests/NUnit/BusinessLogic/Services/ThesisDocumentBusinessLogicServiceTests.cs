using ApiProject.BusinessLogic.Services;
using ApiProject.DatabaseAccess.Context;
using Microsoft.EntityFrameworkCore;
using ApiProject.BusinessLogic.Models;
using ApiProject.Constants;
using ApiProject.DatabaseAccess.Entities;
using Microsoft.AspNetCore.Http;

namespace ApiProject.Tests.NUnit.BusinessLogic.Services;

[TestFixture]
public class ThesisDocumentBusinessLogicServiceTests
{
    private ThesisDbContext _context;
    private IThesisDocumentBusinessLogicService _service;
    private TestDataSeeder _seeder;
    private Guid _billingStatusId;
    private Guid _statusId;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<ThesisDbContext>()
            .UseSqlite("Data Source=ThesisDocumentBusinessLogicServiceTests.db")
            .Options;
        _context = new ThesisDbContext(options);
        _context.Database.EnsureCreated();

        _seeder = new TestDataSeeder(_context);
        _seeder.SeedRoles();
        _seeder.SeedThesisStatuses();
        _seeder.SeedBillingStatuses();
        _seeder.SeedSubjectAreas();
        _context.SaveChanges();

        _billingStatusId = _context.BillingStatuses.First().Id;
        _statusId = _context.ThesisStatuses.First().Id;

        _service = new ThesisDocumentBusinessLogicService(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    private class MockFormFile : IFormFile
    {
        public string ContentType { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public long Length { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContentDisposition { get; set; } = string.Empty;
        public IHeaderDictionary Headers { get; set; } = new HeaderDictionary();
        public Stream OpenReadStream() => new MemoryStream(Content);
        public void CopyTo(Stream target) => new MemoryStream(Content).CopyTo(target);
        public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default) => new MemoryStream(Content).CopyToAsync(target, cancellationToken);
        public byte[] Content { get; set; } = Array.Empty<byte>();
    }

    [Test]
    public async Task GetByThesisIdAsync_ReturnsDocumentForOwner()
    {
        // Arrange
        var student = _seeder.SeedUser("Student", "One", "student@example.com", "password", Roles.Student);
        var subjectArea = _context.SubjectAreas.First();
        var thesis = _seeder.SeedThesis("Thesis", student.Id, subjectArea.Id, _statusId, _billingStatusId, "Description");
        var document = new ThesisDocumentDataAccessModel
        {
            FileName = "test.pdf",
            ContentType = "application/pdf",
            Content = new byte[] { 1, 2, 3 },
            ThesisId = thesis.Id
        };
        _context.ThesisDocuments.Add(document);
        _context.SaveChanges();

        // Act
        var result = await _service.GetByThesisIdAsync(thesis.Id, student.Id, new List<string> { Roles.Student });

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.FileName, Is.EqualTo("test.pdf"));
        Assert.That(result.ContentType, Is.EqualTo("application/pdf"));
        Assert.That(result.Content.SequenceEqual(new byte[] { 1, 2, 3 }), Is.True);
    }

    [Test]
    public async Task GetByThesisIdAsync_ReturnsDocumentForTutor()
    {
        // Arrange
        var student = _seeder.SeedUser("Student", "One", "student@example.com", "password", Roles.Student);
        var tutor = _seeder.SeedUser("Tutor", "One", "tutor@example.com", "password", Roles.Tutor);
        var subjectArea = _context.SubjectAreas.First();
        var thesis = _seeder.SeedThesis("Thesis", student.Id, subjectArea.Id, _statusId, _billingStatusId, "Description");
        thesis.TutorId = tutor.Id;
        _context.SaveChanges();
        var document = new ThesisDocumentDataAccessModel
        {
            FileName = "test.pdf",
            ContentType = "application/pdf",
            Content = new byte[] { 1, 2, 3 },
            ThesisId = thesis.Id
        };
        _context.ThesisDocuments.Add(document);
        _context.SaveChanges();

        // Act
        var result = await _service.GetByThesisIdAsync(thesis.Id, tutor.Id, new List<string> { Roles.Tutor });

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.FileName, Is.EqualTo("test.pdf"));
        Assert.That(result.ContentType, Is.EqualTo("application/pdf"));
        Assert.That(result.Content.SequenceEqual(new byte[] { 1, 2, 3 }), Is.True);
    }

    [Test]
    public async Task GetByThesisIdAsync_ReturnsNullForUnauthorizedUser()
    {
        // Arrange
        var student1 = _seeder.SeedUser("Student1", "One", "student1@example.com", "password", Roles.Student);
        var student2 = _seeder.SeedUser("Student2", "Two", "student2@example.com", "password", Roles.Student);
        var subjectArea = _context.SubjectAreas.First();
        var thesis = _seeder.SeedThesis("Thesis", student1.Id, subjectArea.Id, _statusId, _billingStatusId, "Description");
        var document = new ThesisDocumentDataAccessModel
        {
            FileName = "test.pdf",
            ContentType = "application/pdf",
            Content = new byte[] { 1, 2, 3 },
            ThesisId = thesis.Id
        };
        _context.ThesisDocuments.Add(document);
        _context.SaveChanges();

        // Act
        var result = await _service.GetByThesisIdAsync(thesis.Id, student2.Id, new List<string> { Roles.Student });

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetByThesisIdAsync_ReturnsNullForNonExistentThesis()
    {
        // Arrange
        var student = _seeder.SeedUser("Student", "One", "student@example.com", "password", Roles.Student);

        // Act
        var result = await _service.GetByThesisIdAsync(Guid.NewGuid(), student.Id, new List<string> { Roles.Student });

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task UpdateAsync_UpdatesDocumentForOwner()
    {
        // Arrange
        var student = _seeder.SeedUser("Student", "One", "student@example.com", "password", Roles.Student);
        var subjectArea = _context.SubjectAreas.First();
        var thesis = _seeder.SeedThesis("Thesis", student.Id, subjectArea.Id, _statusId, _billingStatusId, "Description");
        var document = new ThesisDocumentDataAccessModel
        {
            FileName = "test.pdf",
            ContentType = "application/pdf",
            Content = new byte[] { 1, 2, 3 },
            ThesisId = thesis.Id
        };
        _context.ThesisDocuments.Add(document);
        _context.SaveChanges();

        var newFile = new MockFormFile
        {
            FileName = "new.pdf",
            ContentType = "application/pdf",
            Content = new byte[] { 4, 5, 6 }
        };

        // Act
        var result = await _service.UpdateAsync(thesis.Id, newFile, student.Id, new List<string> { Roles.Student });

        // Assert
        Assert.That(result.FileName, Is.EqualTo("new.pdf"));
        Assert.That(result.ContentType, Is.EqualTo("application/pdf"));
        Assert.That(result.Content.SequenceEqual(new byte[] { 4, 5, 6 }), Is.True);
    }

    [Test]
    public async Task UpdateAsync_ThrowsUnauthorizedForNonOwner()
    {
        // Arrange
        var student1 = _seeder.SeedUser("Student1", "One", "student1@example.com", "password", Roles.Student);
        var student2 = _seeder.SeedUser("Student2", "Two", "student2@example.com", "password", Roles.Student);
        var subjectArea = _context.SubjectAreas.First();
        var thesis = _seeder.SeedThesis("Thesis", student1.Id, subjectArea.Id, _statusId, _billingStatusId, "Description");
        var document = new ThesisDocumentDataAccessModel
        {
            FileName = "test.pdf",
            ContentType = "application/pdf",
            Content = new byte[] { 1, 2, 3 },
            ThesisId = thesis.Id
        };
        _context.ThesisDocuments.Add(document);
        _context.SaveChanges();

        var newFile = new MockFormFile
        {
            FileName = "new.pdf",
            ContentType = "application/pdf",
            Content = new byte[] { 4, 5, 6 }
        };

        // Act & Assert
        Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await _service.UpdateAsync(thesis.Id, newFile, student2.Id, new List<string> { Roles.Student }));
    }

    [Test]
    public async Task UpdateAsync_ThrowsKeyNotFoundForNonExistentThesis()
    {
        // Arrange
        var student = _seeder.SeedUser("Student", "One", "student@example.com", "password", Roles.Student);
        var newFile = new MockFormFile
        {
            FileName = "new.pdf",
            ContentType = "application/pdf",
            Content = new byte[] { 4, 5, 6 }
        };

        // Act & Assert
        Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            await _service.UpdateAsync(Guid.NewGuid(), newFile, student.Id, new List<string> { Roles.Student }));
    }

    [Test]
    public async Task DeleteAsync_DeletesDocumentForOwner()
    {
        // Arrange
        var student = _seeder.SeedUser("Student", "One", "student@example.com", "password", Roles.Student);
        var subjectArea = _context.SubjectAreas.First();
        var thesis = _seeder.SeedThesis("Thesis", student.Id, subjectArea.Id, _statusId, _billingStatusId, "Description");
        var document = new ThesisDocumentDataAccessModel
        {
            FileName = "test.pdf",
            ContentType = "application/pdf",
            Content = new byte[] { 1, 2, 3 },
            ThesisId = thesis.Id
        };
        _context.ThesisDocuments.Add(document);
        _context.SaveChanges();

        // Act
        var result = await _service.DeleteAsync(thesis.Id, student.Id, new List<string> { Roles.Student });

        // Assert
        Assert.That(result, Is.True);
        var deleted = await _context.ThesisDocuments.FindAsync(document.Id);
        Assert.That(deleted, Is.Null);
    }

    [Test]
    public async Task DeleteAsync_ReturnsFalseForNonOwner()
    {
        // Arrange
        var student1 = _seeder.SeedUser("Student1", "One", "student1@example.com", "password", Roles.Student);
        var student2 = _seeder.SeedUser("Student2", "Two", "student2@example.com", "password", Roles.Student);
        var subjectArea = _context.SubjectAreas.First();
        var thesis = _seeder.SeedThesis("Thesis", student1.Id, subjectArea.Id, _statusId, _billingStatusId, "Description");
        var document = new ThesisDocumentDataAccessModel
        {
            FileName = "test.pdf",
            ContentType = "application/pdf",
            Content = new byte[] { 1, 2, 3 },
            ThesisId = thesis.Id
        };
        _context.ThesisDocuments.Add(document);
        _context.SaveChanges();

        // Act
        var result = await _service.DeleteAsync(thesis.Id, student2.Id, new List<string> { Roles.Student });

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task DeleteAsync_ReturnsFalseForNonExistentDocument()
    {
        // Arrange
        var student = _seeder.SeedUser("Student", "One", "student@example.com", "password", Roles.Student);

        // Act
        var result = await _service.DeleteAsync(Guid.NewGuid(), student.Id, new List<string> { Roles.Student });

        // Assert
        Assert.That(result, Is.False);
    }
}
