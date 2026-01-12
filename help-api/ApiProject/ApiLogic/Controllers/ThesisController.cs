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
        private readonly IThesisBusinessLogicService _thesisBusinessLogicService;
        private readonly IThesisApiMapper _thesisApiMapper;

        public ThesisController(IThesisBusinessLogicService thesisBusinessLogicService, IThesisApiMapper thesisApiMapper)
        {
            _thesisBusinessLogicService = thesisBusinessLogicService;
            _thesisApiMapper = thesisApiMapper;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<ThesisResponse>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            var result = await _thesisBusinessLogicService.GetAllAsync(page, pageSize, userId, userRoles);
            
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
            var thesis = await _thesisBusinessLogicService.GetByIdAsync(id);
            if (thesis == null) return NotFound();
            return Ok(_thesisApiMapper.MapToResponse(thesis));
        }

        [HttpPost]
        [Authorize(Roles = "STUDENT")]
        public async Task<ActionResult<ThesisResponse>> Create([FromForm] CreateThesisApiRequest request)
        {
            var ownerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var businessModel = await _thesisApiMapper.MapToCreateBusinessModel(request, ownerId);
            var created = await _thesisBusinessLogicService.CreateThesisAsync(businessModel);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, _thesisApiMapper.MapToResponse(created));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ThesisResponse>> Update(Guid id, [FromForm] UpdateThesisRequest request)
        {
            try
            {
                var businessModel = await _thesisApiMapper.MapToUpdateBusinessModel(request);
                var updated = await _thesisBusinessLogicService.UpdateThesisAsync(id, businessModel);
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
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            var result = await _thesisBusinessLogicService.DeleteThesisAsync(id, userId, userRoles);
            return result switch
            {
                DeleteThesisResult.NotFound => NotFound(),
                DeleteThesisResult.NotAuthorized => Forbid(),
                DeleteThesisResult.Deleted => NoContent(),
                _ => throw new InvalidOperationException("Unexpected delete result")
            };
        }

        [HttpGet("billing-statuses")]
        public async Task<ActionResult<List<BillingStatusResponse>>> GetBillingStatuses()
        {
            var billingStatuses = await _thesisBusinessLogicService.GetAllBillingStatusesAsync();
            var response = billingStatuses.Select(bs => new BillingStatusResponse
            {
                Id = bs.Id,
                Name = bs.Name
            }).ToList();
            return Ok(response);
        }

        [HttpPatch("{id}/billing-status")]
        [Authorize(Roles = "ADMIN,TUTOR")]
        public async Task<ActionResult<ThesisResponse>> UpdateBillingStatus(Guid id, [FromBody] UpdateBillingStatusRequest request)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

                var updated = await _thesisBusinessLogicService.UpdateBillingStatusAsync(id, request.BillingStatusId, userId, userRoles);
                return Ok(_thesisApiMapper.MapToResponse(updated));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
        }
    }
}
