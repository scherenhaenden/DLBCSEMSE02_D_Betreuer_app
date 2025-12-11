using System.ComponentModel.DataAnnotations.Schema;

namespace ApiProject.DatabaseAccess.Entities
{
    [Table("Users")]
    public sealed class UserDataAccessModel : BaseEntity
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }

        public ICollection<UserRoleDataAccessModel> UserRoles { get; set; } = new List<UserRoleDataAccessModel>();

        public ICollection<UserTopicDataAccessModel> UserTopics { get; set; } = new List<UserTopicDataAccessModel>();
    }
}
