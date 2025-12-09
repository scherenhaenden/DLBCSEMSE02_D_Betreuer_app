using ApiProject.BusinessLogic.Models;

namespace ApiProject.BusinessLogic.Services
{
    public interface IUserBusinessLogicService
    {
        Task<PaginatedResultBusinessLogicModel<UserBusinessLogicModel>> GetAllAsync(int page, int pageSize, string? email = null, string? firstName = null, string? lastName = null, string? role = null);
        Task<UserBusinessLogicModel?> GetByIdAsync(Guid id);
        Task<UserBusinessLogicModel?> GetByEmailAsync(string email);
        Task<UserBusinessLogicModel> CreateUserAsync(string firstName, string lastName, string email, string password, IEnumerable<string> roleNames);
        Task<bool> VerifyPasswordAsync(string email, string password);
        Task<bool> UserHasRoleAsync(Guid userId, string roleName);
    }
}
