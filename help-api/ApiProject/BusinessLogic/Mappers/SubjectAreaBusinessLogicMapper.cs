using ApiProject.BusinessLogic.Models;
using ApiProject.DatabaseAccess.Entities;

namespace ApiProject.BusinessLogic.Mappers;

public static class SubjectAreaBusinessLogicMapper
{
    public static SubjectAreaBusinessLogicModel ToBusinessModel(SubjectAreaDataAccessModel dataAccessModel)
    {
        if (dataAccessModel == null)
        {
            return null;
        }

        return new SubjectAreaBusinessLogicModel
        {
            Id = dataAccessModel.Id,
            Title = dataAccessModel.Title,
            Description = dataAccessModel.Description,
            IsActive = dataAccessModel.IsActive,
            TutorIds = dataAccessModel.UserToSubjectAreas.Select(ut => ut.UserId).ToList()
        };
    }
}