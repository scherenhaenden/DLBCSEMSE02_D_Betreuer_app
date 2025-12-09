namespace ApiProject.ApiLogic.models;

public sealed class UpdateThesisApiRequest
{
    public string? Title { get; set; }
    public string? Status { get; set; }
    public int? ProgressPercent { get; set; }
    public string? ExposePath { get; set; }
    public string? BillingStatus { get; set; }
    public Guid? TutorId { get; set; }
    public Guid? SecondSupervisorId { get; set; }
    public Guid? TopicId { get; set; }
}