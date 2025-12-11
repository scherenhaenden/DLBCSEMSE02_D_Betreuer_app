using System.ComponentModel.DataAnnotations;

namespace ApiProject.ApiLogic.Models
{
    public class CreateUserRequest
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
}
