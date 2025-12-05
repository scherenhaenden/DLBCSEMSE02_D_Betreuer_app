using ApiProject.Db.Entities;
using ApiProject.Db.Context;
using ApiProject.Logic.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace ApiProject.Logic.Services;

public sealed class UserService : IUserService
{
    private readonly ThesisDbContext _context;

    public UserService(ThesisDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResult<User>> GetAllAsync(int page, int pageSize, string? email = null, string? firstName = null, string? lastName = null, string? role = null)
    {
        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(email))
        {
            query = query.Where(u => u.Email.Contains(email));
        }

        if (!string.IsNullOrWhiteSpace(firstName))
        {
            query = query.Where(u => u.FirstName.Contains(firstName));
        }

        if (!string.IsNullOrWhiteSpace(lastName))
        {
            query = query.Where(u => u.LastName.Contains(lastName));
        }

        if (!string.IsNullOrWhiteSpace(role))
        {
            var normalizedRole = role.Trim().ToUpperInvariant();
            query = query.Where(u => u.UserRoles.Any(ur => ur.Role != null && ur.Role.Name == normalizedRole));
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<User>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.SingleOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
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
        string password,
        IEnumerable<string> roleNames)
    {
        if (await _context.Users.AnyAsync(u =>
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("A user with this e-mail already exists.");
        }

        var salt = GenerateSalt();
        var passwordHash = HashPassword(password, salt);

        var user = new User
        {
            FirstName    = firstName.Trim(),
            LastName     = lastName.Trim(),
            Email        = email.Trim(),
            PasswordHash = passwordHash,
            Salt         = salt
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

            _context.UserRoles.Add(userRole);
        }

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> VerifyPasswordAsync(string email, string password)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        if (user is null)
        {
            return false;
        }

        var hash = HashPassword(password, user.Salt);
        return hash == user.PasswordHash;
    }

    private static string GenerateSalt()
    {
        var saltBytes = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }
        return Convert.ToBase64String(saltBytes);
    }

    private static string HashPassword(string password, string salt)
    {
        var saltBytes = Convert.FromBase64String(salt);
        using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 10000, HashAlgorithmName.SHA256))
        {
            var hashBytes = pbkdf2.GetBytes(32);
            return Convert.ToBase64String(hashBytes);
        }
    }
}
