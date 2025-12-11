using ApiProject.ApiLogic.Models;
using ApiProject.BusinessLogic.Models;
using ApiProject.BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;


namespace ApiProject.ApiLogic.Controllers
{
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
        public async Task<ActionResult<PaginatedResponse<ThesisResponse>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _thesisService.GetAllAsync(page, pageSize);
            var response = new PaginatedResponse<ThesisResponse>
            {
                Items = result.Items.Select(MapToResponse).ToList(),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ThesisResponse>> GetById(Guid id)
        {
            var thesis = await _thesisService.GetByIdAsync(id);
            if (thesis == null)
            {
                return NotFound();
            }
            return Ok(MapToResponse(thesis));
        }

        [HttpPost]
        public async Task<ActionResult<ThesisResponse>> Create([FromBody] CreateThesisRequest request)
        {
            var created = await _thesisService.CreateThesisAsync(new ThesisCreateRequestBusinessLogicModel
            {
                Title = request.Title,
                SubjectArea = request.SubjectArea,
                OwnerId = request.OwnerId,
                TutorId = request.TutorId,
                SecondSupervisorId = request.SecondSupervisorId,
                TopicId = request.TopicId
            });

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToResponse(created));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ThesisResponse>> Update(Guid id, [FromBody] UpdateThesisRequest request)
        {
            try
            {
                var updated = await _thesisService.UpdateThesisAsync(id, new ThesisUpdateRequestBusinessLogicModel
                {
                    Title = request.Title,
                    SubjectArea = request.SubjectArea,
                    //StatusId = request.StatusId,
                    //BillingStatusId = request.BillingStatusId,
                    TutorId = request.TutorId,
                    SecondSupervisorId = request.SecondSupervisorId,
                    TopicId = request.TopicId
                });
                return Ok(MapToResponse(updated));
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

        private static ThesisResponse MapToResponse(ThesisBusinessLogicModel thesis)
        {
            return new ThesisResponse
            {
                Id = thesis.Id,
                Title = thesis.Title,
                SubjectArea = thesis.SubjectArea,
                Status = thesis.Status,
                //BillingStatus = thesis.BillingStatus,
                OwnerId = thesis.OwnerId,
                TutorId = thesis.TutorId,
                SecondSupervisorId = thesis.SecondSupervisorId,
                TopicId = thesis.TopicId,
                DocumentFileName = thesis.DocumentFileName
            };
        }
    }
}
