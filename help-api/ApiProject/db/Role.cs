namespace ApiProject.Db;

public sealed class Role : BaseEntity
{
    /// <summary>
    /// Expected values: "STUDENT", "TUTOR", "SECOND_SUPERVISOR"
    /// </summary>
    public required string Name { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}

