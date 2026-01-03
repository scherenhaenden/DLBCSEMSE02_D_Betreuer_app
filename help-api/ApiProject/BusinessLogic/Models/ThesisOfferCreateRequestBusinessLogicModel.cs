namespace ApiProject.BusinessLogic.Models;

public class ThesisOfferCreateRequestBusinessLogicModel
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public Guid SubjectAreaId { get; set; }
    public Guid TutorId { get; set; }
    public int? MaxStudents { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
