namespace ApiProject.BusinessLogic.Models
{
    public class ThesisUpdateRequestBusinessLogicModel
    {
        public string? Title { get; set; }
        public string? SubjectArea { get; set; }
        public string? StatusName { get; set; }
        public string? BillingStatusName { get; set; }
        public Guid? TutorId { get; set; }
        public Guid? SecondSupervisorId { get; set; }
        public Guid? TopicId { get; set; }
    }
}
