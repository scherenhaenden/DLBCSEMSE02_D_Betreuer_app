namespace ApiProject.ApiLogic.Models
{
    public class UpdateThesisOfferRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public Guid? SubjectAreaId { get; set; }
        public int? MaxStudents { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public Guid? ThesisOfferStatusId { get; set; }
    }
}
