namespace ApiProject.ApiLogic.Models
{
    public class UpdateThesisRequest
    {
        public string? Title { get; set; }
        public string? SubjectArea { get; set; }
        public Guid? StatusId { get; set; }
        public Guid? BillingStatusId { get; set; }
        public Guid? TutorId { get; set; }
        public Guid? SecondSupervisorId { get; set; }
        public Guid? TopicId { get; set; }
    }
}
