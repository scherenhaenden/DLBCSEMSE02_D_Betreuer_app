using ApiProject.ApiLogic.Models;
using ApiProject.BusinessLogic.Models;

namespace ApiProject.ApiLogic.Mappers
{
    public interface IThesisDocumentApiMapper
    {
        ThesisDocumentResponse MapToResponse(ThesisDocumentBusinessLogicModel model, Guid userId);
    }
}
