using ApiProject.BusinessLogic.Models;

namespace ApiProject.BusinessLogic.Services
{
    /// <summary>
    /// Provides business logic operations for managing theses in the thesis management system.
    /// This service handles CRUD operations for theses, including role-based access control and status management.
    /// </summary>
    public interface IThesisBusinessLogicService
    {
        /// <summary>
        /// Retrieves a paginated list of theses asynchronously, filtered based on user roles and permissions.
        /// Admins see all theses, tutors see theses they supervise, students see their own theses.
        /// Includes related status, billing status, and document information.
        /// </summary>
        /// <param name="page">The page number to retrieve (1-based).</param>
        /// <param name="pageSize">The number of theses per page.</param>
        /// <param name="userId">The unique identifier of the user making the request.</param>
        /// <param name="userRoles">The list of roles assigned to the user.</param>
        /// <returns>A task representing the asynchronous operation, containing a <see cref="PaginatedResultBusinessLogicModel{T}"/> with a list of <see cref="ThesisBusinessLogicModel"/> and pagination metadata.</returns>
        Task<PaginatedResultBusinessLogicModel<ThesisBusinessLogicModel>> GetAllAsync(int page, int pageSize, Guid userId, List<string> userRoles);

        /// <summary>
        /// Retrieves a specific thesis by its unique identifier asynchronously.
        /// Includes related status, billing status, and document information.
        /// </summary>
        /// <param name="id">The unique identifier of the thesis to retrieve.</param>
        /// <returns>A task representing the asynchronous operation, containing the <see cref="ThesisBusinessLogicModel"/> if found, or null if not found.</returns>
        Task<ThesisBusinessLogicModel?> GetByIdAsync(Guid id);

        /// <summary>
        /// Creates a new thesis asynchronously.
        /// Validates that the owner has the STUDENT role and sets initial statuses (Registered for thesis, None for billing).
        /// Supervisors are assigned later via requests. Optionally attaches a document.
        /// </summary>
        /// <param name="request">The request model containing the details for the new thesis.</param>
        /// <returns>A task representing the asynchronous operation, containing the created <see cref="ThesisBusinessLogicModel"/>.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the owner does not have the STUDENT role.</exception>
        Task<ThesisBusinessLogicModel> CreateThesisAsync(ThesisCreateRequestBusinessLogicModel request);

        /// <summary>
        /// Updates an existing thesis asynchronously.
        /// Enforces status-based restrictions: blocks updates if the thesis is Submitted or Defended, and blocks subject area changes after registration.
        /// Allows updating title, subject area (if allowed), and document.
        /// </summary>
        /// <param name="id">The unique identifier of the thesis to update.</param>
        /// <param name="request">The request model containing the updated details for the thesis.</param>
        /// <returns>A task representing the asynchronous operation, containing the updated <see cref="ThesisBusinessLogicModel"/>.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the thesis is not found.</exception>
        /// <exception cref="InvalidOperationException">Thrown if updates are not allowed based on the current status.</exception>
        Task<ThesisBusinessLogicModel> UpdateThesisAsync(Guid id, ThesisUpdateRequestBusinessLogicModel request);

        /// <summary>
        /// Deletes a thesis by its unique identifier asynchronously.
        /// This operation will cascade-delete related ThesisRequest entities per EF configuration.
        /// Only the thesis owner or users with Admin role may delete a thesis.
        /// </summary>
        /// <param name="id">The unique identifier of the thesis to delete.</param>
        /// <param name="userId">The unique identifier of the user attempting the deletion.</param>
        /// <param name="userRoles">The list of roles assigned to the user.</param>
        /// <returns>A task representing the asynchronous operation, containing the result of the deletion attempt.</returns>
        Task<DeleteThesisResult> DeleteThesisAsync(Guid id, Guid userId, List<string> userRoles);

        /// <summary>
        /// Retrieves all billing statuses asynchronously.
        /// Billing statuses represent the payment states of theses (e.g., None, Issued, Paid).
        /// </summary>
        /// <returns>A task representing the asynchronous operation, containing a list of <see cref="BillingStatusBusinessLogicModel"/>.</returns>
        Task<List<BillingStatusBusinessLogicModel>> GetAllBillingStatusesAsync();
    }
}
