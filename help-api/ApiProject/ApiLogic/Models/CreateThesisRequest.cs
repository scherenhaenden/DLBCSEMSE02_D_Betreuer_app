using System.ComponentModel.DataAnnotations;

namespace ApiProject.ApiLogic.Models
{
    public class CreateThesisRequest
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string SubjectArea { get; set; }
        [Required]
        public Guid OwnerId { get; set; }
        [Required]
        public Guid TutorId { get; set; }
        public Guid? SecondSupervisorId { get; set; }
        public Guid? TopicId { get; set; }
    }
}
