using ApiProject.ApiLogic.Models;
using ApiProject.BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiProject.ApiLogic.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TutorController : ControllerBase
    {
        private readonly IUserBusinessLogicService _userBusinessLogicService;

        public TutorController(IUserBusinessLogicService userBusinessLogicService)
        {
            _userBusinessLogicService = userBusinessLogicService;
        }

        /// <summary>
        /// Gets a paginated list of tutors, optionally filtered by topic ID or topic name.
        /// </summary>
        /// <param name="topicId">The ID of the topic to filter by.</param>
        /// <param name="topicName">The name (or partial name) of the topic to filter by (case-insensitive).</param>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>A paginated list of tutors.</returns>
        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<TutorProfileResponse>>> GetTutors(
            [FromQuery] Guid? topicId, [FromQuery] string? topicName, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var tutors = await _userBusinessLogicService.GetTutorsAsync(topicId, topicName, page, pageSize);
            return Ok(tutors);
        }

        /// <summary>
        /// Gets the profile of a specific tutor by their ID.
        /// </summary>
        /// <param name="id">The ID of the tutor to retrieve.</param>
        /// <returns>The tutor's profile.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<TutorProfileResponse>> GetTutorById(Guid id)
        {
            var tutor = await _userBusinessLogicService.GetTutorByIdAsync(id);
            if (tutor == null)
            {
                return NotFound();
            }
            return Ok(tutor);
        }
    }
}
