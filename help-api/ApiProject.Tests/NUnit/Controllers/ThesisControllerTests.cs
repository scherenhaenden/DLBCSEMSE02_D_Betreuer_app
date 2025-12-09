/*using ApiProject.Api.Controllers;
using ApiProject.ApiLogic.models;
using ApiProject.Logic.Models;
using ApiProject.Logic.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ApiProject.Tests.NUnit.Controllers;

[TestFixture]
public class ThesisControllerTests
{
    private ThesisController _controller = null!;
    private Mock<IThesisService> _mockThesisService = null!;

    [SetUp]
    public void Setup()
    {
        _mockThesisService = new Mock<IThesisService>();
        _controller = new ThesisController(_mockThesisService.Object);
    }

    [Test]
    public async Task GetAll_ReturnsOkWithPaginatedResponse()
    {
        // Arrange
        var theses = new List<Thesis>
        {
            new Thesis { Id = Guid.NewGuid(), Title = "Thesis 1" },
            new Thesis { Id = Guid.NewGuid(), Title = "Thesis 2" }
        };
        var paginatedResult = new PaginatedResult<Thesis> { Items = theses, TotalCount = 2, Page = 1, PageSize = 10 };
        _mockThesisService.Setup(s => s.GetAllAsync(1, 10)).ReturnsAsync(paginatedResult);

        // Act
        var result = await _controller.GetAll(1, 10);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        var response = okResult!.Value as PaginatedResponse<Thesis>;
        Assert.That(response!.Items.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task GetById_ExistingThesis_ReturnsOk()
    {
        // Arrange
        var thesisId = Guid.NewGuid();
        var thesis = new Thesis { Id = thesisId, Title = "Test Thesis" };
        _mockThesisService.Setup(s => s.GetByIdAsync(thesisId)).ReturnsAsync(thesis);

        // Act
        var result = await _controller.GetById(thesisId);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult!.Value, Is.EqualTo(thesis));
    }

    [Test]
    public async Task GetById_NonExistingThesis_ReturnsNotFound()
    {
        // Arrange
        _mockThesisService.Setup(s => s.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Thesis?)null);

        // Act
        var result = await _controller.GetById(Guid.NewGuid());

        // Assert
        Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task Create_ValidRequest_ReturnsCreated()
    {
        // Arrange
        var request = new CreateThesisApiRequest
        {
            Title = "New Thesis",
            OwnerId = Guid.NewGuid(),
            TutorId = Guid.NewGuid(),
            TopicId = Guid.NewGuid(),
            ProgressPercent = 0
        };
        var createdThesis = new Thesis { Id = Guid.NewGuid(), Title = "New Thesis" };
        _mockThesisService.Setup(s => s.CreateThesisAsync(It.IsAny<ThesisCreateRequest>())).ReturnsAsync(createdThesis);

        // Act
        var result = await _controller.Create(request);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
    }

    [Test]
    public async Task Update_ValidRequest_ReturnsOk()
    {
        // Arrange
        var thesisId = Guid.NewGuid();
        var request = new UpdateThesisApiRequest { Title = "Updated Title" };
        var updatedThesis = new Thesis { Id = thesisId, Title = "Updated Title" };
        _mockThesisService.Setup(s => s.UpdateThesisAsync(thesisId, It.IsAny<ThesisUpdateRequest>())).ReturnsAsync(updatedThesis);

        // Act
        var result = await _controller.Update(thesisId, request);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task Update_NonExistingThesis_ReturnsNotFound()
    {
        // Arrange
        _mockThesisService.Setup(s => s.UpdateThesisAsync(It.IsAny<Guid>(), It.IsAny<ThesisUpdateRequest>()))
            .ThrowsAsync(new KeyNotFoundException());

        var request = new UpdateThesisApiRequest { Title = "Updated Title" };

        // Act
        var result = await _controller.Update(Guid.NewGuid(), request);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task Delete_ExistingThesis_ReturnsNoContent()
    {
        // Arrange
        _mockThesisService.Setup(s => s.DeleteThesisAsync(It.IsAny<Guid>())).ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(Guid.NewGuid());

        // Assert
        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public async Task Delete_NonExistingThesis_ReturnsNotFound()
    {
        // Arrange
        _mockThesisService.Setup(s => s.DeleteThesisAsync(It.IsAny<Guid>())).ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(Guid.NewGuid());

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }
}
*/