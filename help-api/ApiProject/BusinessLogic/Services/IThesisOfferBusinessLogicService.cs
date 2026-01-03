using ApiProject.BusinessLogic.Models;

namespace ApiProject.BusinessLogic.Services;

/// <summary>
/// Provides business logic operations for managing thesis offers and their applications in the thesis management system.
/// This service handles the creation, retrieval, and management of thesis offers by tutors and applications by students.
/// </summary>
public interface IThesisOfferBusinessLogicService
{
    /// <summary>
    /// Retrieves a paginated list of all thesis offers asynchronously.
    /// Filters the results based on the user's roles: admins see all offers, tutors see their own offers, students see all offers.
    /// </summary>
    /// <param name="page">The page number to retrieve (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="userId">The unique identifier of the user making the request.</param>
    /// <param name="userRoles">The list of roles assigned to the user.</param>
    /// <returns>A task representing the asynchronous operation, containing a <see cref="PaginatedResultBusinessLogicModel{T}"/> with a list of <see cref="ThesisOfferBusinessLogicModel"/> and pagination metadata.</returns>
    Task<PaginatedResultBusinessLogicModel<ThesisOfferBusinessLogicModel>> GetAllAsync(int page, int pageSize, Guid userId, List<string> userRoles);

    /// <summary>
    /// Retrieves a paginated list of thesis offer applications for a specific user (student).
    /// </summary>
    /// <param name="userId">The unique identifier of the student whose applications are to be retrieved.</param>
    /// <param name="page">The page number to retrieve (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A task representing the asynchronous operation, containing a <see cref="PaginatedResultBusinessLogicModel{T}"/> with a list of <see cref="ThesisOfferApplicationBusinessLogicModel"/> and pagination metadata.</returns>
    Task<PaginatedResultBusinessLogicModel<ThesisOfferApplicationBusinessLogicModel>> GetApplicationsForUserAsync(Guid userId, int page, int pageSize);

    /// <summary>
    /// Creates a new thesis offer asynchronously.
    /// Validates that the creator is a tutor and sets the offer status to "OPEN".
    /// </summary>
    /// <param name="request">The request model containing the details for the new thesis offer.</param>
    /// <returns>A task representing the asynchronous operation, containing the created <see cref="ThesisOfferBusinessLogicModel"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the user is not a tutor.</exception>
    Task<ThesisOfferBusinessLogicModel> CreateAsync(ThesisOfferCreateRequestBusinessLogicModel request);

    /// <summary>
    /// Updates an existing thesis offer asynchronously.
    /// Validates that the updater is the tutor who created the offer and that the offer is still open.
    /// </summary>
    /// <param name="id">The unique identifier of the thesis offer to update.</param>
    /// <param name="request">The request model containing the updated details for the thesis offer.</param>
    /// <param name="userId">The unique identifier of the user attempting to update the offer.</param>
    /// <returns>A task representing the asynchronous operation, containing the updated <see cref="ThesisOfferBusinessLogicModel"/>.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the thesis offer is not found.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the user is not the tutor who created the offer or if the offer is not open.</exception>
    Task<ThesisOfferBusinessLogicModel> UpdateAsync(Guid id, ThesisOfferUpdateRequestBusinessLogicModel request, Guid userId);

    /// <summary>
    /// Creates a new application for a thesis offer asynchronously.
    /// Validates that the thesis offer exists and is in an "OPEN" state.
    /// </summary>
    /// <param name="request">The request model containing the details for the new application.</param>
    /// <returns>A task representing the asynchronous operation, containing a tuple with the created <see cref="ThesisOfferApplicationBusinessLogicModel"/> (or null if creation failed) and an error message (or null if successful).</returns>
    Task<(ThesisOfferApplicationBusinessLogicModel?, string?)> CreateApplicationAsync(ThesisOfferApplicationCreateRequestBusinessLogicModel request);
}