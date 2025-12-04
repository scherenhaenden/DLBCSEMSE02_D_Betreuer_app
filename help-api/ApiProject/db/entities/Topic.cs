using System;

namespace ApiProject.Db.Entities;

public sealed class Topic : BaseEntity
{
    public required string Title        { get; set; }
    public required string Description  { get; set; }
    public required string SubjectArea  { get; set; }
    public bool IsActive                { get; set; } = true;

    /// <summary>
    /// FK -> Users.Id (must be a tutor)
    /// </summary>
    public Guid TutorId { get; set; }
}
