using ApiProject.ApiLogic.Models;
using ApiProject.BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApiProject.ApiLogic.Controllers
{
    [ApiController]
    [Route("thesis-requests")]
    [Authorize]
    public class ThesisRequestController : ControllerBase
    {
        private readonly IThesisRequestBusinessLogicService _requestBusinessLogicService;

        public ThesisRequestController(IThesisRequestBusinessLogicService requestBusinessLogicService)
        {
            _requestBusinessLogicService = requestBusinessLogicService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRequest([FromBody] CreateThesisRequestRequest request)
        {
            var requesterId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var createdRequest = await _requestBusinessLogicService.CreateRequestAsync(requesterId, request.ThesisId, request.ReceiverId, request.RequestType, request.Message);
            return CreatedAtAction(nameof(GetRequestById), new { id = createdRequest.Id }, createdRequest);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ThesisRequestResponse>>> GetMyRequests()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var requests = await _requestBusinessLogicService.GetRequestsForUserAsync(userId);
            return Ok(requests);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ThesisRequestResponse>> GetRequestById(Guid id)
        {
            var request = await _requestBusinessLogicService.GetRequestByIdAsync(id);
            if (request == null) return NotFound();
            return Ok(request);
        }

        [HttpPost("{id}/respond")]
        public async Task<IActionResult> RespondToRequest(Guid id, [FromBody] RespondToThesisRequestRequest response)
        {
            var receiverId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _requestBusinessLogicService.RespondToRequestAsync(id, receiverId, response.Accepted, response.Message);
            return NoContent();
        }
    }
}
