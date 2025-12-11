using System.ComponentModel.DataAnnotations.Schema;

namespace ApiProject.DatabaseAccess.Entities
{
    [Table("UserTopics")]
    public class UserTopicDataAccessModel
    {
        public Guid UserId { get; set; }
        public UserDataAccessModel User { get; set; }

        public Guid TopicId { get; set; }
        public TopicDataAccessModel Topic { get; set; }
    }
}
