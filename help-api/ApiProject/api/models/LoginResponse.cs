namespace ApiProject.Api.Models;

/// <summary>
/// Response model for login.
/// </summary>
public class LoginResponse
{
    /// <summary>
    /// The JWT token.
    /// </summary>
    public required string Token { get; set; }

    /// <summary>
    /// The user information.
    /// </summary>
    public required UserResponse User { get; set; }
}
