using ApiProject.Db.Entities;
using ApiProject.Logic.Services;
using ApiProject.Logic.Models;
using ApiProject.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiProject.Api.Controllers;

[ApiController]
[Route("theses")]
public sealed class ThesisController : ControllerBase
{
    private readonly IThesisService _thesisService;

    public ThesisController(IThesisService thesisService)
    {
        _thesisService = thesisService;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<Thesis>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _thesisService.GetAllAsync(page, pageSize);
        var response = new PaginatedResponse<Thesis>
        {
            Items = result.Items,
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize
        };
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Thesis>> GetById(Guid id)
    {
        var thesis = await _thesisService.GetByIdAsync(id);
        if (thesis is null)
        {
            return NotFound();
        }
        return Ok(thesis);
    }

    [HttpPost]
    public async Task<ActionResult<Thesis>> Create([FromBody] CreateThesisApiRequest request)
    {
        var created = await _thesisService.CreateThesisAsync(new ThesisCreateRequest
        {
            Title              = request.Title,
            OwnerId            = request.OwnerId,
            TutorId            = request.TutorId,
            SecondSupervisorId = request.SecondSupervisorId,
            TopicId            = request.TopicId,
            ProgressPercent    = request.ProgressPercent,
            ExposePath         = request.ExposePath,
            BillingStatus      = request.BillingStatus
        });

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Thesis>> Update(Guid id, [FromBody] UpdateThesisApiRequest request)
    {
        try
        {
            var updated = await _thesisService.UpdateThesisAsync(id, new ThesisUpdateRequest
            {
                Title              = request.Title,
                Status             = request.Status,
                ProgressPercent    = request.ProgressPercent,
                ExposePath         = request.ExposePath,
                BillingStatus      = request.BillingStatus,
                TutorId            = request.TutorId,
                SecondSupervisorId = request.SecondSupervisorId,
                TopicId            = request.TopicId
            });
            return Ok(updated);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var deleted = await _thesisService.DeleteThesisAsync(id);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }
}