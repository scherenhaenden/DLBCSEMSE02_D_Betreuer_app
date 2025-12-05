using ApiProject.Logic.Services;
using ApiProject.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiProject.Api.Controllers;

[ApiController]
[Route("topics")]
public sealed class TopicController : ControllerBase
{
    private readonly ITopicService _topicService;

    public TopicController(ITopicService topicService)
    {
        _topicService = topicService;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<TopicResponse>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _topicService.GetAllAsync(page, pageSize);
        var topicResponses = result.Items
            .Select(t => new TopicResponse
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                SubjectArea = t.SubjectArea,
                IsActive = t.IsActive,
                TutorId = t.TutorId
            });

        var response = new PaginatedResponse<TopicResponse>
        {
            Items = topicResponses.ToList(),
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize
        };

        return Ok(response);
    }

    [HttpGet("search")]
    public async Task<ActionResult<PaginatedResponse<TopicResponse>>> Search([FromQuery] string q, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _topicService.SearchAsync(q, page, pageSize);
        var topicResponses = result.Items
            .Select(t => new TopicResponse
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                SubjectArea = t.SubjectArea,
                IsActive = t.IsActive,
                TutorId = t.TutorId
            });

        var response = new PaginatedResponse<TopicResponse>
        {
            Items = topicResponses.ToList(),
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize
        };

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TopicResponse>> GetById(Guid id)
    {
        var topic = await _topicService.GetByIdAsync(id);
        if (topic is null)
        {
            return NotFound();
        }

        var response = new TopicResponse
        {
            Id = topic.Id,
            Title = topic.Title,
            Description = topic.Description,
            SubjectArea = topic.SubjectArea,
            IsActive = topic.IsActive,
            TutorId = topic.TutorId
        };

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<TopicResponse>> Create([FromBody] CreateTopicRequest request)
    {
        try
        {
            var topic = await _topicService.CreateTopicAsync(
                request.Title,
                request.Description,
                request.SubjectArea,
                request.TutorId);

            var response = new TopicResponse
            {
                Id = topic.Id,
                Title = topic.Title,
                Description = topic.Description,
                SubjectArea = topic.SubjectArea,
                IsActive = topic.IsActive,
                TutorId = topic.TutorId
            };

            return CreatedAtAction(nameof(GetById), new { id = topic.Id }, response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TopicResponse>> Update(Guid id, [FromBody] UpdateTopicRequest request)
    {
        try
        {
            var topic = await _topicService.UpdateTopicAsync(
                id,
                request.Title,
                request.Description,
                request.SubjectArea,
                request.IsActive);

            var response = new TopicResponse
            {
                Id = topic.Id,
                Title = topic.Title,
                Description = topic.Description,
                SubjectArea = topic.SubjectArea,
                IsActive = topic.IsActive,
                TutorId = topic.TutorId
            };

            return Ok(response);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var deleted = await _topicService.DeleteTopicAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
