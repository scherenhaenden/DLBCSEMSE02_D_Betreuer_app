namespace ApiProject.BusinessLogic.Models
{
    public class ThesisBusinessLogicModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string SubjectArea { get; set; }
        public string Status { get; set; }
        public BillingStatusBusinessLogicModel? BillingStatus { get; set; }
        public Guid OwnerId { get; set; }
        public Guid TutorId { get; set; }
        public Guid? SecondSupervisorId { get; set; }
        public Guid? TopicId { get; set; }
        public string? DocumentFileName { get; set; }
    }
}
