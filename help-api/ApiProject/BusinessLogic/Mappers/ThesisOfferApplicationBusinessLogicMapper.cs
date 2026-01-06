using ApiProject.BusinessLogic.Models;
using ApiProject.DatabaseAccess.Entities;

namespace ApiProject.BusinessLogic.Mappers;

public static class ThesisOfferApplicationBusinessLogicMapper
{
    public static ThesisOfferApplicationBusinessLogicModel ToBusinessModel(ThesisOfferApplicationDataAccessModel dataAccessModel)
    {
        if (dataAccessModel == null)
        {
            return null;
        }

        return new ThesisOfferApplicationBusinessLogicModel
        {
            Id = dataAccessModel.Id,
            CreatedAt = dataAccessModel.CreatedAt,
            UpdatedAt = dataAccessModel.UpdatedAt,
            ThesisOfferId = dataAccessModel.ThesisOfferId,
            StudentId = dataAccessModel.StudentId,
            Status = dataAccessModel.RequestStatus?.Name,
            Message = dataAccessModel.Message
        };
    }
}
