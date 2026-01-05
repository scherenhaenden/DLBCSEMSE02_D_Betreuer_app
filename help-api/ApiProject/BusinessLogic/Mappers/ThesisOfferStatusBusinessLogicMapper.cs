using ApiProject.BusinessLogic.Models;
using ApiProject.DatabaseAccess.Entities;

namespace ApiProject.BusinessLogic.Mappers;

public static class ThesisOfferStatusBusinessLogicMapper
{
    public static ThesisOfferStatusBusinessLogicModel ToBusinessModel(ThesisOfferStatusDataAccessModel dataAccessModel)
    {
        if (dataAccessModel == null)
        {
            return null;
        }

        return new ThesisOfferStatusBusinessLogicModel
        {
            Id = dataAccessModel.Id,
            CreatedAt = dataAccessModel.CreatedAt,
            UpdatedAt = dataAccessModel.UpdatedAt,
            Name = dataAccessModel.Name
        };
    }
}
