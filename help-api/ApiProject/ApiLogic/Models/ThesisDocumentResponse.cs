namespace ApiProject.ApiLogic.Models
{
    public class ThesisDocumentResponse
    {
        public Guid Id { get; set; }
        public required string FileName { get; set; }
        public required string ContentType { get; set; }
        public Guid ThesisId { get; set; }
        public Guid UserId { get; set; }
    }
}
