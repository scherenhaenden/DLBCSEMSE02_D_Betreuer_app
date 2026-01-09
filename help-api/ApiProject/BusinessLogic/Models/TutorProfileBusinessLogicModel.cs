namespace ApiProject.BusinessLogic.Models
{
    public class TutorProfileBusinessLogicModel
    {
        public Guid Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public List<SubjectAreaBusinessLogicModel> SubjectAreas { get; set; } = new();
    }
}

