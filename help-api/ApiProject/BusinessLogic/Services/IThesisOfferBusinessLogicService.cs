using ApiProject.BusinessLogic.Models;

namespace ApiProject.BusinessLogic.Services;

public interface IThesisOfferBusinessLogicService
{
    Task<PaginatedResultBusinessLogicModel<ThesisOfferBusinessLogicModel>> GetAllAsync(int page, int pageSize, Guid userId, List<string> userRoles);
    Task<PaginatedResultBusinessLogicModel<ThesisOfferApplicationBusinessLogicModel>> GetApplicationsForUserAsync(Guid userId, int page, int pageSize);
    Task<ThesisOfferBusinessLogicModel> CreateAsync(ThesisOfferCreateRequestBusinessLogicModel request);
    Task<(ThesisOfferApplicationBusinessLogicModel?, string?)> CreateApplicationAsync(ThesisOfferApplicationCreateRequestBusinessLogicModel request);
}