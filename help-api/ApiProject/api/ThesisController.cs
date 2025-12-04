using ApiProject.Db;
using ApiProject.Logic;
using Microsoft.AspNetCore.Mvc;

namespace ApiProject.Api;

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
    public ActionResult<IEnumerable<Thesis>> GetAll()
    {
        return Ok(_thesisService.GetAll());
    }

    [HttpGet("{id}")]
    public ActionResult<Thesis> GetById(Guid id)
    {
        var thesis = _thesisService.GetById(id);
        if (thesis is null)
        {
            return NotFound();
        }
        return Ok(thesis);
    }

    [HttpPost]
    public ActionResult<Thesis> Create([FromBody] CreateThesisApiRequest request)
    {
        var created = _thesisService.CreateThesis(new ThesisCreateRequest
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
    public ActionResult<Thesis> Update(Guid id, [FromBody] UpdateThesisApiRequest request)
    {
        try
        {
            var updated = _thesisService.UpdateThesis(id, new ThesisUpdateRequest
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
    public ActionResult Delete(Guid id)
    {
        var deleted = _thesisService.DeleteThesis(id);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }
}