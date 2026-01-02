namespace ApiProject.ApiLogic.Models
{
    public class ThesisOfferResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public Guid SubjectAreaId { get; set; }
        public Guid TutorId { get; set; }
        public string? Status { get; set; }
        public int? MaxStudents { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}

