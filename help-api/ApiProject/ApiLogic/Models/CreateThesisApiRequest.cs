namespace ApiProject.ApiLogic.models;

public sealed class CreateThesisApiRequest
{
    public required string Title { get; set; }

    public string? Description { get; set; }

    public Guid OwnerId { get; set; }
    public Guid TutorId { get; set; }
    public Guid? SecondSupervisorId { get; set; }
    public Guid? SubjectAreaId { get; set; }

    public int ProgressPercent { get; set; }
    public string? ExposePath { get; set; }
}