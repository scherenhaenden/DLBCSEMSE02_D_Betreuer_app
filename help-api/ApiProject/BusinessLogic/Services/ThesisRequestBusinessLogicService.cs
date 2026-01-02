using ApiProject.ApiLogic.Models;
using ApiProject.DatabaseAccess.Context;
using ApiProject.DatabaseAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiProject.Constants;

namespace ApiProject.BusinessLogic.Services
{
    public class ThesisRequestBusinessLogicService : IThesisRequestBusinessLogicService
    {
        private readonly ThesisDbContext _context;

        public ThesisRequestBusinessLogicService(ThesisDbContext context)
        {
            _context = context;
        }

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

        public async Task<IEnumerable<ThesisRequestResponse>> GetRequestsForUserAsync(Guid userId)
        {
            return await _context.ThesisRequests
                .Include(r => r.Thesis)
                .Include(r => r.Requester).ThenInclude(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .Include(r => r.Receiver).ThenInclude(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .Include(r => r.RequestType)
                .Include(r => r.Status)
                .Where(r => r.ReceiverId == userId || r.RequesterId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => MapToResponse(r))
                .ToListAsync();
        }

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

        public async Task<ThesisRequestResponse> CreatedStudentRequestForTutor(Guid studentId, Guid tutorId, Guid thesisId, string? message)
        {
            return await CreateRequestAsync(studentId, thesisId, tutorId, RequestTypes.Supervision, message);
        }

        public async Task<ThesisRequestResponse> CreatedTutorRequestForSecondSupervisor(Guid tutorId, Guid secondSupervisorId, Guid thesisId, string? message)
        {
            return await CreateRequestAsync(tutorId, thesisId, secondSupervisorId, RequestTypes.CoSupervision, message);
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
