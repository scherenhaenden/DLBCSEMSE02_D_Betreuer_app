namespace ApiProject.BusinessLogic.Models
{
    public class ThesisDocumentBusinessLogicModel
    {
        public Guid Id { get; set; }
        public required string FileName { get; set; }
        public required string ContentType { get; set; }
        public required byte[] Content { get; set; }
        public Guid ThesisId { get; set; }
    }
}

