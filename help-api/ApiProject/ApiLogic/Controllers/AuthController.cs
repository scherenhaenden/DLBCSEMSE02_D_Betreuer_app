using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiProject.ApiLogic.Models;
using ApiProject.BusinessLogic.Services;
using BL = ApiProject.BusinessLogic.Models;

namespace ApiProject.ApiLogic.Controllers
{
    [ApiController]
    [Route("auth")]
    public sealed class AuthController : ControllerBase
    {
        private readonly IUserBusinessLogicService _userBusinessLogicService;
        private readonly IConfiguration _configuration;

        public AuthController(IUserBusinessLogicService userBusinessLogicService, IConfiguration configuration)
        {
            _userBusinessLogicService = userBusinessLogicService;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            var isValid = await _userBusinessLogicService.VerifyPasswordAsync(request.Email, request.Password);
            if (!isValid)
            {
                return Unauthorized("Invalid email or password.");
            }

            var user = await _userBusinessLogicService.GetByEmailAsync(request.Email);
            if (user == null)
            {
                return Unauthorized();
            }

            var token = GenerateJwtToken(user);

            var userResponse = new UserResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Roles = user.Roles
            };

            var response = new LoginResponse
            {
                Token = token,
                User = userResponse
            };

            return Ok(response);
        }

        private string GenerateJwtToken(BL.UserBusinessLogicModel user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("loginId", user.Id.ToString())
            };

            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "default-secret-key"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
