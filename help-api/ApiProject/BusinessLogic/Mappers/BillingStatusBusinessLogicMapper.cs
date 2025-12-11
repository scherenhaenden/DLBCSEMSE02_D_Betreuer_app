using ApiProject.BusinessLogic.Models;
using ApiProject.DatabaseAccess.Entities;

namespace ApiProject.BusinessLogic.Mappers;

public static class BillingStatusBusinessLogicMapper
{
    public static BillingStatusBusinessLogicModel ToBusinessModel(BillingStatusDataAccessModel dataAccessModel)
    {
        if (dataAccessModel == null)
        {
            return null;
        }

        return new BillingStatusBusinessLogicModel
        {
            Id = dataAccessModel.Id,
            Name = dataAccessModel.Name
        };
    }
}
