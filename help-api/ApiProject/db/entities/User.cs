namespace ApiProject.Db.Entities;

public sealed class User : BaseEntity
{
    public required string FirstName { get; set; }
    public required string LastName  { get; set; }
    public required string Email     { get; set; }

    // For the exercise we store a "hash" as string.
    // In real life: never store plain passwords.
    public required string PasswordHash { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
