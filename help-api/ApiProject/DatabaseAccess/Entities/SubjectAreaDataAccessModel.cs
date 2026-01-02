using System.ComponentModel.DataAnnotations.Schema;

namespace ApiProject.DatabaseAccess.Entities
{
    [Table("SubjectAreas")]
    public sealed class SubjectAreaDataAccessModel : BaseEntity
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<UserToSubjectAreas> UserToSubjectAreas { get; set; } = new List<UserToSubjectAreas>();
    }
}
