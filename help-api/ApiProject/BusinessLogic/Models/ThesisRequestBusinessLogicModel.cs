namespace ApiProject.BusinessLogic.Models
{
    public class ThesisRequestBusinessLogicModel
    {
        public Guid Id { get; set; }
        public Guid ThesisId { get; set; }
        public string ThesisTitle { get; set; }
        public UserBusinessLogicModel Requester { get; set; }
        public UserBusinessLogicModel Receiver { get; set; }
        public string RequestType { get; set; }
        public string Status { get; set; }
        public string? Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime PlannedStartOfSupervision { get; set; }
        public DateTime PlannedEndOfSupervision { get; set; }
    }
}
