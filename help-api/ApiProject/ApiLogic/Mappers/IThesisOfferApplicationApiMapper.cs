using ApiProject.ApiLogic.Models;
using ApiProject.BusinessLogic.Models;

namespace ApiProject.ApiLogic.Mappers
{
    public interface IThesisOfferApplicationApiMapper
    {
        ThesisOfferApplicationResponse MapToResponse(ThesisOfferApplicationBusinessLogicModel thesisOfferApplication);
    }
}
