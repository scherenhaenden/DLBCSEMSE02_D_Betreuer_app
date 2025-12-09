using ApiProject.BusinessLogic.Models;

namespace ApiProject.BusinessLogic.Services
{
    /// <summary>
    /// Interface for the Topic Service, providing CRUD operations for topics.
    /// </summary>
    public interface ITopicService
    {
        /// <summary>
        /// Returns all topics in a paginated result.
        /// </summary>
        Task<PaginatedResultBusinessLogicModel<TopicBusinessLogicModel>> GetAllAsync(int page, int pageSize);

        /// <summary>
        /// Returns a topic by its ID.
        /// </summary>
        Task<TopicBusinessLogicModel?> GetByIdAsync(Guid id);

        /// <summary>
        /// Searches for topics by title or subject area.
        /// </summary>
        Task<PaginatedResultBusinessLogicModel<TopicBusinessLogicModel>> SearchAsync(string searchTerm, int page, int pageSize);

        /// <summary>
        /// Creates a new topic.
        /// </summary>
        Task<TopicBusinessLogicModel> CreateTopicAsync(TopicCreateRequestBusinessLogicModel request);

        /// <summary>
        /// Updates an existing topic.
        /// </summary>
        Task<TopicBusinessLogicModel> UpdateTopicAsync(Guid id, TopicUpdateRequestBusinessLogicModel request);

        /// <summary>
        /// Deletes a topic by its ID.
        /// </summary>
        Task<bool> DeleteTopicAsync(Guid id);
    }
}
