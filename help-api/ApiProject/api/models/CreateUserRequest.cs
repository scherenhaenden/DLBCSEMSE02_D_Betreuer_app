namespace ApiProject.Api.Models;

public sealed class CreateUserRequest
{
    public required string FirstName { get; set; }
    public required string LastName  { get; set; }
    public required string Email     { get; set; }
    public required string Password  { get; set; }

    /// <summary>
    /// Example: ["STUDENT"] or ["TUTOR"]
    /// </summary>
    public List<string> Roles { get; set; } = new();
}