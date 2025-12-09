using System.ComponentModel.DataAnnotations.Schema;

namespace ApiProject.DatabaseAccess.Entities
{
    [Table("Theses")]
    public sealed class ThesisDataAccessModel : BaseEntity
    {
        public required string Title { get; set; }
        public required string SubjectArea { get; set; }

        public Guid StatusId { get; set; }
        public ThesisStatusDataAccessModel Status { get; set; }

        public Guid BillingStatusId { get; set; }
        public BillingStatusDataAccessModel BillingStatus { get; set; }

        public Guid OwnerId { get; set; }
        public UserDataAccessModel Owner { get; set; }

        public Guid TutorId { get; set; }
        public UserDataAccessModel Tutor { get; set; }

        public Guid? SecondSupervisorId { get; set; }
        public UserDataAccessModel? SecondSupervisor { get; set; }

        public Guid? TopicId { get; set; }
        public TopicDataAccessModel? Topic { get; set; }
        
        public ThesisDocumentDataAccessModel? Document { get; set; }
    }
}
