using ApiProject.BusinessLogic.Mappers;
using ApiProject.BusinessLogic.Models;
using ApiProject.DatabaseAccess.Context;
using ApiProject.DatabaseAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiProject.BusinessLogic.Services
{
    public sealed class UserBusinessLogicService : IUserBusinessLogicService
    {
        private readonly ThesisDbContext _context;

        public UserBusinessLogicService(ThesisDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResultBusinessLogicModel<UserBusinessLogicModel>> GetAllAsync(int page, int pageSize, string? email = null, string? firstName = null, string? lastName = null, string? role = null)
        {
            var query = _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .AsQueryable();

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
                query = query.Where(u => u.UserRoles.Any(ur => ur.Role.Name == normalizedRole));
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResultBusinessLogicModel<UserBusinessLogicModel>
            {
                Items = items.Select(UserBusinessLogicMapper.ToBusinessModel).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<UserBusinessLogicModel?> GetByIdAsync(Guid id)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .SingleOrDefaultAsync(u => u.Id == id);

            return UserBusinessLogicMapper.ToBusinessModel(user);
        }

        public async Task<UserBusinessLogicModel?> GetByEmailAsync(string email)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .SingleOrDefaultAsync(u => u.Email == email);
            
            return UserBusinessLogicMapper.ToBusinessModel(user);
        }

        public async Task<UserBusinessLogicModel> CreateUserAsync(string firstName, string lastName, string email, string password, IEnumerable<string> roleNames)
        {
            var roleNamesList = roleNames?.Select(r => r.Trim().ToUpperInvariant()).Distinct().ToList() 
                                ?? new List<string>();
            if (!roleNamesList.Any())
            {
                throw new ArgumentException("User must be assigned at least one role.", nameof(roleNames));
            }
            
            if (await _context.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower()))
            {
                throw new InvalidOperationException("A user with this e-mail already exists.");
            }

            var user = new UserDataAccessModel
            {
                FirstName = firstName.Trim(),
                LastName = lastName.Trim(),
                Email = email.Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
            };

            var existingRoles = await _context.Roles
                .Where(r => roleNamesList.Contains(r.Name))
                .ToListAsync();

            if (existingRoles.Count != roleNamesList.Count)
            {
                var missingRoles = roleNamesList.Except(existingRoles.Select(r => r.Name));
                throw new InvalidOperationException($"The following roles do not exist: {string.Join(", ", missingRoles)}");
            }

            foreach (var role in existingRoles)
                user.UserRoles.Add(new UserRoleDataAccessModel { User = user, Role = role });

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            // We need to fetch the user again to get the roles populated for the business model
            var createdUser = await GetByIdAsync(user.Id);
            return createdUser!;
        }

        public async Task<bool> VerifyPasswordAsync(string email, string password)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return false;
            }

            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }


        public async Task<bool> UserHasRoleAsync(Guid userId, string roleName)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .SingleOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return false;
            }

            var normalizedRole = roleName.Trim().ToUpperInvariant();
            return user.UserRoles.Any(ur => ur.Role.Name == normalizedRole);
        }
    }
}
