/*using ApiProject.Logic.Services;
using Moq;

namespace ApiProject.Tests.NUnit.Services;

[TestFixture]
public class TopicServiceTests : TestBase
{
    private TopicService _topicService = null!;
    private Mock<IUserService> _mockUserService = null!;

    [SetUp]
    public void Setup()
    {
        var context = CreateInMemoryContext();
        _mockUserService = new Mock<IUserService>();
        _topicService = new TopicService(context, _mockUserService.Object);
    }

    [Test]
    public async Task GetAllAsync_ReturnsPaginatedResult()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var topic1 = new Topic { Id = Guid.NewGuid(), Title = "Topic 1", Description = "Desc 1", SubjectArea = "Area 1" };
        var topic2 = new Topic { Id = Guid.NewGuid(), Title = "Topic 2", Description = "Desc 2", SubjectArea = "Area 2" };
        context.Topics.AddRange(topic1, topic2);
        await context.SaveChangesAsync();

        var service = new TopicService(context, _mockUserService.Object);

        // Act
        var result = await service.GetAllAsync(1, 10);

        // Assert
        Assert.That(result.Items.Count, Is.EqualTo(2));
        Assert.That(result.TotalCount, Is.EqualTo(2));
    }

    [Test]
    public async Task GetByIdAsync_ExistingTopic_ReturnsTopic()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var topicId = Guid.NewGuid();
        var topic = new Topic { Id = topicId, Title = "Test Topic", Description = "Test Desc", SubjectArea = "Test Area" };
        context.Topics.Add(topic);
        await context.SaveChangesAsync();

        var service = new TopicService(context, _mockUserService.Object);

        // Act
        var result = await service.GetByIdAsync(topicId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(topicId));
    }

    [Test]
    public async Task SearchAsync_FindsMatchingTopics()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var topic1 = new Topic { Id = Guid.NewGuid(), Title = "Machine Learning", Description = "AI topic", SubjectArea = "Computer Science" };
        var topic2 = new Topic { Id = Guid.NewGuid(), Title = "Database Design", Description = "DB topic", SubjectArea = "Computer Science" };
        context.Topics.AddRange(topic1, topic2);
        await context.SaveChangesAsync();

        var service = new TopicService(context, _mockUserService.Object);

        // Act
        var result = await service.SearchAsync("Computer", 1, 10);

        // Assert
        Assert.That(result.Items.Count, Is.EqualTo(2));
        Assert.That(result.TotalCount, Is.EqualTo(2));
    }

    [Test]
    public async Task CreateTopicAsync_ValidData_CreatesTopic()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new TopicService(context, _mockUserService.Object);
        _mockUserService.Setup(us => us.UserHasRoleAsync(It.IsAny<Guid>(), "TUTOR")).ReturnsAsync(true);

        // Act
        var result = await service.CreateTopicAsync("New Topic", "Description", "Subject Area", Guid.NewGuid());

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Title, Is.EqualTo("New Topic"));
        Assert.That(result.IsActive, Is.True);
    }

    [Test]
    public void CreateTopicAsync_TutorNotTutor_ThrowsException()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new TopicService(context, _mockUserService.Object);
        _mockUserService.Setup(us => us.UserHasRoleAsync(It.IsAny<Guid>(), "TUTOR")).ReturnsAsync(false);

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await service.CreateTopicAsync("New Topic", "Description", "Subject Area", Guid.NewGuid()));
    }

    [Test]
    public async Task UpdateTopicAsync_ValidData_UpdatesTopic()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new TopicService(context, _mockUserService.Object);
        var topicId = Guid.NewGuid();
        var topic = new Topic { Id = topicId, Title = "Old Title", Description = "Old Desc", SubjectArea = "Old Area", IsActive = true };
        context.Topics.Add(topic);
        await context.SaveChangesAsync();

        // Act
        var result = await service.UpdateTopicAsync(topicId, "New Title", "New Desc", "New Area", false);

        // Assert
        Assert.That(result.Title, Is.EqualTo("New Title"));
        Assert.That(result.Description, Is.EqualTo("New Desc"));
        Assert.That(result.SubjectArea, Is.EqualTo("New Area"));
        Assert.That(result.IsActive, Is.False);
    }

    [Test]
    public void UpdateTopicAsync_NonExistingTopic_ThrowsException()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new TopicService(context, _mockUserService.Object);

        // Act & Assert
        Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            await service.UpdateTopicAsync(Guid.NewGuid(), "New Title", null, null, null));
    }

    [Test]
    public async Task DeleteTopicAsync_ExistingTopic_ReturnsTrue()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new TopicService(context, _mockUserService.Object);
        var topicId = Guid.NewGuid();
        var topic = new Topic { Id = topicId, Title = "Test Topic", Description = "Test Desc", SubjectArea = "Test Area" };
        context.Topics.Add(topic);
        await context.SaveChangesAsync();

        // Act
        var result = await service.DeleteTopicAsync(topicId);

        // Assert
        Assert.That(result, Is.True);
        var deleted = await context.Topics.FindAsync(topicId);
        Assert.That(deleted, Is.Null);
    }

    [Test]
    public async Task DeleteTopicAsync_NonExistingTopic_ReturnsFalse()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new TopicService(context, _mockUserService.Object);

        // Act
        var result = await service.DeleteTopicAsync(Guid.NewGuid());

        // Assert
        Assert.That(result, Is.False);
    }
}*/
