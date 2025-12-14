using ApiProject.ApiLogic.Mappers;
using ApiProject.ApiLogic.Models;
using ApiProject.BusinessLogic.Models;
using ApiProject.BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApiProject.ApiLogic.Controllers
{
    [ApiController]
    [Route("theses")]
    [Authorize]
    public sealed class ThesisController : ControllerBase
    {
        private readonly IThesisService _thesisService;
        private readonly IThesisApiMapper _thesisApiMapper;

        public ThesisController(IThesisService thesisService, IThesisApiMapper thesisApiMapper)
        {
            _thesisService = thesisService;
            _thesisApiMapper = thesisApiMapper;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<ThesisResponse>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            var result = await _thesisService.GetAllAsync(page, pageSize, userId, userRoles);
            
            var response = new PaginatedResponse<ThesisResponse>
            {
                Items = result.Items.Select(_thesisApiMapper.MapToResponse).ToList(),
                TotalCount = result.TotalCount,
                Page = page, // Ensure page is reflected from request if result is ambiguous
                PageSize = pageSize // Ensure pageSize is reflected from request
            };
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ThesisResponse>> GetById(Guid id)
        {
            var thesis = await _thesisService.GetByIdAsync(id);
            if (thesis == null) return NotFound();
            return Ok(_thesisApiMapper.MapToResponse(thesis));
        }

        [HttpPost]
        public async Task<ActionResult<ThesisResponse>> Create([FromBody] CreateThesisRequest request)
        {
            var ownerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var created = await _thesisService.CreateThesisAsync(new ThesisCreateRequestBusinessLogicModel
            {
                Title = request.Title,
                OwnerId = ownerId,
                TopicId = request.TopicId
            });

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, _thesisApiMapper.MapToResponse(created));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ThesisResponse>> Update(Guid id, [FromBody] UpdateThesisRequest request)
        {
            try
            {
                var updated = await _thesisService.UpdateThesisAsync(id, new ThesisUpdateRequestBusinessLogicModel
                {
                    Title = request.Title,
                    TopicId = request.TopicId
                });
                return Ok(_thesisApiMapper.MapToResponse(updated));
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
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
