using ApiProject.BusinessLogic.Mappers;
using ApiProject.BusinessLogic.Models;
using ApiProject.DatabaseAccess.Context;
using ApiProject.DatabaseAccess.Entities;
using Microsoft.EntityFrameworkCore;
using ApiProject.Constants;

namespace ApiProject.BusinessLogic.Services;

public class ThesisOfferBusinessLogicService : IThesisOfferBusinessLogicService
{
    private readonly ThesisDbContext _context;

    public ThesisOfferBusinessLogicService(ThesisDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves a paginated list of thesis offers based on user roles and permissions.
    /// This method supports role-based access control, allowing admins to see all offers, tutors to see their own offers, and students to see all offers.
    /// 
    /// What: Fetches thesis offers with their status information, filtered by user permissions.
    /// How: Builds a queryable starting from ThesisOffers table, includes ThesisOfferStatus.
    /// Applies role-based filters: admins see all, tutors see their own, students see all.
    /// Counts total matching offers for pagination, skips and takes the appropriate page, maps to business models.
    /// Why: Essential for thesis offer management interfaces, ensuring users only access offers relevant to them.
    /// Supports secure data access and efficient pagination for large datasets.
    /// </summary>
    /// <param name="page">The page number to retrieve (1-based).</param>
    /// <param name="pageSize">The number of offers per page.</param>
    /// <param name="userId">The ID of the user making the request.</param>
    /// <param name="userRoles">The list of roles assigned to the user.</param>
    /// <returns>A paginated result containing the list of thesis offers and metadata.</returns>
    public async Task<PaginatedResultBusinessLogicModel<ThesisOfferBusinessLogicModel>> GetAllAsync(int page, int pageSize, Guid userId, List<string> userRoles)
    {
        var query = _context.ThesisOffers
            .Include(to => to.ThesisOfferStatus)
            .AsQueryable();

        if (!userRoles.Contains(Roles.Admin))
        {
            if (userRoles.Contains(Roles.Tutor))
            {
                query = query.Where(to => to.TutorId == userId);
            }
            // For students, show all offers
        }

        var totalCount = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PaginatedResultBusinessLogicModel<ThesisOfferBusinessLogicModel>
        {
            Items = items.Select(ThesisOfferBusinessLogicMapper.ToBusinessModel).ToList(),
            TotalCount = totalCount
        };
    }

    /// <summary>
    /// Retrieves a paginated list of thesis offer applications for a specific student user.
    /// 
    /// What: Fetches applications submitted by the student, including their request status.
    /// How: Builds a queryable from ThesisOfferApplications table, includes RequestStatus, filters by StudentId.
    /// Counts total applications for pagination, skips and takes the appropriate page, maps to business models.
    /// Why: Allows students to view their submitted applications and track their status.
    /// Supports efficient pagination for managing multiple applications.
    /// Ensures students can only see their own applications for privacy.
    /// </summary>
    /// <param name="userId">The unique identifier of the student user.</param>
    /// <param name="page">The page number to retrieve (1-based).</param>
    /// <param name="pageSize">The number of applications per page.</param>
    /// <returns>A paginated result containing the list of applications and metadata.</returns>
    public async Task<PaginatedResultBusinessLogicModel<ThesisOfferApplicationBusinessLogicModel>> GetApplicationsForUserAsync(Guid userId, int page, int pageSize)
    {
        var query = _context.ThesisOfferApplications
            .Include(toa => toa.RequestStatus)
            .Where(toa => toa.StudentId == userId)
            .AsQueryable();

        var totalCount = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PaginatedResultBusinessLogicModel<ThesisOfferApplicationBusinessLogicModel>
        {
            Items = items.Select(ThesisOfferApplicationBusinessLogicMapper.ToBusinessModel).ToList(),
            TotalCount = totalCount
        };
    }

    /// <summary>
    /// Creates a new thesis offer with the specified details, ensuring the creator is a tutor and setting the status to open.
    /// 
    /// What: Creates a new thesis offer record in the database with provided title, description, subject area, and constraints.
    /// How: Validates that the creator has TUTOR role by checking user roles.
    /// Retrieves the "OPEN" status from ThesisOfferStatuses, creates a new ThesisOfferDataAccessModel with provided data.
    /// Saves the offer to the database and returns the mapped business model.
    /// Why: Enables tutors to propose thesis topics for students to apply to.
    /// Ensures only qualified tutors can create offers, maintaining quality and relevance.
    /// Sets appropriate initial status for offer lifecycle management.
    /// </summary>
    /// <param name="request">The request model containing thesis offer creation details.</param>
    /// <returns>The created thesis offer business model.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the user is not a tutor.</exception>
    public async Task<ThesisOfferBusinessLogicModel> CreateAsync(ThesisOfferCreateRequestBusinessLogicModel request)
    {
        var tutorUser = await _context.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).SingleOrDefaultAsync(u => u.Id == request.TutorId);
        if (tutorUser == null || !tutorUser.UserRoles.Any(ur => ur.Role.Name == Roles.Tutor))
        {
            throw new InvalidOperationException("Only tutors can create thesis offers.");
        }

        var openStatus = await _context.ThesisOfferStatuses.SingleAsync(s => s.Name == ThesisOfferStatuses.Open);

        var thesisOffer = new ThesisOfferDataAccessModel
        {
            Title = request.Title,
            Description = request.Description,
            SubjectAreaId = request.SubjectAreaId,
            TutorId = request.TutorId,
            ThesisOfferStatusId = openStatus.Id,
            MaxStudents = request.MaxStudents,
            ExpiresAt = request.ExpiresAt
        };

        _context.ThesisOffers.Add(thesisOffer);
        await _context.SaveChangesAsync();

        return ThesisOfferBusinessLogicMapper.ToBusinessModel(thesisOffer);
    }

