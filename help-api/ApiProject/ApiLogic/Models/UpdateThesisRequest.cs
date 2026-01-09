namespace ApiProject.ApiLogic.Models
{
    public class UpdateThesisRequest
    {
        public string? Title { get; set; }

        public string? Description { get; set; }

        public Guid? SubjectAreaId { get; set; }
        public IFormFile? Document { get; set; }
        // Note: Status and Tutor assignments are handled by the request workflow, not direct updates.
    }
}
