using System.ComponentModel.DataAnnotations.Schema;

namespace ApiProject.DatabaseAccess.Entities
{
    [Table("UserRoles")]
    public class UserRoleDataAccessModel
    {
        public Guid UserId { get; set; }
        public virtual UserDataAccessModel User { get; set; }

        public Guid RoleId { get; set; }
        public virtual RoleDataAccessModel Role { get; set; }
    }
}
