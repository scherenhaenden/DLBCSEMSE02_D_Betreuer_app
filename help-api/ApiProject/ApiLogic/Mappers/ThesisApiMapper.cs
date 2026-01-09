using ApiProject.ApiLogic.Models;
using ApiProject.BusinessLogic.Models;

namespace ApiProject.ApiLogic.Mappers
{
    public class ThesisApiMapper : IThesisApiMapper
    {
        public ThesisResponse MapToResponse(ThesisBusinessLogicModel thesis)
        {
            if (thesis == null) return null;

            return new ThesisResponse
            {
                Id = thesis.Id,
                Title = thesis.Title,

                Description = thesis.Description,

                Status = thesis.Status,
                BillingStatus = thesis.BillingStatus?.Name,
                OwnerId = thesis.OwnerId,
                TutorId = thesis.TutorId,
                SecondSupervisorId = thesis.SecondSupervisorId,
                SubjectAreaId = thesis.SubjectAreaId,
                DocumentFileName = thesis.DocumentFileName,
                DocumentId = thesis.DocumentId
            };
        }
    }
}
