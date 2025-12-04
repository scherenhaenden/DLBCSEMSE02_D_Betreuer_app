using System;

namespace ApiProject.Db;

public sealed class Thesis : BaseEntity
{
    public required string Title { get; set; }

    public ThesisStatus Status { get; set; } = ThesisStatus.Draft;

    /// <summary>
    /// 0–100
    /// </summary>
    public int ProgressPercent { get; set; }

    /// <summary>
    /// Optional path to Exposé.
    /// </summary>
    public string? ExposePath { get; set; }

    public BillingStatus BillingStatus { get; set; } = BillingStatus.None;

    /// <summary>
    /// FK -> Users.Id (must have role STUDENT)
    /// </summary>
    public Guid OwnerId { get; set; }

    /// <summary>
    /// FK -> Users.Id (must have role TUTOR)
    /// </summary>
    public Guid TutorId { get; set; }

    /// <summary>
    /// FK -> Users.Id (must have role TUTOR), optional
    /// </summary>
    public Guid? SecondSupervisorId { get; set; }

    /// <summary>
    /// FK -> Topics.Id, optional
    /// </summary>
    public Guid? TopicId { get; set; }
}

