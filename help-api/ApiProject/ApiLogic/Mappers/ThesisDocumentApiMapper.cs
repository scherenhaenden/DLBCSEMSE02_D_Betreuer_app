using ApiProject.ApiLogic.Models;
using ApiProject.BusinessLogic.Models;

namespace ApiProject.ApiLogic.Mappers
{
    public class ThesisDocumentApiMapper : IThesisDocumentApiMapper
    {
        public ThesisDocumentResponse MapToResponse(ThesisDocumentBusinessLogicModel model, Guid userId)
        {
            return new ThesisDocumentResponse
            {
                Id = model.Id,
                FileName = model.FileName,
                ContentType = model.ContentType,
                ThesisId = model.ThesisId,
                UserId = userId
            };
        }
    }
}
