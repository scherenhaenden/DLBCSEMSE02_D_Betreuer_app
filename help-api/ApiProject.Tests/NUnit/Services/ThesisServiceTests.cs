using ApiProject.Db.Entities;
using ApiProject.Logic.Models;
using ApiProject.Logic.Services;
using Moq;

namespace ApiProject.Tests.NUnit.Services;

[TestFixture]
public class ThesisServiceTests : TestBase
{
    private ThesisService _thesisService = null!;
    private Mock<IUserService> _mockUserService = null!;

    [SetUp]
    public void Setup()
    {
        var context = CreateInMemoryContext();
        _mockUserService = new Mock<IUserService>();
        _thesisService = new ThesisService(context, _mockUserService.Object);
    }

    [Test]
    public async Task GetAllAsync_ReturnsPaginatedResult()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var thesis1 = new Thesis { Id = Guid.NewGuid(), Title = "Thesis 1" };
        var thesis2 = new Thesis { Id = Guid.NewGuid(), Title = "Thesis 2" };
        context.Theses.AddRange(thesis1, thesis2);
        await context.SaveChangesAsync();

        var service = new ThesisService(context, _mockUserService.Object);

        // Act
        var result = await service.GetAllAsync(1, 10);

        // Assert
        Assert.That(result.Items.Count, Is.EqualTo(2));
        Assert.That(result.TotalCount, Is.EqualTo(2));
    }

    [Test]
    public async Task GetByIdAsync_ExistingThesis_ReturnsThesis()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var thesisId = Guid.NewGuid();
        var thesis = new Thesis { Id = thesisId, Title = "Test Thesis" };
        context.Theses.Add(thesis);
        await context.SaveChangesAsync();

        var service = new ThesisService(context, _mockUserService.Object);

        // Act
        var result = await service.GetByIdAsync(thesisId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(thesisId));
    }

    [Test]
    public async Task CreateThesisAsync_ValidData_CreatesThesis()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new ThesisService(context, _mockUserService.Object);
        _mockUserService.Setup(us => us.UserHasRoleAsync(It.IsAny<Guid>(), "STUDENT")).ReturnsAsync(true);
        _mockUserService.Setup(us => us.UserHasRoleAsync(It.IsAny<Guid>(), "TUTOR")).ReturnsAsync(true);

        var request = new ThesisCreateRequest
        {
            Title = "New Thesis",
            OwnerId = Guid.NewGuid(),
            TutorId = Guid.NewGuid(),
            TopicId = Guid.NewGuid(),
            ProgressPercent = 0
        };

        // Act
        var result = await service.CreateThesisAsync(request);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Title, Is.EqualTo("New Thesis"));
        Assert.That(result.Status, Is.EqualTo(ThesisStatus.Draft));
    }

    [Test]
    public void CreateThesisAsync_OwnerNotStudent_ThrowsException()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new ThesisService(context, _mockUserService.Object);
        _mockUserService.Setup(us => us.UserHasRoleAsync(It.IsAny<Guid>(), "STUDENT")).ReturnsAsync(false);

        var request = new ThesisCreateRequest
        {
            Title = "New Thesis",
            OwnerId = Guid.NewGuid(),
            TutorId = Guid.NewGuid(),
            TopicId = Guid.NewGuid(),
            ProgressPercent = 0
        };

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(async () => await service.CreateThesisAsync(request));
    }

    [Test]
    public async Task UpdateThesisAsync_ValidData_UpdatesThesis()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new ThesisService(context, _mockUserService.Object);
        var thesisId = Guid.NewGuid();
        var thesis = new Thesis { Id = thesisId, Title = "Old Title", ProgressPercent = 0 };
        context.Theses.Add(thesis);
        await context.SaveChangesAsync();

        var request = new ThesisUpdateRequest
        {
            Title = "New Title",
            ProgressPercent = 50
        };

        // Act
        var result = await service.UpdateThesisAsync(thesisId, request);

        // Assert
        Assert.That(result.Title, Is.EqualTo("New Title"));
        Assert.That(result.ProgressPercent, Is.EqualTo(50));
    }

    [Test]
    public void UpdateThesisAsync_NonExistingThesis_ThrowsException()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new ThesisService(context, _mockUserService.Object);

        var request = new ThesisUpdateRequest { Title = "New Title" };

        // Act & Assert
        Assert.ThrowsAsync<KeyNotFoundException>(async () => await service.UpdateThesisAsync(Guid.NewGuid(), request));
    }

    [Test]
    public async Task DeleteThesisAsync_ExistingThesis_ReturnsTrue()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new ThesisService(context, _mockUserService.Object);
        var thesisId = Guid.NewGuid();
        var thesis = new Thesis { Id = thesisId, Title = "Test Thesis" };
        context.Theses.Add(thesis);
        await context.SaveChangesAsync();

        // Act
        var result = await service.DeleteThesisAsync(thesisId);

        // Assert
        Assert.That(result, Is.True);
        var deleted = await context.Theses.FindAsync(thesisId);
        Assert.That(deleted, Is.Null);
    }

    [Test]
    public async Task DeleteThesisAsync_NonExistingThesis_ReturnsFalse()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new ThesisService(context, _mockUserService.Object);

        // Act
        var result = await service.DeleteThesisAsync(Guid.NewGuid());

        // Assert
        Assert.That(result, Is.False);
    }
}
