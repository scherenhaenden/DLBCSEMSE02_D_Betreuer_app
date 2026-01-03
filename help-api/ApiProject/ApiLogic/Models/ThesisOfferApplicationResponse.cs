namespace ApiProject.ApiLogic.Models
{
    public class ThesisOfferApplicationResponse
    {
        public Guid Id { get; set; }
        public Guid ThesisOfferId { get; set; }
        public Guid StudentId { get; set; }
        public string? Status { get; set; }
        public string? Message { get; set; }
    }
}

