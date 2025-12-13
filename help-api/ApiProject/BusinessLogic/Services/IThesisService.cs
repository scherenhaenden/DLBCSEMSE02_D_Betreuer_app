using ApiProject.BusinessLogic.Models;
using System.Collections.Generic;

namespace ApiProject.BusinessLogic.Services
{
    /// <summary>
    /// Interface for the Thesis Service, providing CRUD operations for theses.
    /// </summary>
    public interface IThesisService
    {
        /// <summary>
        /// Returns all theses in a paginated result, filtered by user roles and ID.
        /// </summary>
        /// <param name="page">The page number (1-based).</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="userId">The ID of the current user.</param>
        /// <param name="userRoles">The roles of the current user.</param>
        /// <returns>A paginated result containing the theses.</returns>
        Task<PaginatedResultBusinessLogicModel<ThesisBusinessLogicModel>> GetAllAsync(int page, int pageSize, Guid userId, List<string> userRoles);

        /// <summary>
        /// Returns a thesis by its ID.
        /// </summary>
        /// <param name="id">The GUID of the thesis.</param>
        /// <returns>The thesis or null if not found.</returns>
        Task<ThesisBusinessLogicModel?> GetByIdAsync(Guid id);

        /// <summary>
        /// Creates a new thesis based on the request.
        /// </summary>
        /// <param name="request">The request containing the new thesis details.</param>
        /// <returns>The created thesis.</returns>
        /// <exception cref="InvalidOperationException">Thrown if validations fail.</exception>
        Task<ThesisBusinessLogicModel> CreateThesisAsync(ThesisCreateRequestBusinessLogicModel request);

        /// <summary>
        /// Updates an existing thesis.
        /// </summary>
        /// <param name="id">The GUID of the thesis to update.</param>
        /// <param name="request">The request with the fields to update.</param>
        /// <returns>The updated thesis.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the thesis is not found.</exception>
        /// <exception cref="InvalidOperationException">Thrown if validations fail.</exception>
        Task<ThesisBusinessLogicModel> UpdateThesisAsync(Guid id, ThesisUpdateRequestBusinessLogicModel request);

        /// <summary>
        /// Deletes a thesis by its ID.
        /// </summary>
        /// <param name="id">The GUID of the thesis to delete.</param>
        /// <returns>True if the thesis was deleted; otherwise, false.</returns>
        Task<bool> DeleteThesisAsync(Guid id);
    }
}
