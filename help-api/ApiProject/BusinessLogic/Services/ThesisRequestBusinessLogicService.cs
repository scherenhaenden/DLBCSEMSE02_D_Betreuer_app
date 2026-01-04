using ApiProject.ApiLogic.Models;
using ApiProject.DatabaseAccess.Context;
using ApiProject.DatabaseAccess.Entities;
using Microsoft.EntityFrameworkCore;
using ApiProject.Constants;
using ApiProject.BusinessLogic.Models;

namespace ApiProject.BusinessLogic.Services
{
    public class ThesisRequestBusinessLogicService : IThesisRequestBusinessLogicService
    {
        private readonly ThesisDbContext _context;

        public ThesisRequestBusinessLogicService(ThesisDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates a new thesis request with comprehensive validation of roles, expertise, and constraints.
        /// This method enforces business rules for supervision and co-supervision requests.
        /// 
        /// What: Creates a thesis request record linking a requester to a receiver for a specific thesis and request type.
        /// How: Validates thesis existence, user roles, subject area expertise, and request-specific constraints.
        /// For supervision: ensures requester is thesis owner (student), for co-supervision: ensures requester is main supervisor (tutor) and distinct from receiver.
        /// Sets status to pending, saves the request, and returns the full request details.
        /// Why: Enables structured request process for thesis supervision assignments.
        /// Ensures only authorized users can make requests and receivers are qualified tutors.
        /// Maintains data integrity and enforces workflow constraints.
        /// </summary>
        /// <param name="requesterId">The unique identifier of the user making the request.</param>
        /// <param name="thesisId">The unique identifier of the thesis associated with the request.</param>
        /// <param name="receiverId">The unique identifier of the user receiving the request (must be a tutor).</param>
        /// <param name="requestType">The type of request, either "SUPERVISION" or "CO_SUPERVISION".</param>
        /// <param name="message">An optional message accompanying the request.</param>
        /// <returns>The created thesis request response with full details.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the thesis is not found.</exception>
        /// <exception cref="ArgumentException">Thrown if the request type is invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown if validation constraints are not met.</exception>
        public async Task<ThesisRequestResponse> CreateRequestAsync(Guid requesterId, Guid thesisId, Guid receiverId, string requestType, string? message)
        {
            var thesis = await _context.Theses.FindAsync(thesisId);
            if (thesis == null) throw new KeyNotFoundException("Thesis not found.");

            var requester = await _context.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).SingleAsync(u => u.Id == requesterId);
            var receiver = await _context.Users
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .Include(u => u.UserToSubjectAreas) // Include subject areas to validate expertise
                .SingleAsync(u => u.Id == receiverId);

            var requestTypeEntity = await _context.RequestTypes.SingleOrDefaultAsync(rt => rt.Name == requestType.ToUpper());
            if (requestTypeEntity == null) throw new ArgumentException("Invalid request type.", nameof(requestType));

            // --- Constraint Validation ---
            if (!receiver.UserRoles.Any(r => r.Role.Name == Roles.Tutor))
                throw new InvalidOperationException("The receiver of a request must be a TUTOR.");

            // Validate subject area Expertise (Constraint 2.3)
            if (thesis.SubjectAreaId.HasValue && !receiver.UserToSubjectAreas.Any(ut => ut.SubjectAreaId == thesis.SubjectAreaId.Value))
            {
                throw new InvalidOperationException("The selected tutor does not cover the subject area of this thesis.");
            }

            if (requestTypeEntity.Name == RequestTypes.Supervision)
            {
                if (!requester.UserRoles.Any(r => r.Role.Name == Roles.Student) || thesis.OwnerId != requesterId)
                    throw new InvalidOperationException("Only the thesis owner (STUDENT) can request supervision.");
            }
            else if (requestTypeEntity.Name == RequestTypes.CoSupervision)
            {
                if (!requester.UserRoles.Any(r => r.Role.Name == Roles.Tutor) || thesis.TutorId != requesterId)
                    throw new InvalidOperationException("Only the main supervisor (TUTOR) can request co-supervision.");
                
                // Validate Supervisor Distinctness (Constraint 2.4)
                if (receiverId == requesterId)
                {
                    throw new InvalidOperationException("The second supervisor cannot be the same as the main supervisor.");
                }
            }
            // --- End Validation ---

            var pendingStatus = await _context.RequestStatuses.SingleAsync(rs => rs.Name == RequestStatuses.Pending);

            var newRequest = new ThesisRequestDataAccessModel
            {
                RequesterId = requesterId,
                ReceiverId = receiverId,
                ThesisId = thesisId,
                RequestTypeId = requestTypeEntity.Id,
                StatusId = pendingStatus.Id,
                Message = message
            };

            _context.ThesisRequests.Add(newRequest);
            await _context.SaveChangesAsync();

            return await GetRequestByIdAsync(newRequest.Id);
        }

