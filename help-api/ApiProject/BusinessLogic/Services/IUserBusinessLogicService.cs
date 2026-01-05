using ApiProject.ApiLogic.Models;
using ApiProject.BusinessLogic.Models;

namespace ApiProject.BusinessLogic.Services
{
    /// <summary>
    /// Provides business logic operations for managing users in the thesis management system.
    /// This service handles user retrieval, creation, authentication, role checking, and tutor-specific queries.
    /// </summary>
    public interface IUserBusinessLogicService
    {
        /// <summary>
        /// Retrieves a paginated list of users asynchronously, with optional filtering by email, first name, last name, or role.
        /// Includes associated roles for each user.
        /// </summary>
        /// <param name="page">The page number to retrieve (1-based).</param>
        /// <param name="pageSize">The number of users per page.</param>
        /// <param name="email">Optional filter for users whose email contains this string.</param>
        /// <param name="firstName">Optional filter for users whose first name contains this string.</param>
        /// <param name="lastName">Optional filter for users whose last name contains this string.</param>
        /// <param name="role">Optional filter for users with this exact role (case-insensitive).</param>
        /// <returns>A task representing the asynchronous operation, containing a <see cref="PaginatedResultBusinessLogicModel{T}"/> with a list of <see cref="UserBusinessLogicModel"/> and pagination metadata.</returns>
        Task<PaginatedResultBusinessLogicModel<UserBusinessLogicModel>> GetAllAsync(int page, int pageSize, string? email = null, string? firstName = null, string? lastName = null, string? role = null);

        /// <summary>
        /// Retrieves a specific user by their unique identifier asynchronously.
        /// Includes associated roles.
        /// </summary>
        /// <param name="id">The unique identifier of the user to retrieve.</param>
        /// <returns>A task representing the asynchronous operation, containing the <see cref="UserBusinessLogicModel"/> if found, or null if not found.</returns>
        Task<UserBusinessLogicModel?> GetByIdAsync(Guid id);

        /// <summary>
        /// Retrieves a specific user by their email address asynchronously.
        /// Includes associated roles.
        /// </summary>
        /// <param name="email">The email address of the user to retrieve.</param>
        /// <returns>A task representing the asynchronous operation, containing the <see cref="UserBusinessLogicModel"/> if found, or null if not found.</returns>
        Task<UserBusinessLogicModel?> GetByEmailAsync(string email);

        /// <summary>
        /// Creates a new user asynchronously with the specified details and assigns roles.
        /// Validates that the email is unique, at least one role is provided, and all roles exist.
        /// Hashes the password securely.
        /// </summary>
        /// <param name="firstName">The first name of the user.</param>
        /// <param name="lastName">The last name of the user.</param>
        /// <param name="email">The email address of the user (must be unique).</param>
        /// <param name="password">The plain text password to be hashed and stored.</param>
        /// <param name="roleNames">A collection of role names to assign to the user.</param>
        /// <returns>A task representing the asynchronous operation, containing the created <see cref="UserBusinessLogicModel"/> with roles populated.</returns>
        /// <exception cref="ArgumentException">Thrown if no roles are provided.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the email already exists or if some roles do not exist.</exception>
        Task<UserBusinessLogicModel> CreateUserAsync(string firstName, string lastName, string email, string password, IEnumerable<string> roleNames);

        /// <summary>
        /// Verifies a user's password asynchronously by comparing the provided password with the stored hash.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <param name="password">The plain text password to verify.</param>
        /// <returns>A task representing the asynchronous operation, containing true if the password is correct, false otherwise.</returns>
        Task<bool> VerifyPasswordAsync(string email, string password);

        /// <summary>
        /// Checks if a user has a specific role assigned asynchronously.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="roleName">The name of the role to check for (case-insensitive).</param>
        /// <returns>A task representing the asynchronous operation, containing true if the user has the role, false otherwise.</returns>
        Task<bool> UserHasRoleAsync(Guid userId, string roleName);

        /// <summary>
        /// Retrieves a paginated list of tutors asynchronously, with optional filtering by subject area or name.
        /// Includes associated subject areas for each tutor.
        /// </summary>
        /// <param name="subjectAreaId">Optional GUID to filter tutors by a specific subject area.</param>
        /// <param name="subjectAreaName">Optional string to filter tutors by subject area name (partial match, case-insensitive).</param>
        /// <param name="name">Optional string to filter tutors by first or last name (partial match).</param>
        /// <param name="page">The page number to retrieve (1-based).</param>
        /// <param name="pageSize">The number of tutors per page.</param>
        /// <returns>A task representing the asynchronous operation, containing a <see cref="PaginatedResponse{T}"/> with a list of <see cref="TutorProfileResponse"/> and pagination metadata.</returns>
        Task<PaginatedResponse<TutorProfileResponse>> GetTutorsAsync(Guid? subjectAreaId, string? subjectAreaName, string? name, int page, int pageSize);

        /// <summary>
        /// Retrieves detailed information about a specific tutor by their unique identifier asynchronously.
        /// Includes associated subject areas. Only returns users with the TUTOR role.
        /// </summary>
        /// <param name="id">The unique identifier of the tutor to retrieve.</param>
        /// <returns>A task representing the asynchronous operation, containing the <see cref="TutorProfileResponse"/> if found and is a tutor, or null otherwise.</returns>
        Task<TutorProfileResponse?> GetTutorByIdAsync(Guid id);
    }
}
