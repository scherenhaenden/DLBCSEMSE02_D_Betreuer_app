namespace ApiProject.BusinessLogic.Models
{
    public class UserBusinessLogicModel : BaseEntityBusinessLogicModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
}
