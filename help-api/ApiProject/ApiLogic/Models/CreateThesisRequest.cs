using System.ComponentModel.DataAnnotations;

namespace ApiProject.ApiLogic.Models
{
    public class CreateThesisApiRequest
    {
        [Required]
        public required string Title { get; set; }
        
        // OwnerId will be taken from the authenticated user's claims

        public Guid? SubjectAreaId { get; set; }

        public IFormFile? Document { get; set; }
        public Guid OwnerId { get; set; }
        public string? Description { get; set; }
    }
}
