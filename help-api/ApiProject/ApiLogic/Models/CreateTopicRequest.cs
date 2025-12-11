using System.ComponentModel.DataAnnotations;

namespace ApiProject.ApiLogic.Models
{
    public class CreateTopicRequest
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string SubjectArea { get; set; }
        public List<Guid> TutorIds { get; set; } = new List<Guid>();
    }
}
