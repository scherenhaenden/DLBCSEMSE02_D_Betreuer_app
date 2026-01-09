using ApiProject.BusinessLogic.Models;

namespace ApiProject.BusinessLogic.Services
{
    /// <summary>
    /// Provides business logic operations for managing subject areas in the thesis management system.
    /// This service handles CRUD operations for subject areas, including assignment of tutors to subject areas.
    /// </summary>
    public interface ISubjectAreaBusinessLogicService
    {
        /// <summary>
        /// Retrieves a paginated list of all subject areas asynchronously.
        /// Includes associated tutors for each subject area.
        /// </summary>
        /// <param name="page">The page number to retrieve (1-based).</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>A task representing the asynchronous operation, containing a <see cref="PaginatedResultBusinessLogicModel{T}"/> with a list of <see cref="SubjectAreaBusinessLogicModel"/> and pagination metadata.</returns>
        Task<PaginatedResultBusinessLogicModel<SubjectAreaBusinessLogicModel>> GetAllAsync(int page, int pageSize);

        /// <summary>
        /// Retrieves a specific subject area by its unique identifier asynchronously.
        /// Includes associated tutors for the subject area.
        /// </summary>
        /// <param name="id">The unique identifier of the subject area to retrieve.</param>
        /// <returns>A task representing the asynchronous operation, containing the <see cref="SubjectAreaBusinessLogicModel"/> if found, or null if not found.</returns>
        Task<SubjectAreaBusinessLogicModel?> GetByIdAsync(Guid id);

        /// <summary>
        /// Searches for subject areas by title asynchronously.
        /// Returns a paginated list of matching subject areas, including associated tutors.
        /// </summary>
        /// <param name="searchTerm">The search term to match against the subject area titles.</param>
        /// <param name="page">The page number to retrieve (1-based).</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>A task representing the asynchronous operation, containing a <see cref="PaginatedResultBusinessLogicModel{T}"/> with a list of matching <see cref="SubjectAreaBusinessLogicModel"/> and pagination metadata.</returns>
        Task<PaginatedResultBusinessLogicModel<SubjectAreaBusinessLogicModel>> SearchAsync(string searchTerm, int page, int pageSize);

        /// <summary>
        /// Creates a new subject area asynchronously.
        /// Validates that the title is not empty and that all specified tutors have the TUTOR role.
        /// Assigns the subject area to the specified tutors and sets it as active.
        /// </summary>
        /// <param name="request">The request model containing the details for the new subject area.</param>
        /// <returns>A task representing the asynchronous operation, containing the created <see cref="SubjectAreaBusinessLogicModel"/>.</returns>
        /// <exception cref="ArgumentException">Thrown if the title is null, empty, or whitespace.</exception>
        /// <exception cref="InvalidOperationException">Thrown if any specified user does not have the TUTOR role.</exception>
        Task<SubjectAreaBusinessLogicModel> CreateSubjectAreaAsync(SubjectAreaCreateRequestBusinessLogicModel request);

        /// <summary>
        /// Updates an existing subject area asynchronously.
        /// Validates that the subject area exists, the title is not empty if provided, and that all specified tutors have the TUTOR role.
        /// Updates the subject area's properties and tutor assignments as specified.
        /// </summary>
        /// <param name="id">The unique identifier of the subject area to update.</param>
        /// <param name="request">The request model containing the updated details for the subject area.</param>
        /// <returns>A task representing the asynchronous operation, containing the updated <see cref="SubjectAreaBusinessLogicModel"/>.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the subject area is not found.</exception>
        /// <exception cref="ArgumentException">Thrown if the title is provided but is null, empty, or whitespace.</exception>
        /// <exception cref="InvalidOperationException">Thrown if any specified user does not have the TUTOR role.</exception>
        Task<SubjectAreaBusinessLogicModel> UpdateSubjectAreaAsync(Guid id, SubjectAreaUpdateRequestBusinessLogicModel request);

        /// <summary>
        /// Deletes a subject area by its unique identifier asynchronously.
        /// </summary>
        /// <param name="id">The unique identifier of the subject area to delete.</param>
        /// <returns>A task representing the asynchronous operation, containing true if the subject area was deleted, or false if it was not found.</returns>
        Task<bool> DeleteSubjectAreaAsync(Guid id);
    }
}
