using System.ComponentModel.DataAnnotations.Schema;

namespace ApiProject.DatabaseAccess.Entities
{
    [Table("BillingStatuses")]
    public class BillingStatusDataAccessModel : BaseEntity
    {
        public required string Name { get; set; }
    }
}
