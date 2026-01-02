namespace ApiProject.ApiLogic.Models
{
    public class UpdateThesisRequest
    {
        public string? Title { get; set; }
        public Guid? SubjectAreaId { get; set; }
        // Note: Status and Tutor assignments are handled by the request workflow, not direct updates.
    }
}
