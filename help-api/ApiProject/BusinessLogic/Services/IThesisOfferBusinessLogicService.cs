using ApiProject.BusinessLogic.Models;

namespace ApiProject.BusinessLogic.Services;

public interface IThesisOfferBusinessLogicService
{
    Task<PaginatedResultBusinessLogicModel<SubjectAreaBusinessLogicModel>> GetAllAsync(int page, int pageSize);
}