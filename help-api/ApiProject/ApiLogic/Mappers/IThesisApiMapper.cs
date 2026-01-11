using ApiProject.ApiLogic.Models;
using ApiProject.BusinessLogic.Models;

namespace ApiProject.ApiLogic.Mappers
{
    public interface IThesisApiMapper
    {
        ThesisResponse MapToResponse(ThesisBusinessLogicModel thesis);
        Task<ThesisCreateRequestBusinessLogicModel> MapToCreateBusinessModel(CreateThesisApiRequest request, Guid ownerId);
        Task<ThesisUpdateRequestBusinessLogicModel> MapToUpdateBusinessModel(UpdateThesisRequest request);
    }
}
