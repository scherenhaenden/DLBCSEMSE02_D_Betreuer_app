using ApiProject.ApiLogic.Models;
using ApiProject.BusinessLogic.Models;

namespace ApiProject.ApiLogic.Mappers
{
    public interface IThesisOfferApiMapper
    {
        ThesisOfferResponse MapToResponse(ThesisOfferBusinessLogicModel thesisOffer);
        ThesisOfferStatusResponse MapToStatusResponse(ThesisOfferStatusBusinessLogicModel status);
    }
}
