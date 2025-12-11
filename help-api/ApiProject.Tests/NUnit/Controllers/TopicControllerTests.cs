/*using ApiProject.Api.Controllers;
using ApiProject.ApiLogic.models;
using ApiProject.Logic.Models;
using ApiProject.Logic.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ApiProject.Tests.NUnit.Controllers;

[TestFixture]
public class TopicControllerTests
{
    private TopicController _controller = null!;
    private Mock<ITopicService> _mockTopicService = null!;

    [SetUp]
    public void Setup()
    {
        _mockTopicService = new Mock<ITopicService>();
        _controller = new TopicController(_mockTopicService.Object);
    }

    [Test]
    public async Task GetAll_ReturnsOkWithPaginatedResponse()
    {
        // Arrange
        var topics = new List<Topic>
        {
            new Topic { Id = Guid.NewGuid(), Title = "Topic 1", Description = "Desc 1", SubjectArea = "Area 1" },
            new Topic { Id = Guid.NewGuid(), Title = "Topic 2", Description = "Desc 2", SubjectArea = "Area 2" }
        };
        var paginatedResult = new PaginatedResult<Topic> { Items = topics, TotalCount = 2, Page = 1, PageSize = 10 };
        _mockTopicService.Setup(s => s.GetAllAsync(1, 10)).ReturnsAsync(paginatedResult);

        // Act
        var result = await _controller.GetAll(1, 10);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        var response = okResult!.Value as PaginatedResponse<TopicResponse>;
        Assert.That(response!.Items.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task Search_ReturnsOkWithPaginatedResponse()
    {
        // Arrange
        var topics = new List<Topic>
        {
            new Topic { Id = Guid.NewGuid(), Title = "AI Topic", Description = "AI Desc", SubjectArea = "CS" }
        };
        var paginatedResult = new PaginatedResult<Topic> { Items = topics, TotalCount = 1, Page = 1, PageSize = 10 };
        _mockTopicService.Setup(s => s.SearchAsync("AI", 1, 10)).ReturnsAsync(paginatedResult);

        // Act
        var result = await _controller.Search("AI", 1, 10);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        var response = okResult!.Value as PaginatedResponse<TopicResponse>;
        Assert.That(response!.Items.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task GetById_ExistingTopic_ReturnsOk()
    {
        // Arrange
        var topicId = Guid.NewGuid();
        var topic = new Topic { Id = topicId, Title = "Test Topic", Description = "Test Desc", SubjectArea = "Test Area" };
        _mockTopicService.Setup(s => s.GetByIdAsync(topicId)).ReturnsAsync(topic);

        // Act
        var result = await _controller.GetById(topicId);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        var response = okResult!.Value as TopicResponse;
        Assert.That(response!.Id, Is.EqualTo(topicId));
    }

    [Test]
    public async Task GetById_NonExistingTopic_ReturnsNotFound()
    {
        // Arrange
        _mockTopicService.Setup(s => s.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Topic?)null);

        // Act
        var result = await _controller.GetById(Guid.NewGuid());

        // Assert
        Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task Create_ValidRequest_ReturnsCreated()
    {
        // Arrange
        var request = new CreateTopicRequest
        {
            Title = "New Topic",
            Description = "New Description",
            SubjectArea = "New Area",
            TutorId = Guid.NewGuid()
        };
        var createdTopic = new Topic { Id = Guid.NewGuid(), Title = "New Topic", Description = "New Description", SubjectArea = "New Area" };
        _mockTopicService.Setup(s => s.CreateTopicAsync("New Topic", "New Description", "New Area", request.TutorId)).ReturnsAsync(createdTopic);

        // Act
        var result = await _controller.Create(request);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
    }

    [Test]
    public async Task Update_ValidRequest_ReturnsOk()
    {
        // Arrange
        var topicId = Guid.NewGuid();
        var request = new UpdateTopicRequest { Title = "Updated Title" };
        var updatedTopic = new Topic { Id = topicId, Title = "Updated Title", Description = "Desc", SubjectArea = "Area" };
        _mockTopicService.Setup(s => s.UpdateTopicAsync(topicId, "Updated Title", null, null, null)).ReturnsAsync(updatedTopic);

        // Act
        var result = await _controller.Update(topicId, request);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task Update_NonExistingTopic_ReturnsNotFound()
    {
        // Arrange
        _mockTopicService.Setup(s => s.UpdateTopicAsync(It.IsAny<Guid>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<bool?>()))
            .ThrowsAsync(new KeyNotFoundException());

        var request = new UpdateTopicRequest { Title = "Updated Title" };

        // Act
        var result = await _controller.Update(Guid.NewGuid(), request);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task Delete_ExistingTopic_ReturnsNoContent()
    {
        // Arrange
        _mockTopicService.Setup(s => s.DeleteTopicAsync(It.IsAny<Guid>())).ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(Guid.NewGuid());

        // Assert
        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public async Task Delete_NonExistingTopic_ReturnsNotFound()
    {
        // Arrange
        _mockTopicService.Setup(s => s.DeleteTopicAsync(It.IsAny<Guid>())).ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(Guid.NewGuid());

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }
}*/
