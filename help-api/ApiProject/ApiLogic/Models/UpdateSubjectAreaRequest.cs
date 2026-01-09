namespace ApiProject.ApiLogic.Models
{
    public class UpdateSubjectAreaRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? SubjectArea { get; set; }
        public bool? IsActive { get; set; }
        public List<Guid>? TutorIds { get; set; }
    }
}
