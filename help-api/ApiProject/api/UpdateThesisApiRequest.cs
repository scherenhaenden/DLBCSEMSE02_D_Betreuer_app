using ApiProject.Db;

namespace ApiProject.Api;

public sealed class UpdateThesisApiRequest
{
    public string? Title { get; set; }
    public ThesisStatus? Status { get; set; }
    public int? ProgressPercent { get; set; }
    public string? ExposePath { get; set; }
    public BillingStatus? BillingStatus { get; set; }
    public Guid? TutorId { get; set; }
    public Guid? SecondSupervisorId { get; set; }
    public Guid? TopicId { get; set; }
}