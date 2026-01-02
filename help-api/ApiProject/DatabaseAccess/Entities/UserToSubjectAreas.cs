using System.ComponentModel.DataAnnotations.Schema;

namespace ApiProject.DatabaseAccess.Entities
{
    [Table("UserToSubjectArea")]
    public class UserToSubjectAreas
    {
        public Guid UserId { get; set; }
        public UserDataAccessModel User { get; set; }

        public Guid UserToSubjectAreaId { get; set; }
        public SubjectAreaDataAccessModel SubjectArea { get; set; }
    }
}
