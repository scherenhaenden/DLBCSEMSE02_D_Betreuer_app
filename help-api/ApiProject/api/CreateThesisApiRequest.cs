using ApiProject.Db;

namespace ApiProject.Api;

public sealed class CreateThesisApiRequest
{
    public required string Title { get; set; }

    public Guid OwnerId { get; set; }
    public Guid TutorId { get; set; }
    public Guid? SecondSupervisorId { get; set; }
    public Guid? TopicId { get; set; }

    public int ProgressPercent { get; set; }
    public string? ExposePath { get; set; }
    public BillingStatus BillingStatus { get; set; } = BillingStatus.None;
}