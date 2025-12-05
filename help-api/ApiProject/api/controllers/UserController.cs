using ApiProject.Db.Entities;
using ApiProject.Logic.Services;
using ApiProject.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiProject.Api.Controllers;

[ApiController]
[Route("users")]
public sealed class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
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
        var result = await _userService.GetAllAsync(page, pageSize, email, firstName, lastName, role);
        var userResponses = result.Items
            .Select(u => new UserResponse
            {
                Id        = u.Id,
                FirstName = u.FirstName,
                LastName  = u.LastName,
                Email     = u.Email,
                Roles     = u.UserRoles
                    .Where(ur => ur.Role is not null)
                    .Select(ur => ur.Role!.Name)
                    .ToList()
            });

        var response = new PaginatedResponse<UserResponse>
        {
            Items = userResponses.ToList(),
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize
        };

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<UserResponse>> Create([FromBody] CreateUserRequest request)
    {
        var user = await _userService.CreateUserAsync(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password,
            request.Roles ?? Enumerable.Empty<string>());

        var response = new UserResponse
        {
            Id        = user.Id,
            FirstName = user.FirstName,
            LastName  = user.LastName,
            Email     = user.Email,
            Roles     = user.UserRoles
                .Where(ur => ur.Role is not null)
                .Select(ur => ur.Role!.Name)
                .ToList()
        };

        return CreatedAtAction(nameof(GetAll), new { id = response.Id }, response);
    }
}