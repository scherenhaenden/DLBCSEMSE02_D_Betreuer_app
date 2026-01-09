namespace ApiProject.BusinessLogic.Models
{
    public class ThesisCreateRequestBusinessLogicModel: BaseEntityBusinessLogicModel
    {
        public string Title { get; set; }

        public string? Description { get; set; }

        public string SubjectArea { get; set; }
        public Guid OwnerId { get; set; }
        public Guid? SubjectAreaId { get; set; }
        public string? DocumentFileName { get; set; }
        public string? DocumentContentType { get; set; }
        public byte[]? DocumentContent { get; set; }
    }
}
