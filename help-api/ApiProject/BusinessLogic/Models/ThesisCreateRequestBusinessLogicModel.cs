namespace ApiProject.BusinessLogic.Models
{
    public class ThesisCreateRequestBusinessLogicModel
    {
        public string Title { get; set; }
        public string SubjectArea { get; set; }
        public Guid OwnerId { get; set; }
        public Guid TutorId { get; set; }
        public Guid? SecondSupervisorId { get; set; }
        public Guid? TopicId { get; set; }
    }
}
