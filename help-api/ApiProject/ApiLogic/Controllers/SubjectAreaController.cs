using ApiProject.ApiLogic.Models;
using ApiProject.BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;
using BL = ApiProject.BusinessLogic.Models;

namespace ApiProject.ApiLogic.Controllers
{
    [ApiController]
    [Route("subject-areas")]
    public sealed class SubjectAreaController : ControllerBase
    {
        private readonly ISubjectAreaBusinessLogicService _subjectAreaBusinessLogicService;

        public SubjectAreaController(ISubjectAreaBusinessLogicService subjectAreaBusinessLogicService)
        {
            _subjectAreaBusinessLogicService = subjectAreaBusinessLogicService;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<SubjectAreaResponse>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _subjectAreaBusinessLogicService.GetAllAsync(page, pageSize);
            var response = new PaginatedResponse<SubjectAreaResponse>
            {
                Items = result.Items.Select(MapToResponse).ToList(),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };
            return Ok(response);
        }
        
        [HttpGet("search")]
        public async Task<ActionResult<PaginatedResponse<SubjectAreaResponse>>> Search([FromQuery] string q, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _subjectAreaBusinessLogicService.SearchAsync(q, page, pageSize);
            var response = new PaginatedResponse<SubjectAreaResponse>
            {
                Items = result.Items.Select(MapToResponse).ToList(),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SubjectAreaResponse>> GetById(Guid id)
        {
            var subjectArea = await _subjectAreaBusinessLogicService.GetByIdAsync(id);
            if (subjectArea == null)
            {
                return NotFound();
            }
            return Ok(MapToResponse(subjectArea));
        }

        [HttpPost]
        public async Task<ActionResult<SubjectAreaResponse>> Create([FromBody] CreateSubjectAreaRequest request)
        {
            try
            {
                var created = await _subjectAreaBusinessLogicService.CreateSubjectAreaAsync(new BL.SubjectAreaCreateRequestBusinessLogicModel
                {
                    Title = request.Title,
                    Description = request.Description,
                    SubjectArea = request.SubjectArea,
                    TutorIds = request.TutorIds
                });
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToResponse(created));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<SubjectAreaResponse>> Update(Guid id, [FromBody] UpdateSubjectAreaRequest request)
        {
            try
            {
                var updated = await _subjectAreaBusinessLogicService.UpdateSubjectAreaAsync(id, new BL.SubjectAreaUpdateRequestBusinessLogicModel
                {
                    Title = request.Title,
                    Description = request.Description,
                    SubjectArea = request.SubjectArea,
                    IsActive = request.IsActive,
                    TutorIds = request.TutorIds
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
            var deleted = await _subjectAreaBusinessLogicService.DeleteSubjectAreaAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }

        private static SubjectAreaResponse MapToResponse(BL.SubjectAreaBusinessLogicModel subjectArea)
        {
            return new SubjectAreaResponse
            {
                Id = subjectArea.Id,
                Title = subjectArea.Title,
                Description = subjectArea.Description,
                IsActive = subjectArea.IsActive,
                TutorIds = subjectArea.TutorIds
            };
        }
    }
}
