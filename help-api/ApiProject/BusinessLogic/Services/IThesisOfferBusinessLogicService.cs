using ApiProject.BusinessLogic.Models;

namespace ApiProject.BusinessLogic.Services;

public interface IThesisOfferBusinessLogicService
{
    Task<PaginatedResultBusinessLogicModel<ThesisOfferBusinessLogicModel>> GetAllAsync(int page, int pageSize);
}