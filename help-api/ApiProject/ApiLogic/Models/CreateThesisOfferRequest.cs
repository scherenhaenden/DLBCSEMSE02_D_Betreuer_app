using System.ComponentModel.DataAnnotations;

namespace ApiProject.ApiLogic.Models
{
    public class CreateThesisOfferRequest
    {
        [Required]
        public required string Title { get; set; }
        
        public string? Description { get; set; }
        
        [Required]
        public required Guid SubjectAreaId { get; set; }
        
        public int? MaxStudents { get; set; }
        
        public DateTime? ExpiresAt { get; set; }
        public Guid TutorId { get; set; }
    }
}
