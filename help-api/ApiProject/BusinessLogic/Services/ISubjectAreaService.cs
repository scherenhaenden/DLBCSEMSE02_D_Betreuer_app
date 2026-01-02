using ApiProject.BusinessLogic.Models;

namespace ApiProject.BusinessLogic.Services
{
    /// <summary>
    /// Interface for the Subject Area Service, providing CRUD operations for subject areas.
    /// </summary>
    public interface ISubjectAreaService
    {
        /// <summary>
        /// Returns all subject areas in a paginated result.
        /// </summary>
        Task<PaginatedResultBusinessLogicModel<SubjectAreaBusinessLogicModel>> GetAllAsync(int page, int pageSize);

        /// <summary>
        /// Returns a subject area by its ID.
        /// </summary>
        Task<SubjectAreaBusinessLogicModel?> GetByIdAsync(Guid id);

        /// <summary>
        /// Searches for subject areas by title or subject area.
        /// </summary>
        Task<PaginatedResultBusinessLogicModel<SubjectAreaBusinessLogicModel>> SearchAsync(string searchTerm, int page, int pageSize);

        /// <summary>
        /// Creates a new subject area.
        /// </summary>
        Task<SubjectAreaBusinessLogicModel> CreateSubjectAreaAsync(SubjectAreaCreateRequestBusinessLogicModel request);

        /// <summary>
        /// Updates an existing subject area.
        /// </summary>
        Task<SubjectAreaBusinessLogicModel> UpdateSubjectAreaAsync(Guid id, SubjectAreaUpdateRequestBusinessLogicModel request);

        /// <summary>
        /// Deletes a subject area by its ID.
        /// </summary>
        Task<bool> DeleteSubjectAreaAsync(Guid id);
    }
}
