namespace ApiProject.BusinessLogic.Models;

public class ThesisOfferApplicationBusinessLogicModel : BaseEntityBusinessLogicModel
{
    public Guid ThesisOfferId { get; set; }
    public Guid StudentId { get; set; }
    public string? Status { get; set; }
    public string? Message { get; set; }
}
