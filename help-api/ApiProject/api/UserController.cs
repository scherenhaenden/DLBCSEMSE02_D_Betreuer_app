using ApiProject.Db;
using ApiProject.Logic;
using Microsoft.AspNetCore.Mvc;

namespace ApiProject.Api;

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
    public ActionResult<IEnumerable<UserResponse>> GetAll()
    {
        var users = _userService
            .GetAll()
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
    public ActionResult<UserResponse> Create([FromBody] CreateUserRequest request)
    {
        var user = _userService.CreateUser(
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