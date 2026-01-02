using ApiProject.BusinessLogic.Models;

namespace ApiProject.BusinessLogic.Services
{
    /// <summary>
    /// Interface for the Topic Service, providing CRUD operations for topics.
    /// </summary>
    public interface ISubjectAreaService
    {
        /// <summary>
        /// Returns all topics in a paginated result.
        /// </summary>
        Task<PaginatedResultBusinessLogicModel<SubjectAreaBusinessLogicModel>> GetAllAsync(int page, int pageSize);

        /// <summary>
        /// Returns a topic by its ID.
        /// </summary>
        Task<SubjectAreaBusinessLogicModel?> GetByIdAsync(Guid id);

        /// <summary>
        /// Searches for topics by title or subject area.
        /// </summary>
        Task<PaginatedResultBusinessLogicModel<SubjectAreaBusinessLogicModel>> SearchAsync(string searchTerm, int page, int pageSize);

        /// <summary>
        /// Creates a new topic.
        /// </summary>
        Task<SubjectAreaBusinessLogicModel> CreateTopicAsync(SubjectAreaCreateRequestBusinessLogicModel request);

        /// <summary>
        /// Updates an existing topic.
        /// </summary>
        Task<SubjectAreaBusinessLogicModel> UpdateTopicAsync(Guid id, SubjectAreaUpdateRequestBusinessLogicModel request);

        /// <summary>
        /// Deletes a topic by its ID.
        /// </summary>
        Task<bool> DeleteTopicAsync(Guid id);
    }
}
