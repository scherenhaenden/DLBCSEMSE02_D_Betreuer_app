using System.ComponentModel.DataAnnotations.Schema;

namespace ApiProject.DatabaseAccess.Entities
{
    [Table("Roles")]
    public sealed class RoleDataAccessModel : BaseEntity
    {
        /// <summary>
        /// Expected values: "STUDENT", "TUTOR"
        /// </summary>
        public required string Name { get; set; }

        public ICollection<UserRoleDataAccessModel> UserRoles { get; set; } = new List<UserRoleDataAccessModel>();
    }
}
