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
    [Route("thesis-offer-applications")]
    [Authorize]
    public class ThesisOfferApplicationController : ControllerBase
    {
        private readonly IThesisOfferBusinessLogicService _thesisOfferService;
        private readonly IThesisOfferApplicationApiMapper _thesisOfferApplicationApiMapper;

        public ThesisOfferApplicationController(IThesisOfferBusinessLogicService thesisOfferService, IThesisOfferApplicationApiMapper thesisOfferApplicationApiMapper)
        {
            _thesisOfferService = thesisOfferService;
            _thesisOfferApplicationApiMapper = thesisOfferApplicationApiMapper;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<ThesisOfferApplicationResponse>>> GetMyApplications([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _thesisOfferService.GetApplicationsForUserAsync(userId, page, pageSize);

            var response = new PaginatedResponse<ThesisOfferApplicationResponse>
            {
                Items = result.Items.Select(_thesisOfferApplicationApiMapper.MapToResponse).ToList(),
                TotalCount = result.TotalCount,
                Page = page,
                PageSize = pageSize
            };
            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "STUDENT")]
        public async Task<ActionResult<ThesisOfferApplicationResponse>> Create([FromBody] CreateThesisOfferApplicationRequest request)
        {
            var studentId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var (created, error) = await _thesisOfferService.CreateApplicationAsync(new ThesisOfferApplicationCreateRequestBusinessLogicModel
            {
                ThesisOfferId = request.ThesisOfferId,
                StudentId = studentId,
                Message = request.Message
            });

            if (error != null)
            {
                return BadRequest(error);
            }

            return CreatedAtAction(nameof(GetMyApplications), new { }, _thesisOfferApplicationApiMapper.MapToResponse(created!));
        }
    }
}