    /// <summary>
    /// Updates an existing thesis offer asynchronously.
    /// Validates that the updater is the tutor who created the offer and that the offer is still open.
    /// 
    /// What: Modifies a thesis offer's details based on the provided request.
    /// How: Retrieves the thesis offer by ID, checks if it exists, is in "OPEN" status, and the user is the tutor who created it.
    /// Applies updates to title, description, subject area, max students, and expiration date if provided.
    /// Saves changes to the database and returns the updated business model.
    /// Why: Allows tutors to modify their thesis offers before they expire or are closed.
    /// Ensures only the original tutor can update their offers for security.
    /// Supports flexible partial updates for offer management.
    /// </summary>
    /// <param name="id">The unique identifier of the thesis offer to update.</param>
    /// <param name="request">The request model containing the updated details for the thesis offer.</param>
    /// <param name="userId">The unique identifier of the user attempting to update the offer.</param>
    /// <returns>The updated thesis offer business model.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the thesis offer is not found.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the user is not the tutor who created the offer or if the offer is not open.</exception>
    public async Task<ThesisOfferBusinessLogicModel> UpdateAsync(Guid id, ThesisOfferUpdateRequestBusinessLogicModel request, Guid userId)
    {
        var thesisOffer = await _context.ThesisOffers
            .Include(to => to.ThesisOfferStatus)
            .FirstOrDefaultAsync(to => to.Id == id);

        if (thesisOffer == null)
        {
            throw new KeyNotFoundException("Thesis offer not found.");
        }

        if (thesisOffer.TutorId != userId)
        {
            throw new InvalidOperationException("Only the tutor who created the offer can update it.");
        }

        if (thesisOffer.ThesisOfferStatus?.Name != ThesisOfferStatuses.Open)
        {
            throw new InvalidOperationException("Thesis offer can only be updated if it is open.");
        }

        if (request.Title != null)
        {
            thesisOffer.Title = request.Title;
        }

        if (request.Description != null)
        {
            thesisOffer.Description = request.Description;
        }

        if (request.SubjectAreaId.HasValue)
        {
            thesisOffer.SubjectAreaId = request.SubjectAreaId.Value;
        }

        if (request.MaxStudents.HasValue)
        {
            thesisOffer.MaxStudents = request.MaxStudents.Value;
        }

        if (request.ExpiresAt.HasValue)
        {
            thesisOffer.ExpiresAt = request.ExpiresAt.Value;
        }

        await _context.SaveChangesAsync();

        return ThesisOfferBusinessLogicMapper.ToBusinessModel(thesisOffer);
    }

