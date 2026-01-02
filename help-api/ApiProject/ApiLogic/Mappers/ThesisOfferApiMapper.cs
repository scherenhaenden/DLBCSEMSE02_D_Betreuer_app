using ApiProject.ApiLogic.Models;
using ApiProject.BusinessLogic.Models;

namespace ApiProject.ApiLogic.Mappers
{
    public class ThesisOfferApiMapper : IThesisOfferApiMapper
    {
        public ThesisOfferResponse MapToResponse(ThesisOfferBusinessLogicModel thesisOffer)
        {
            if (thesisOffer == null) return null;

            return new ThesisOfferResponse
            {
                Id = thesisOffer.Id,
                Title = thesisOffer.Title,
                Description = thesisOffer.Description,
                SubjectAreaId = thesisOffer.SubjectAreaId,
                TutorId = thesisOffer.TutorId,
                Status = thesisOffer.Status,
                MaxStudents = thesisOffer.MaxStudents,
                ExpiresAt = thesisOffer.ExpiresAt
            };
        }
    }
}
