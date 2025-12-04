namespace ApiProject.Api.Models;

/// <summary>
/// Request model for user login.
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// The user's email.
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// The user's password.
    /// </summary>
    public required string Password { get; set; }
}
