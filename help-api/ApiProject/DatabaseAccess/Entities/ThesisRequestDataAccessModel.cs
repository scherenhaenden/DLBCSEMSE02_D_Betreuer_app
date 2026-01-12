using System.ComponentModel.DataAnnotations.Schema;

namespace ApiProject.DatabaseAccess.Entities
{
    [Table("ThesisRequests")]
    public class ThesisRequestDataAccessModel : BaseEntity
    {
        public Guid RequesterId { get; set; }
        public UserDataAccessModel Requester { get; set; }

        public Guid ReceiverId { get; set; }
        public UserDataAccessModel Receiver { get; set; }

        public Guid ThesisId { get; set; }
        public ThesisDataAccessModel Thesis { get; set; }

        public Guid RequestTypeId { get; set; }
        public RequestTypeDataAccessModel RequestType { get; set; }

        public Guid StatusId { get; set; }
        public RequestStatusDataAccessModel Status { get; set; }

        public string? Message { get; set; }

        public DateTime PlannedStartOfSupervision { get; set; }
        public DateTime PlannedEndOfSupervision { get; set; }
    }
}
