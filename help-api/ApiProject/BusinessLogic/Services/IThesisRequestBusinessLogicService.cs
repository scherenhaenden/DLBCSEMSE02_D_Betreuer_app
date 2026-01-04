using ApiProject.ApiLogic.Models;
using ApiProject.BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiProject.BusinessLogic.Services
{
    /// <summary>
    /// Provides business logic operations for managing thesis requests in the thesis management system.
    /// This service handles the creation, retrieval, and response to requests related to thesis supervision and co-supervision.
    /// </summary>
    public interface IThesisRequestBusinessLogicService
    {
        /// <summary>
        /// Creates a new thesis request asynchronously.
        /// Validates constraints such as user roles, subject area expertise, and request type permissions.
        /// </summary>
        /// <param name="requesterId">The unique identifier of the user making the request.</param>
        /// <param name="thesisId">The unique identifier of the thesis associated with the request.</param>
        /// <param name="receiverId">The unique identifier of the user receiving the request (must be a tutor).</param>
        /// <param name="requestType">The type of the request, either "SUPERVISION" or "CO_SUPERVISION".</param>
        /// <param name="message">An optional message providing additional details for the request.</param>
        /// <returns>A task representing the asynchronous operation, containing the <see cref="ThesisRequestResponse"/> for the created request.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the thesis is not found.</exception>
        /// <exception cref="ArgumentException">Thrown if the request type is invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown if validation constraints are not met, such as role requirements or subject area expertise.</exception>
        Task<ThesisRequestResponse> CreateRequestAsync(Guid requesterId, Guid thesisId, Guid receiverId, string requestType, string? message);

        /// <summary>
        /// Creates a supervision request from a student to a tutor for a specific thesis.
        /// This is a convenience method for requesting primary supervision.
        /// </summary>
        /// <param name="studentId">The unique identifier of the student making the request.</param>
        /// <param name="tutorId">The unique identifier of the tutor receiving the request.</param>
        /// <param name="thesisId">The unique identifier of the thesis for which supervision is requested.</param>
        /// <param name="message">An optional message accompanying the request.</param>
        /// <returns>A task representing the asynchronous operation, containing the <see cref="ThesisRequestResponse"/> for the created request.</returns>
        Task<ThesisRequestResponse> CreatedStudentRequestForTutor(Guid studentId, Guid tutorId, Guid thesisId, string? message);

        /// <summary>
        /// Creates a co-supervision request from a tutor to another tutor for a specific thesis.
        /// This is used when the main supervisor requests a second supervisor.
        /// </summary>
        /// <param name="tutorId">The unique identifier of the main tutor making the request.</param>
        /// <param name="secondSupervisorId">The unique identifier of the second supervisor receiving the request.</param>
        /// <param name="thesisId">The unique identifier of the thesis for which co-supervision is requested.</param>
        /// <param name="message">An optional message accompanying the request.</param>
        /// <returns>A task representing the asynchronous operation, containing the <see cref="ThesisRequestResponse"/> for the created request.</returns>
        Task<ThesisRequestResponse> CreatedTutorRequestForSecondSupervisor(Guid tutorId, Guid secondSupervisorId, Guid thesisId, string? message);

        /// <summary>
        /// Retrieves all thesis requests associated with a specific user, either as requester or receiver.
        /// Requests are ordered by creation date in descending order.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose requests are to be retrieved.</param>
        /// <param name="page">The page number for pagination (1-based).</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>A task representing the asynchronous operation, containing a paginated result of <see cref="ThesisRequestResponse"/> objects.</returns>
        Task<Models.PaginatedResultBusinessLogicModel<ThesisRequestResponse>> GetRequestsForUserAsync(Guid userId, int page, int pageSize);

        /// <summary>
        /// Retrieves a specific thesis request by its unique identifier.
        /// </summary>
        /// <param name="requestId">The unique identifier of the request to retrieve.</param>
        /// <returns>A task representing the asynchronous operation, containing the <see cref="ThesisRequestResponse"/> if found, or null if not found.</returns>
        Task<ThesisRequestResponse?> GetRequestByIdAsync(Guid requestId);

        /// <summary>
        /// Allows the receiver of a thesis request to respond to it, either accepting or rejecting.
        /// If accepted, updates the thesis with the appropriate supervisor assignment.
        /// </summary>
        /// <param name="requestId">The unique identifier of the request being responded to.</param>
        /// <param name="receiverId">The unique identifier of the user responding (must be the receiver).</param>
        /// <param name="accepted">True to accept the request, false to reject it.</param>
        /// <param name="message">An optional response message.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the request is not found.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown if the user is not authorized to respond to the request.</exception>
        Task RespondToRequestAsync(Guid requestId, Guid receiverId, bool accepted, string? message);
    }
}
