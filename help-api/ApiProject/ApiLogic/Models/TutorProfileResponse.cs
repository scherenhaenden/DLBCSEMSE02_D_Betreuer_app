namespace ApiProject.ApiLogic.Models
{
    public class TutorProfileResponse
    {
        public Guid Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public List<TopicResponse> Topics { get; set; } = new();
    }
}
