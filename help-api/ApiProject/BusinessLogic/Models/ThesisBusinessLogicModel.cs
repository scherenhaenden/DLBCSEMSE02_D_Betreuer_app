namespace ApiProject.BusinessLogic.Models
{
    public class ThesisBusinessLogicModel : BaseEntityBusinessLogicModel
    {
        public string Title { get; set; }

        public string? Description { get; set; }

        public string Status { get; set; }
        public BillingStatusBusinessLogicModel? BillingStatus { get; set; }
        public Guid OwnerId { get; set; }
        public Guid? TutorId { get; set; }
        public Guid? SecondSupervisorId { get; set; }
        public Guid? SubjectAreaId { get; set; }
        public string? DocumentFileName { get; set; }
        public Guid? DocumentId { get; set; }
    }
}
