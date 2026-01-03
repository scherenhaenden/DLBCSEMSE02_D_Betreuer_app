using ApiProject.ApiLogic.Mappers;
using ApiProject.ApiLogic.Models;
using ApiProject.BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ApiProject.BusinessLogic.Models;

namespace ApiProject.ApiLogic.Controllers
{
    [ApiController]
    [Route("thesis-offers")]
    [Authorize]
    public class ThesisOfferController : ControllerBase
    {
        private readonly IThesisOfferBusinessLogicService _thesisOfferService;
        private readonly IThesisOfferApiMapper _thesisOfferApiMapper;

        public ThesisOfferController(IThesisOfferBusinessLogicService thesisOfferService, IThesisOfferApiMapper thesisOfferApiMapper)
        {
            _thesisOfferService = thesisOfferService;
            _thesisOfferApiMapper = thesisOfferApiMapper;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<ThesisOfferResponse>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            var result = await _thesisOfferService.GetAllAsync(page, pageSize, userId, userRoles);

            var response = new PaginatedResponse<ThesisOfferResponse>
            {
                Items = result.Items.Select(_thesisOfferApiMapper.MapToResponse).ToList(),
                TotalCount = result.TotalCount,
                Page = page,
                PageSize = pageSize
            };
            return Ok(response);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<PaginatedResponse<ThesisOfferResponse>>> GetByUserId(Guid userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            var result = await _thesisOfferService.GetByUserIdAsync(userId, currentUserId, userRoles, page, pageSize);

            var response = new PaginatedResponse<ThesisOfferResponse>
            {
                Items = result.Items.Select(_thesisOfferApiMapper.MapToResponse).ToList(),
                TotalCount = result.TotalCount,
                Page = page,
                PageSize = pageSize
            };
            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "TUTOR")]
        public async Task<ActionResult<ThesisOfferResponse>> Create([FromBody] CreateThesisOfferRequest request)
        {
            var tutorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var created = await _thesisOfferService.CreateAsync(new ThesisOfferCreateRequestBusinessLogicModel
            {
                Title = request.Title,
                Description = request.Description,
                SubjectAreaId = request.SubjectAreaId,
                TutorId = tutorId,
                MaxStudents = request.MaxStudents,
                ExpiresAt = request.ExpiresAt
            });

            return CreatedAtAction(nameof(GetAll), new { }, _thesisOfferApiMapper.MapToResponse(created));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "TUTOR")]
        public async Task<ActionResult<ThesisOfferResponse>> Update(Guid id, [FromBody] UpdateThesisOfferRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var updated = await _thesisOfferService.UpdateAsync(id, new ThesisOfferUpdateRequestBusinessLogicModel
            {
                Title = request.Title,
                Description = request.Description,
                SubjectAreaId = request.SubjectAreaId,
                MaxStudents = request.MaxStudents,
                ExpiresAt = request.ExpiresAt
            }, userId);

            return Ok(_thesisOfferApiMapper.MapToResponse(updated));
        }
    }
}
