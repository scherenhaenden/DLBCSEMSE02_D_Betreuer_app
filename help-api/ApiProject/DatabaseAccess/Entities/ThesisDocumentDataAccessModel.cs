using System.ComponentModel.DataAnnotations.Schema;

namespace ApiProject.DatabaseAccess.Entities
{
    [Table("ThesisDocuments")]
    public class ThesisDocumentDataAccessModel : BaseEntity
    {
        public required string FileName { get; set; }
        public required string ContentType { get; set; }
        public required byte[] Content { get; set; }

        public Guid ThesisId { get; set; }
        public ThesisDataAccessModel Thesis { get; set; }
    }
}
