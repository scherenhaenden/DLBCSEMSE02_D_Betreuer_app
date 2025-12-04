using ApiProject.Db.Entities;
using ApiProject.Db.Context;
using Microsoft.EntityFrameworkCore;

namespace ApiProject.Logic.Services;

public sealed class UserService : IUserService
{
    private readonly ThesisDbContext _context;

    public UserService(ThesisDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.SingleOrDefaultAsync(u => u.Id == id);
    }

    public async Task<Role> EnsureRoleAsync(string roleName)
    {
        var normalized = roleName.Trim().ToUpperInvariant();

        var existing = await _context.Roles.SingleOrDefaultAsync(r => r.Name == normalized);
        if (existing is not null)
        {
            return existing;
        }

        var role = new Role
        {
            Name = normalized
        };

        _context.Roles.Add(role);
        await _context.SaveChangesAsync();
        return role;
    }

    public async Task<bool> UserHasRoleAsync(Guid userId, string roleName)
    {
        var user = await GetByIdAsync(userId);
        if (user is null)
        {
            return false;
        }

        var normalized = roleName.Trim().ToUpperInvariant();

        return user.UserRoles.Any(ur =>
            ur.Role is not null &&
            ur.Role.Name == normalized);
    }

    public async Task<User> CreateUserAsync(
        string firstName,
        string lastName,
        string email,
        string passwordHash,
        IEnumerable<string> roleNames)
    {
        if (await _context.Users.AnyAsync(u =>
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("A user with this e-mail already exists.");
        }

        var user = new User
        {
            FirstName    = firstName.Trim(),
            LastName     = lastName.Trim(),
            Email        = email.Trim(),
            PasswordHash = passwordHash // assume already hashed
        };

        foreach (var roleName in roleNames)
        {
            var role = await EnsureRoleAsync(roleName);

            var userRole = new UserRole
            {
                User   = user,
                UserId = user.Id,
                Role   = role,
                RoleId = role.Id
            };

            user.UserRoles.Add(userRole);
            role.UserRoles.Add(userRole);
        }

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }
}
