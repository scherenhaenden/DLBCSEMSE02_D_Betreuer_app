using System.ComponentModel.DataAnnotations.Schema;

namespace ApiProject.DatabaseAccess.Entities
{
    [Table("ThesisStatuses")]
    public class ThesisStatusDataAccessModel : BaseEntity
    {
        public required string Name { get; set; }
    }
}
