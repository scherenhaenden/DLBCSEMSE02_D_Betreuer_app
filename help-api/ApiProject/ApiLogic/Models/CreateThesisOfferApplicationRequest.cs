using System.ComponentModel.DataAnnotations;

namespace ApiProject.ApiLogic.Models
{
    public class CreateThesisOfferApplicationRequest
    {
        [Required]
        public required Guid ThesisOfferId { get; set; }
        
        public string? Message { get; set; }
    }
}