    /// <summary>
    /// Creates a new application for a thesis offer, validating that the offer exists and is open.
    /// 
    /// What: Creates a new application record linking a student to a thesis offer, with an optional message.
    /// How: Retrieves the thesis offer with its status, checks if it exists and is "OPEN".
    /// Retrieves the "PENDING" status from RequestStatuses, creates a new ThesisOfferApplicationDataAccessModel.
    /// Saves the application to the database and returns the mapped business model with no error.
    /// If offer not found or not open, returns null model with error message.
    /// Why: Allows students to express interest in thesis offers proposed by tutors.
    /// Ensures applications can only be submitted to active, open offers.
    /// Sets initial status to pending for tutor review and approval process.
    /// </summary>
    /// <param name="request">The request model containing application creation details.</param>
    /// <returns>A tuple containing the created application business model (or null) and an error message (or null).</returns>
    public async Task<(ThesisOfferApplicationBusinessLogicModel?, string?)> CreateApplicationAsync(ThesisOfferApplicationCreateRequestBusinessLogicModel request)
    {
        var thesisOffer = await _context.ThesisOffers
            .Include(to => to.ThesisOfferStatus)
            .FirstOrDefaultAsync(to => to.Id == request.ThesisOfferId);

        if (thesisOffer == null)
        {
            return (null, "Thesis offer not found.");
        }

        if (thesisOffer.ThesisOfferStatus?.Name != ThesisOfferStatuses.Open)
        {
            return (null, "Thesis offer does not have open state anymore.");
        }

        var pendingStatus = await _context.RequestStatuses.SingleAsync(s => s.Name == RequestStatuses.Pending);

        var application = new ThesisOfferApplicationDataAccessModel
        {
            ThesisOfferId = request.ThesisOfferId,
            StudentId = request.StudentId,
            RequestStatusId = pendingStatus.Id,
            Message = request.Message
        };

        _context.ThesisOfferApplications.Add(application);
        await _context.SaveChangesAsync();

        return (ThesisOfferApplicationBusinessLogicMapper.ToBusinessModel(application), null);
    }

    /// <summary>
    /// Retrieves a paginated list of thesis offers for a specific user asynchronously.
    /// Filters based on the current user's roles and permissions.
    /// 
    /// What: Fetches thesis offers where the tutor is the specified user, with status information.
    /// How: Builds a queryable starting from ThesisOffers table, includes ThesisOfferStatus, filters by TutorId == userId.
    /// Applies role-based access: allows if current user is admin, student, or the target user (tutor viewing own).
    /// Counts total matching offers for pagination, skips and takes the appropriate page, maps to business models.
    /// Why: Enables users to view thesis offers from a specific tutor, with proper access control.
    /// Supports scenarios like students browsing a tutor's offers.
    /// </summary>
    /// <param name="userId">The unique identifier of the user whose offers are to be retrieved.</param>
    /// <param name="currentUserId">The unique identifier of the current user making the request.</param>
    /// <param name="userRoles">The list of roles assigned to the current user.</param>
    /// <param name="page">The page number to retrieve (1-based).</param>
    /// <param name="pageSize">The number of offers per page.</param>
    /// <returns>A paginated result containing the list of thesis offers for the specified user and metadata.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the current user is not authorized to view the offers.</exception>
    public async Task<PaginatedResultBusinessLogicModel<ThesisOfferBusinessLogicModel>> GetByUserIdAsync(Guid userId, Guid currentUserId, List<string> userRoles, int page, int pageSize)
    {
        // Access control: allow if admin, student, or the user themselves
        if (!userRoles.Contains(Roles.Admin) && !userRoles.Contains(Roles.Student) && currentUserId != userId)
        {
            throw new InvalidOperationException("You are not authorized to view these offers.");
        }

        var query = _context.ThesisOffers
            .Include(to => to.ThesisOfferStatus)
            .Where(to => to.TutorId == userId)
            .AsQueryable();

        var totalCount = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PaginatedResultBusinessLogicModel<ThesisOfferBusinessLogicModel>
        {
            Items = items.Select(ThesisOfferBusinessLogicMapper.ToBusinessModel).ToList(),
            TotalCount = totalCount
        };
    }
}
