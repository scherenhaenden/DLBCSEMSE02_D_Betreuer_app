namespace ApiProject.ApiLogic.Models
{
    public class ThesisResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public string? BillingStatus { get; set; }
        public Guid OwnerId { get; set; }
        public Guid? TutorId { get; set; }
        public Guid? SecondSupervisorId { get; set; }
        public Guid? SubjectAreaId { get; set; }
        public string? DocumentFileName { get; set; }
    }
}
