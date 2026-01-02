using System.ComponentModel.DataAnnotations;

namespace ApiProject.ApiLogic.Models
{
    public class CreateThesisRequest
    {
        [Required]
        public string Title { get; set; }
        
        // OwnerId will be taken from the authenticated user's claims

        public Guid? SubjectAreaId { get; set; }
    }
}
