namespace ApiProject.BusinessLogic.Models;

public class ThesisOfferApplicationCreateRequestBusinessLogicModel
{
    public Guid ThesisOfferId { get; set; }
    public Guid StudentId { get; set; }
    public string? Message { get; set; }
}
