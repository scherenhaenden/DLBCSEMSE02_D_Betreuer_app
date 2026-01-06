using ApiProject.ApiLogic.Models;
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

        /// <summary>
        /// Retrieves a paginated list of users from the database, with optional filtering by email, first name, last name, or role.
        /// This method supports pagination to handle large datasets efficiently and allows for flexible querying to find specific users.
        /// 
        /// What: Fetches users based on provided filters and returns them in a paginated format, including their associated roles.
        /// How: Builds a queryable object starting from the Users table, includes related UserRoles and Role entities for eager loading.
        /// Applies filters conditionally: email contains search, first name contains search, last name contains search, and role exact match (normalized to uppercase).
        /// Counts the total matching records for pagination metadata, then skips and takes the appropriate page of results.
        /// Maps the results to business models using the UserBusinessLogicMapper.
        /// Why: Essential for user management interfaces where administrators or systems need to list, search, and paginate through users.
        /// Supports role-based access control by filtering users by their roles, and improves performance with pagination to avoid loading all users at once.
        /// </summary>
        /// <param name="page">The page number to retrieve (1-based).</param>
        /// <param name="pageSize">The number of users per page.</param>
        /// <param name="email">Optional filter for users whose email contains this string.</param>
        /// <param name="firstName">Optional filter for users whose first name contains this string.</param>
        /// <param name="lastName">Optional filter for users whose last name contains this string.</param>
        /// <param name="role">Optional filter for users with this exact role (case-insensitive).</param>
        /// <returns>A paginated result containing the list of users and pagination metadata.</returns>
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

        /// <summary>
        /// Retrieves a single user by their unique identifier, including their associated roles.
        /// 
        /// What: Fetches a specific user from the database based on their GUID ID.
        /// How: Queries the Users table with eager loading of UserRoles and Role entities to include role information.
        /// Uses SingleOrDefaultAsync to find the user, returning null if not found.
        /// Maps the data access model to the business model using UserBusinessLogicMapper.
        /// Why: Necessary for operations that require detailed user information, such as profile views or user-specific actions.
        /// Ensures data integrity by including roles for authorization checks.
        /// </summary>
        /// <param name="id">The unique GUID identifier of the user.</param>
        /// <returns>The user business model if found, otherwise null.</returns>
        public async Task<UserBusinessLogicModel?> GetByIdAsync(Guid id)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .SingleOrDefaultAsync(u => u.Id == id);

            return UserBusinessLogicMapper.ToBusinessModel(user);
        }

        /// <summary>
        /// Retrieves a single user by their email address, including their associated roles.
        /// 
        /// What: Fetches a specific user from the database based on their email.
        /// How: Queries the Users table with eager loading of UserRoles and Role entities to include role information.
        /// Uses SingleOrDefaultAsync to find the user by exact email match, returning null if not found.
        /// Maps the data access model to the business model using UserBusinessLogicMapper.
        /// Why: Critical for authentication and login processes where users are identified by email.
        /// Ensures roles are loaded for subsequent authorization checks during login.
        /// </summary>
        /// <param name="email">The email address of the user to retrieve.</param>
        /// <returns>The user business model if found, otherwise null.</returns>
        public async Task<UserBusinessLogicModel?> GetByEmailAsync(string email)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .SingleOrDefaultAsync(u => u.Email == email);
            
            return UserBusinessLogicMapper.ToBusinessModel(user);
        }

        /// <summary>
        /// Creates a new user with the specified details and assigns roles.
        /// 
        /// What: Creates a new user account in the system with provided personal information, password, and roles.
        /// How: Validates input by trimming and normalizing role names, checks for at least one role, and ensures email uniqueness.
        /// Hashes the password using BCrypt for security. Retrieves existing roles from the database and validates all requested roles exist.
        /// Creates a new UserDataAccessModel, assigns roles via UserRoleDataAccessModel, saves to database, and fetches the created user with roles populated.
        /// Why: Essential for user registration and account creation processes.
        /// Ensures data integrity with email uniqueness, role validation, and secure password storage.
        /// Supports multi-role assignment for flexible user permissions.
        /// </summary>
        /// <param name="firstName">The first name of the user.</param>
        /// <param name="lastName">The last name of the user.</param>
        /// <param name="email">The email address of the user (must be unique).</param>
        /// <param name="password">The plain text password to be hashed.</param>
        /// <param name="roleNames">A collection of role names to assign to the user.</param>
        /// <returns>The created user business model with roles populated.</returns>
        /// <exception cref="ArgumentException">Thrown if no roles are provided.</exception>
        /// <exception cref="InvalidOperationException">Thrown if email already exists or if some roles do not exist.</exception>
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

        /// <summary>
        /// Verifies a user's password by comparing the provided password with the stored hash.
        /// 
        /// What: Checks if the given password matches the hashed password for the user identified by email.
        /// How: Retrieves the user by email from the database. If user exists, uses BCrypt to verify the plain text password against the stored hash.
        /// Returns true if verification succeeds, false otherwise (including if user not found).
        /// Why: Fundamental for authentication processes to validate user credentials securely.
        /// Uses BCrypt for secure password verification without exposing the hash.
        /// Protects against timing attacks by returning false for non-existent users.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <param name="password">The plain text password to verify.</param>
        /// <returns>True if the password is correct, false otherwise.</returns>
        public async Task<bool> VerifyPasswordAsync(string email, string password)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return false;
            }

            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }


        /// <summary>
        /// Checks if a user has a specific role assigned.
        /// 
        /// What: Determines whether a given user possesses a particular role.
        /// How: Retrieves the user by ID with eager loading of UserRoles and Role entities.
        /// If user exists, normalizes the role name and checks if any of the user's roles match the specified role.
        /// Returns false if user does not exist or does not have the role.
        /// Why: Crucial for authorization and access control throughout the application.
        /// Enables role-based permissions to restrict or allow certain operations.
        /// Supports case-insensitive role checking for robustness.
        /// </summary>
        /// <param name="userId">The unique GUID identifier of the user.</param>
        /// <param name="roleName">The name of the role to check for.</param>
        /// <returns>True if the user has the role, false otherwise.</returns>
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

        /// <summary>
        /// Retrieves a paginated list of tutors with optional filtering by subject area or name.
        /// 
        /// What: Fetches tutors (users with TUTOR role) along with their associated subject areas, with optional filters.
        /// How: Builds a query starting from Users table, includes UserRoles, Role, UserToSubjectAreas, and SubjectArea entities.
        /// Filters users to only those with TUTOR role. Applies optional filters: by subject area ID, subject area name (case-insensitive contains), or name (first/last name contains).
        /// Counts total matching tutors, skips and takes for pagination, then maps to TutorProfileResponse including subject areas.
        /// Why: Enables students and administrators to browse and search for tutors based on expertise areas or names.
        /// Supports discovery of suitable supervisors for theses.
        /// Improves user experience with pagination and flexible search options.
        /// </summary>
        /// <param name="subjectAreaId">Optional GUID to filter tutors by a specific subject area.</param>
        /// <param name="subjectAreaName">Optional string to filter tutors by subject area name (partial match).</param>
        /// <param name="name">Optional string to filter tutors by first or last name (partial match).</param>
        /// <param name="page">The page number to retrieve (1-based).</param>
        /// <param name="pageSize">The number of tutors per page.</param>
        /// <returns>A paginated response containing tutor profiles with their subject areas.</returns>
        public async Task<PaginatedResultBusinessLogicModel<TutorProfileBusinessLogicModel>> GetTutorsAsync(Guid? subjectAreaId, string? subjectAreaName, string? name, int page, int pageSize)
        {
            var query = _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Include(u => u.UserToSubjectAreas)
                .ThenInclude(ut => ut.SubjectArea)
                .Where(u => u.UserRoles.Any(ur => ur.Role.Name == "TUTOR"));

            if (subjectAreaId.HasValue)
            {
                query = query.Where(u => u.UserToSubjectAreas.Any(ut => ut.SubjectAreaId == subjectAreaId.Value));
            }

            if (!string.IsNullOrWhiteSpace(subjectAreaName))
            {
                query = query.Where(u => u.UserToSubjectAreas.Any(ut => ut.SubjectArea.Title.ToLower().Contains(subjectAreaName.ToLower())));
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(u => EF.Functions.Like(u.FirstName, $"%{name}%") || EF.Functions.Like(u.LastName, $"%{name}%"));
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var tutorProfiles = items.Select(u => new TutorProfileBusinessLogicModel
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                SubjectAreas = u.UserToSubjectAreas.Select(ut => new SubjectAreaBusinessLogicModel
                {
                    Id = ut.SubjectArea.Id,
                    Title = ut.SubjectArea.Title,
                    Description = ut.SubjectArea.Description,
                    IsActive = ut.SubjectArea.IsActive,
                    TutorIds = ut.SubjectArea.UserToSubjectAreas.Select(uts => uts.UserId).ToList()
                }).ToList()
            }).ToList();

            return new PaginatedResultBusinessLogicModel<TutorProfileBusinessLogicModel>
            {
                Items = tutorProfiles,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// Retrieves detailed information about a specific tutor by their ID.
        /// 
        /// What: Fetches a tutor's profile including personal details and associated subject areas.
        /// How: Queries the Users table with eager loading of UserRoles, Role, UserToSubjectAreas, and SubjectArea entities.
        /// Filters to ensure the user has the TUTOR role and matches the provided ID.
        /// If found, maps to TutorProfileResponse with subject areas; otherwise returns null.
        /// Why: Allows detailed viewing of tutor profiles for students selecting supervisors or for administrative purposes.
        /// Ensures only users with TUTOR role are accessible via this method.
        /// Provides comprehensive information for informed decision-making in thesis supervision.
        /// </summary>
        /// <param name="id">The unique GUID identifier of the tutor.</param>
        /// <returns>The tutor's profile response if found and is a tutor, otherwise null.</returns>
        public async Task<TutorProfileBusinessLogicModel?> GetTutorByIdAsync(Guid id)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Include(u => u.UserToSubjectAreas)
                .ThenInclude(ut => ut.SubjectArea)
                .Where(u => u.Id == id && u.UserRoles.Any(ur => ur.Role.Name == "TUTOR"))
                .SingleOrDefaultAsync();

            if (user == null)
            {
                return null;
            }

            return new TutorProfileBusinessLogicModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                SubjectAreas = user.UserToSubjectAreas.Select(ut => new SubjectAreaBusinessLogicModel
                {
                    Id = ut.SubjectArea.Id,
                    Title = ut.SubjectArea.Title,
                    Description = ut.SubjectArea.Description,
                    IsActive = ut.SubjectArea.IsActive,
                    TutorIds = ut.SubjectArea.UserToSubjectAreas.Select(uts => uts.UserId).ToList()
                }).ToList()
            };
        }
    }
}
