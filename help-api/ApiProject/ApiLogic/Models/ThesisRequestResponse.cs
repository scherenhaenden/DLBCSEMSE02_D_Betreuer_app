namespace ApiProject.ApiLogic.Models
{
    public class ThesisRequestResponse
    {
        public Guid Id { get; set; }
        public Guid ThesisId { get; set; }
        public string ThesisTitle { get; set; }
        public UserResponse Requester { get; set; }
        public UserResponse Receiver { get; set; }
        public string RequestType { get; set; }
        public string Status { get; set; }
        public string? Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime PlannedStartOfSupervision { get; set; }
        public DateTime PlannedEndOfSupervision { get; set; }
        public string? DocumentFileName { get; set; }
        public Guid? DocumentId { get; set; }
    }
}
