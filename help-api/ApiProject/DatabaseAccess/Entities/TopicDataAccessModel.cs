using System.ComponentModel.DataAnnotations.Schema;

namespace ApiProject.DatabaseAccess.Entities
{
    [Table("Topics")]
    public sealed class TopicDataAccessModel : BaseEntity
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string SubjectArea { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<UserTopicDataAccessModel> UserTopics { get; set; } = new List<UserTopicDataAccessModel>();
    }
}
