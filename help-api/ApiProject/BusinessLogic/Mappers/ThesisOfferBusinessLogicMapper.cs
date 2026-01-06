using ApiProject.BusinessLogic.Models;
using ApiProject.DatabaseAccess.Entities;

namespace ApiProject.BusinessLogic.Mappers;

public static class ThesisOfferBusinessLogicMapper
{
    public static ThesisOfferBusinessLogicModel ToBusinessModel(ThesisOfferDataAccessModel dataAccessModel)
    {
        if (dataAccessModel == null)
        {
            return null;
        }

        return new ThesisOfferBusinessLogicModel
        {
            Id = dataAccessModel.Id,
            CreatedAt = dataAccessModel.CreatedAt,
            UpdatedAt = dataAccessModel.UpdatedAt,
            Title = dataAccessModel.Title,
            Description = dataAccessModel.Description,
            SubjectAreaId = dataAccessModel.SubjectAreaId,
            TutorId = dataAccessModel.TutorId,
            Status = dataAccessModel.ThesisOfferStatus?.Name,
            MaxStudents = dataAccessModel.MaxStudents,
            ExpiresAt = dataAccessModel.ExpiresAt
        };
    }
}
