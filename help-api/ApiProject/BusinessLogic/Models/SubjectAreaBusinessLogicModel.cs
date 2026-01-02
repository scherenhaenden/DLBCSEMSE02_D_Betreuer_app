namespace ApiProject.BusinessLogic.Models
{
    public class SubjectAreaBusinessLogicModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public List<Guid> TutorIds { get; set; } = new List<Guid>();
    }
}
