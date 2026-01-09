using ApiProject.ApiLogic.Models;
using ApiProject.BusinessLogic.Services;
using ApiProject.BusinessLogic.Models;
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
        /// Gets a paginated list of tutors, optionally filtered by subject area, name, or last name.
        /// </summary>
        /// <param name="subjectAreaId">The ID of the subject area to filter by.</param>
        /// <param name="subjectAreaName">The name (or partial name) of the subject area to filter by (case-insensitive).</param>
        /// <param name="name">A string to search for in the tutor's first or last name (case-insensitive).</param>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>A paginated list of tutors.</returns>
        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<TutorProfileResponse>>> GetTutors(
            [FromQuery] Guid? subjectAreaId, 
            [FromQuery] string? subjectAreaName, 
            [FromQuery] string? name,
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            var result = await _userBusinessLogicService.GetTutorsAsync(subjectAreaId, subjectAreaName, name, page, pageSize);
            var response = new PaginatedResponse<TutorProfileResponse>
            {
                Items = result.Items.Select(MapToTutorProfileResponse).ToList(),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };
            return Ok(response);
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
            return Ok(MapToTutorProfileResponse(tutor));
        }

        private TutorProfileResponse MapToTutorProfileResponse(TutorProfileBusinessLogicModel tutor)
        {
            return new TutorProfileResponse
            {
                Id = tutor.Id,
                FirstName = tutor.FirstName,
                LastName = tutor.LastName,
                Email = tutor.Email,
                SubjectAreas = tutor.SubjectAreas.Select(sa => new SubjectAreaResponse
                {
                    Id = sa.Id,
                    Title = sa.Title,
                    Description = sa.Description,
                    SubjectArea = sa.Title, // Assuming SubjectArea is the Title
                    IsActive = sa.IsActive,
                    TutorIds = sa.TutorIds
                }).ToList()
            };
        }
    }
}
