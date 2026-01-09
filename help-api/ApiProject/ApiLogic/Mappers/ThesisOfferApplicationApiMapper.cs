using ApiProject.ApiLogic.Models;
using ApiProject.BusinessLogic.Models;

namespace ApiProject.ApiLogic.Mappers
{
    public class ThesisOfferApplicationApiMapper : IThesisOfferApplicationApiMapper
    {
        public ThesisOfferApplicationResponse MapToResponse(ThesisOfferApplicationBusinessLogicModel thesisOfferApplication)
        {
            if (thesisOfferApplication == null) return null;

            return new ThesisOfferApplicationResponse
            {
                Id = thesisOfferApplication.Id,
                ThesisOfferId = thesisOfferApplication.ThesisOfferId,
                StudentId = thesisOfferApplication.StudentId,
                Status = thesisOfferApplication.Status,
                Message = thesisOfferApplication.Message
            };
        }
    }
}
