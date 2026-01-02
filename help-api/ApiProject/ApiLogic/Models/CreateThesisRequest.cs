using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ApiProject.ApiLogic.Models
{
    public class CreateThesisRequest
    {
        [Required]
        public required string Title { get; set; }
        
        // OwnerId will be taken from the authenticated user's claims

        public Guid? SubjectAreaId { get; set; }

        public IFormFile? Document { get; set; }
    }
}
