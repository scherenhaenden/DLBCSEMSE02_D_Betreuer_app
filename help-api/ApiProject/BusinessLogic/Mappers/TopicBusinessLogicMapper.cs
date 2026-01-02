using ApiProject.BusinessLogic.Models;
using ApiProject.DatabaseAccess.Entities;

namespace ApiProject.BusinessLogic.Mappers;

public static class TopicBusinessLogicMapper
{
    public static TopicBusinessLogicModel ToBusinessModel(SubjectAreaDataAccessModel dataAccessModel)
    {
        if (dataAccessModel == null)
        {
            return null;
        }

        return new TopicBusinessLogicModel
        {
            Id = dataAccessModel.Id,
            Title = dataAccessModel.Title,
            Description = dataAccessModel.Description,
            IsActive = dataAccessModel.IsActive,
            TutorIds = dataAccessModel.UserToSubjectAreas.Select(ut => ut.UserId).ToList()
        };
    }
}