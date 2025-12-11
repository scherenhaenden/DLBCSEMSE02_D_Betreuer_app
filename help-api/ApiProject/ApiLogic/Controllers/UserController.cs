using ApiProject.ApiLogic.Models;
using ApiProject.BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiProject.ApiLogic.Controllers
{
    [ApiController]
    [Route("users")]
    public sealed class UserController : ControllerBase
    {
        private readonly IUserBusinessLogicService _userBusinessLogicService;

        public UserController(IUserBusinessLogicService userBusinessLogicService)
        {
            _userBusinessLogicService = userBusinessLogicService;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<UserResponse>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? email = null,
            [FromQuery] string? firstName = null,
            [FromQuery] string? lastName = null,
            [FromQuery] string? role = null)
        {
            var result = await _userBusinessLogicService.GetAllAsync(page, pageSize, email, firstName, lastName, role);

            var response = new PaginatedResponse<UserResponse>
            {
                Items = result.Items.Select(u => new UserResponse
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Roles = u.Roles
                }).ToList(),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<UserResponse>> Create([FromBody] CreateUserRequest request)
        {
            var user = await _userBusinessLogicService.CreateUserAsync(
                request.FirstName,
                request.LastName,
                request.Email,
                request.Password,
                request.Roles);

            var response = new UserResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Roles = user.Roles
            };

            return CreatedAtAction(nameof(GetAll), new { id = response.Id }, response);
        }
    }
}