        /// <summary>
        /// Retrieves all thesis requests associated with a specific user, either as requester or receiver.
        /// Requests are ordered by creation date in descending order (most recent first).
        /// 
        /// What: Fetches a paginated list of thesis requests where the user is involved, including full details of thesis, users, types, and statuses.
        /// How: Builds a query including all related entities for comprehensive data loading.
        /// Filters requests where the user is either the requester or receiver, orders by creation date descending.
        /// Applies pagination using Skip and Take based on page and pageSize.
        /// Retrieves the total count for pagination metadata.
        /// Maps each request to the response model using the private MapToResponse method.
        /// Why: Allows users to view their request history and pending actions with pagination for performance.
        /// Provides complete context for each request including participant details and current status.
        /// Supports user dashboards and notification systems with efficient data retrieval.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose requests are to be retrieved.</param>
        /// <param name="page">The page number for pagination (1-based).</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>A paginated result containing the list of thesis request responses and pagination metadata.</returns>
        public async Task<PaginatedResultBusinessLogicModel<ThesisRequestResponse>> GetRequestsForUserAsync(Guid userId, int page, int pageSize)
        {
            var query = _context.ThesisRequests
                .Include(r => r.Thesis)
                .Include(r => r.Requester).ThenInclude(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .Include(r => r.Receiver).ThenInclude(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .Include(r => r.RequestType)
                .Include(r => r.Status)
                .Where(r => r.ReceiverId == userId || r.RequesterId == userId)
                .OrderByDescending(r => r.CreatedAt);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => MapToResponse(r))
                .ToListAsync();

            return new PaginatedResultBusinessLogicModel<ThesisRequestResponse>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// Retrieves a specific thesis request by its unique identifier, including all related details.
        /// 
        /// What: Fetches detailed information about a single thesis request from the database.
        /// How: Queries the ThesisRequests table with eager loading of all related entities (thesis, users with roles, request type, status).
        /// Uses SingleOrDefaultAsync to find the request by ID, returning null if not found.
        /// Maps the data access model to the response model using the private MapToResponse method.
        /// Why: Allows detailed viewing of individual requests for review or action.
        /// Ensures all necessary data is loaded to provide complete context without additional queries.
        /// Supports request detail pages and administrative oversight.
        /// </summary>
        /// <param name="requestId">The unique identifier of the thesis request to retrieve.</param>
        /// <returns>The thesis request response if found, otherwise null.</returns>
        public async Task<ThesisRequestResponse?> GetRequestByIdAsync(Guid requestId)
        {
            var request = await _context.ThesisRequests
                .Include(r => r.Thesis)
                .Include(r => r.Requester).ThenInclude(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .Include(r => r.Receiver).ThenInclude(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .Include(r => r.RequestType)
                .Include(r => r.Status)
                .SingleOrDefaultAsync(r => r.Id == requestId);

            return request == null ? null : MapToResponse(request);
        }

        /// <summary>
        /// Allows the receiver of a thesis request to respond to it, either accepting or rejecting.
        /// If accepted, updates the thesis with the appropriate supervisor assignment.
        /// 
        /// What: Processes a response to a thesis request, updating its status and potentially assigning supervisors to the thesis.
        /// How: Retrieves the request with thesis details, validates the responder is the receiver.
        /// Updates the request status to accepted or rejected based on the response.
        /// If accepted, assigns the receiver as the main supervisor (for supervision) or second supervisor (for co-supervision).
        /// Saves all changes to the database.
        /// Why: Enables tutors to approve or decline supervision requests, completing the assignment workflow.
        /// Ensures only the intended receiver can respond to prevent unauthorized changes.
        /// Automatically updates thesis assignments upon acceptance for seamless workflow.
        /// </summary>
        /// <param name="requestId">The unique identifier of the request being responded to.</param>
        /// <param name="receiverId">The unique identifier of the user responding (must be the receiver).</param>
        /// <param name="accepted">True to accept the request, false to reject it.</param>
        /// <param name="message">An optional response message.</param>
        /// <exception cref="KeyNotFoundException">Thrown if the request is not found.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown if the user is not authorized to respond to the request.</exception>
        public async Task RespondToRequestAsync(Guid requestId, Guid receiverId, bool accepted, string? message)
        {
            var request = await _context.ThesisRequests
                .Include(r => r.Thesis)
                .SingleOrDefaultAsync(r => r.Id == requestId);

            if (request == null) throw new KeyNotFoundException("Request not found.");
            if (request.ReceiverId != receiverId) throw new UnauthorizedAccessException("You are not authorized to respond to this request.");

            var newStatusName = accepted ? RequestStatuses.Accepted : RequestStatuses.Rejected;
            var newStatus = await _context.RequestStatuses.SingleAsync(rs => rs.Name == newStatusName);
            request.StatusId = newStatus.Id;

            if (accepted)
            {
                var requestType = await _context.RequestTypes.FindAsync(request.RequestTypeId);
                if (requestType.Name == RequestTypes.Supervision)
                {
                    request.Thesis.TutorId = request.ReceiverId;
                }
                else if (requestType.Name == RequestTypes.CoSupervision)
                {
                    request.Thesis.SecondSupervisorId = request.ReceiverId;
                }
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Creates a supervision request from a student to a tutor for a specific thesis.
        /// This is a convenience method for requesting primary supervision.
        /// 
        /// What: Initiates a supervision request where a student seeks a tutor as their primary supervisor.
        /// How: Calls the general CreateRequestAsync method with the supervision request type.
        /// Passes the student as requester, tutor as receiver, and supervision as the request type.
        /// Why: Simplifies the process for students to request supervision without specifying the request type manually.
        /// Encapsulates the common use case of student-to-tutor supervision requests.
        /// Maintains consistency with the underlying request creation logic.
        /// </summary>
        /// <param name="studentId">The unique identifier of the student making the request.</param>
        /// <param name="tutorId">The unique identifier of the tutor receiving the request.</param>
        /// <param name="thesisId">The unique identifier of the thesis for which supervision is requested.</param>
        /// <param name="message">An optional message accompanying the request.</param>
        /// <returns>The created thesis request response with full details.</returns>
        public async Task<ThesisRequestResponse> CreatedStudentRequestForTutor(Guid studentId, Guid tutorId, Guid thesisId, string? message)
        {
            return await CreateRequestAsync(studentId, thesisId, tutorId, RequestTypes.Supervision, message);
        }

        /// <summary>
        /// Creates a co-supervision request from a tutor to another tutor for a specific thesis.
        /// This is used when the main supervisor requests a second supervisor.
        /// 
        /// What: Initiates a co-supervision request where the main supervisor seeks an additional tutor for secondary supervision.
        /// How: Calls the general CreateRequestAsync method with the co-supervision request type.
        /// Passes the main tutor as requester, second tutor as receiver, and co-supervision as the request type.
        /// Why: Simplifies the process for main supervisors to request co-supervision without specifying the request type manually.
        /// Encapsulates the specific use case of tutor-to-tutor co-supervision requests.
        /// Maintains consistency with the underlying request creation logic and validation.
        /// </summary>
        /// <param name="tutorId">The unique identifier of the main tutor making the request.</param>
        /// <param name="secondSupervisorId">The unique identifier of the second supervisor receiving the request.</param>
        /// <param name="thesisId">The unique identifier of the thesis for which co-supervision is requested.</param>
        /// <param name="message">An optional message accompanying the request.</param>
        /// <returns>The created thesis request response with full details.</returns>
        public async Task<ThesisRequestResponse> CreatedTutorRequestForSecondSupervisor(Guid tutorId, Guid secondSupervisorId, Guid thesisId, string? message)
        {
            return await CreateRequestAsync(tutorId, thesisId, secondSupervisorId, RequestTypes.CoSupervision, message);
        }

        /// <summary>
        /// Retrieves all thesis requests where the specified tutor is the receiver.
        /// Requests are ordered by creation date in descending order.
        /// 
        /// What: Fetches a paginated list of thesis requests where the tutor is the receiver, including full details of thesis, users, types, and statuses.
        /// How: Builds a query including all related entities for comprehensive data loading.
        /// Filters requests where the tutor is the receiver, orders by creation date descending.
        /// Applies pagination using Skip and Take based on page and pageSize.
        /// Retrieves the total count for pagination metadata.
        /// Maps each request to the response model using the private MapToResponse method.
        /// Why: Allows tutors to view their incoming request history and pending actions with pagination for performance.
        /// Provides complete context for each request including participant details and current status.
        /// Supports tutor dashboards and notification systems with efficient data retrieval.
        /// </summary>
        /// <param name="tutorId">The unique identifier of the tutor who is the receiver of the requests.</param>
        /// <param name="page">The page number for pagination (1-based).</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>A paginated result containing the list of thesis request responses and pagination metadata.</returns>
        public async Task<PaginatedResultBusinessLogicModel<ThesisRequestResponse>> GetRequestsForTutorAsReceiver(Guid tutorId, int page, int pageSize)
        {
            var query = _context.ThesisRequests
                .Include(r => r.Thesis)
                .Include(r => r.Requester).ThenInclude(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .Include(r => r.Receiver).ThenInclude(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .Include(r => r.RequestType)
                .Include(r => r.Status)
                .Where(r => r.ReceiverId == tutorId)
                .OrderByDescending(r => r.CreatedAt);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => MapToResponse(r))
                .ToListAsync();

            return new PaginatedResultBusinessLogicModel<ThesisRequestResponse>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// Retrieves all thesis requests where the specified tutor is the requester.
        /// Requests are ordered by creation date in descending order.
        /// 
        /// What: Fetches a paginated list of thesis requests where the tutor is the requester, including full details of thesis, users, types, and statuses.
        /// How: Builds a query including all related entities for comprehensive data loading.
        /// Filters requests where the tutor is the requester, orders by creation date descending.
        /// Applies pagination using Skip and Take based on page and pageSize.
        /// Retrieves the total count for pagination metadata.
        /// Maps each request to the response model using the private MapToResponse method.
        /// Why: Allows tutors to view their outgoing request history and pending actions with pagination for performance.
        /// Provides complete context for each request including participant details and current status.
        /// Supports tutor dashboards and notification systems with efficient data retrieval.
        /// </summary>
        /// <param name="tutorId">The unique identifier of the tutor who is the requester of the requests.</param>
        /// <param name="page">The page number for pagination (1-based).</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>A paginated result containing the list of thesis request responses and pagination metadata.</returns>
        public async Task<PaginatedResultBusinessLogicModel<ThesisRequestResponse>> GetRequestsForTutorAsRequester(Guid tutorId, int page, int pageSize)
        {
            var query = _context.ThesisRequests
                .Include(r => r.Thesis)
                .Include(r => r.Requester).ThenInclude(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .Include(r => r.Receiver).ThenInclude(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .Include(r => r.RequestType)
                .Include(r => r.Status)
                .Where(r => r.RequesterId == tutorId)
                .OrderByDescending(r => r.CreatedAt);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => MapToResponse(r))
                .ToListAsync();

            return new PaginatedResultBusinessLogicModel<ThesisRequestResponse>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        private static ThesisRequestResponse MapToResponse(ThesisRequestDataAccessModel r)
        {
            return new ThesisRequestResponse
            {
                Id = r.Id,
                ThesisId = r.ThesisId,
                ThesisTitle = r.Thesis.Title,
                Requester = new UserResponse { Id = r.Requester.Id, FirstName = r.Requester.FirstName, LastName = r.Requester.LastName, Email = r.Requester.Email, Roles = r.Requester.UserRoles.Select(ur => ur.Role.Name).ToList() },
                Receiver = new UserResponse { Id = r.Receiver.Id, FirstName = r.Receiver.FirstName, LastName = r.Receiver.LastName, Email = r.Receiver.Email, Roles = r.Receiver.UserRoles.Select(ur => ur.Role.Name).ToList() },
                RequestType = r.RequestType.Name,
                Status = r.Status.Name,
                Message = r.Message,
                CreatedAt = r.CreatedAt
            };
        }
    }
}
