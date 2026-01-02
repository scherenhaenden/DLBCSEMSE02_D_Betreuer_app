using ApiProject.BusinessLogic.Models;
using ApiProject.DatabaseAccess.Entities;

namespace ApiProject.BusinessLogic.Mappers;

public static class ThesisBusinessLogicMapper
{
    public static ThesisBusinessLogicModel ToBusinessModel(ThesisDataAccessModel dataAccessModel)
    {
        if (dataAccessModel == null)
        {
            return null;
        }

        return new ThesisBusinessLogicModel
        {
            Id = dataAccessModel.Id,
            Title = dataAccessModel.Title,
            Status = dataAccessModel.Status?.Name,
            BillingStatus = dataAccessModel.BillingStatus != null ? BillingStatusBusinessLogicMapper.ToBusinessModel(dataAccessModel.BillingStatus) : null,
            OwnerId = dataAccessModel.OwnerId,
            TutorId = dataAccessModel.TutorId,
            SecondSupervisorId = dataAccessModel.SecondSupervisorId,
            TopicId = dataAccessModel.SubjectAreaId,
            DocumentFileName = dataAccessModel.Document?.FileName
        };
    }
}
