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
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetAll()
    {
        var users = (await _userService.GetAllAsync())
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

        return Ok(users);
    }

    [HttpPost]
    public async Task<ActionResult<UserResponse>> Create([FromBody] CreateUserRequest request)
    {
        var user = await _userService.CreateUserAsync(
            request.FirstName,
            request.LastName,
            request.Email,
            request.PasswordHash,
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