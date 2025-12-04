using ApiProject.Db.Entities;

namespace ApiProject.Logic.Services;

public interface IUserService
{
    Task<IReadOnlyCollection<User>> GetAllAsync();
    Task<User?> GetByIdAsync(Guid id);

    Task<User> CreateUserAsync(
        string firstName,
        string lastName,
        string email,
        string passwordHash,
        IEnumerable<string> roleNames);

    Task<bool> UserHasRoleAsync(Guid userId, string roleName);
    Task<Role> EnsureRoleAsync(string roleName);
}