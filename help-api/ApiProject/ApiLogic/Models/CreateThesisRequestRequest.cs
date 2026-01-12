namespace ApiProject.ApiLogic.Models
{
    public class CreateThesisRequestRequest
    {
        public Guid ThesisId { get; set; }
        public Guid ReceiverId { get; set; }
        public string RequestType { get; set; } // "SUPERVISION" or "CO_SUPERVISION"
        public string? Message { get; set; }
        public DateTime PlannedStartOfSupervision { get; set; }
        public DateTime PlannedEndOfSupervision { get; set; }
    }
}
