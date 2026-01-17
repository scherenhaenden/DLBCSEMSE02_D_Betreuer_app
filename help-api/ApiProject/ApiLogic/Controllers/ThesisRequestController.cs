using ApiProject.ApiLogic.Models;
using ApiProject.BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ApiProject.BusinessLogic.Models;

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
            try
            {
                var requesterId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                var createdRequest = await _requestBusinessLogicService.CreateRequestAsync(
                    requesterId,
                    request.ThesisId,
                    request.ReceiverId,
                    request.RequestType,
                    request.Message,
                    request.PlannedStartOfSupervision,
                    request.PlannedEndOfSupervision
                );

                return CreatedAtAction(nameof(GetRequestById), new { id = createdRequest.Id }, MapToResponse(createdRequest));
            }
            catch (Exception ex)
            {
                // Return the exact message to the client so Android can show a meaningful Toast
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<ThesisRequestResponse>>> GetMyRequests([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _requestBusinessLogicService.GetRequestsForUserAsync(userId, page, pageSize);
            var response = new PaginatedResponse<ThesisRequestResponse>
            {
                Items = result.Items.Select(MapToResponse).ToList(),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ThesisRequestResponse>> GetRequestById(Guid id)
        {
            var request = await _requestBusinessLogicService.GetRequestByIdAsync(id);
            if (request == null) return NotFound();
            return Ok(MapToResponse(request));
        }

        [HttpPost("{id}/respond")]
        public async Task<IActionResult> RespondToRequest(Guid id, [FromBody] RespondToThesisRequestRequest response)
        {
            var receiverId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _requestBusinessLogicService.RespondToRequestAsync(id, receiverId, response.Accepted, response.Message);
            return NoContent();
        }

        [HttpGet("tutor/receiver")]
        public async Task<ActionResult<PaginatedResponse<ThesisRequestResponse>>> GetRequestsForTutorAsReceiver([FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var tutorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _requestBusinessLogicService.GetRequestsForTutorAsReceiver(tutorId, page, pageSize, status);
            var response = new PaginatedResponse<ThesisRequestResponse>
            {
                Items = result.Items.Select(MapToResponse).ToList(),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };
            return Ok(response);
        }

        [HttpGet("tutor/requester")]
        public async Task<ActionResult<PaginatedResponse<ThesisRequestResponse>>> GetRequestsForTutorAsRequester([FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var tutorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _requestBusinessLogicService.GetRequestsForTutorAsRequester(tutorId, page, pageSize, status);
            var response = new PaginatedResponse<ThesisRequestResponse>
            {
                Items = result.Items.Select(MapToResponse).ToList(),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequest(Guid id)
        {
            var requesterId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _requestBusinessLogicService.DeleteRequestAsync(id, requesterId);
            return result ? NoContent() : NotFound();
        }

        private ThesisRequestResponse MapToResponse(ThesisRequestBusinessLogicModel model)
        {
            return new ThesisRequestResponse
            {
                Id = model.Id,
                ThesisId = model.ThesisId,
                ThesisTitle = model.ThesisTitle,
                Requester = new UserResponse
                {
                    Id = model.Requester.Id,
                    FirstName = model.Requester.FirstName,
                    LastName = model.Requester.LastName,
                    Email = model.Requester.Email,
                    Roles = model.Requester.Roles
                },
                Receiver = new UserResponse
                {
                    Id = model.Receiver.Id,
                    FirstName = model.Receiver.FirstName,
                    LastName = model.Receiver.LastName,
                    Email = model.Receiver.Email,
                    Roles = model.Receiver.Roles
                },
                RequestType = model.RequestType,
                Status = model.Status,
                Message = model.Message,
                CreatedAt = model.CreatedAt,
                PlannedStartOfSupervision = model.PlannedStartOfSupervision,
                PlannedEndOfSupervision = model.PlannedEndOfSupervision,
                DocumentFileName = model.DocumentFileName,
                DocumentId = model.DocumentId
            };
        }
    }
}
