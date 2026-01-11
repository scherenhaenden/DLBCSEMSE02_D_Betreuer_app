using ApiProject.BusinessLogic.Mappers;
using ApiProject.BusinessLogic.Models;
using ApiProject.DatabaseAccess.Context;
using ApiProject.DatabaseAccess.Entities;
using Microsoft.EntityFrameworkCore;
using ApiProject.Constants;

namespace ApiProject.BusinessLogic.Services
{
    public sealed class ThesisBusinessLogicService : IThesisBusinessLogicService
    {
        private readonly ThesisDbContext _context;
        private readonly IUserBusinessLogicService _userBusinessLogicService;

        public ThesisBusinessLogicService(ThesisDbContext context, IUserBusinessLogicService userBusinessLogicService)
        {
            _context = context;
            _userBusinessLogicService = userBusinessLogicService;
        }

        /// <summary>
        /// Retrieves a paginated list of theses based on user roles and permissions.
        /// This method supports role-based access control, allowing admins to see all theses, tutors to see theses they supervise, and students to see their own theses.
        /// 
        /// What: Fetches theses with their status, billing status, and document information, filtered by user permissions.
        /// How: Builds a queryable starting from Theses table, includes related entities for eager loading.
        /// Applies role-based filters: admins see all, tutors see theses where they are primary or secondary supervisor, students see their owned theses, others see none.
        /// Counts total matching theses for pagination, skips and takes the appropriate page, maps to business models.
        /// Why: Essential for thesis management interfaces, ensuring users only access theses relevant to them.
        /// Supports secure data access and efficient pagination for large datasets.
        /// </summary>
        /// <param name="page">The page number to retrieve (1-based).</param>
        /// <param name="pageSize">The number of theses per page.</param>
        /// <param name="userId">The ID of the user making the request.</param>
        /// <param name="userRoles">The list of roles assigned to the user.</param>
        /// <returns>A paginated result containing the list of theses and metadata.</returns>
        public async Task<PaginatedResultBusinessLogicModel<ThesisBusinessLogicModel>> GetAllAsync(int page, int pageSize, Guid userId, List<string> userRoles)
        {
            var query = _context.Theses
                .Include(t => t.Status)
                .Include(t => t.BillingStatus)
                .Include(t => t.Document)
                .AsQueryable();

            if (!userRoles.Contains(Roles.Admin))
            {
                if (userRoles.Contains(Roles.Tutor))
                {
                    query = query.Where(t => t.TutorId == userId || t.SecondSupervisorId == userId);
                }
                else if (userRoles.Contains(Roles.Student))
                {
                    query = query.Where(t => t.OwnerId == userId);
                }
                else
                {
                    query = query.Where(t => false);
                }
            }

            var totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PaginatedResultBusinessLogicModel<ThesisBusinessLogicModel>
            {
                Items = items.Select(ThesisBusinessLogicMapper.ToBusinessModel).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// Retrieves a single thesis by its unique identifier, including related status, billing status, and document information.
        /// 
        /// What: Fetches detailed information about a specific thesis from the database.
        /// How: Queries the Theses table with eager loading of Status, BillingStatus, and Document entities to include all necessary related data.
        /// Uses SingleOrDefaultAsync to find the thesis by ID, returning null if not found.
        /// Maps the data access model to the business model using ThesisBusinessLogicMapper.
        /// Why: Allows detailed viewing of thesis information for authorized users, such as owners, supervisors, or administrators.
        /// Ensures all related data is loaded efficiently to avoid additional database queries.
        /// </summary>
        /// <param name="id">The unique GUID identifier of the thesis.</param>
        /// <returns>The thesis business model if found, otherwise null.</returns>
        public async Task<ThesisBusinessLogicModel?> GetByIdAsync(Guid id)
        {
            var thesis = await _context.Theses
                .Include(t => t.Status)
                .Include(t => t.BillingStatus)
                .Include(t => t.Document)
                .SingleOrDefaultAsync(t => t.Id == id);

            return ThesisBusinessLogicMapper.ToBusinessModel(thesis);
        }

        /// <summary>
        /// Creates a new thesis with the specified details, ensuring the owner is a student and setting initial statuses.
        /// 
        /// What: Creates a new thesis record in the database with provided title, owner, subject area, and optional document.
        /// How: Validates that the owner has the STUDENT role using the user service.
        /// Retrieves initial statuses (Registered for thesis, None for billing) from the database.
        /// Creates a new ThesisDataAccessModel with trimmed title, assigned IDs, and null supervisors (assigned via requests).
        /// Saves the thesis, then optionally creates and saves a document if provided.
        /// Fetches and returns the created thesis with all related data populated.
        /// Why: Enables students to initiate thesis proposals in the system.
        /// Ensures data integrity by validating roles and setting appropriate initial states.
        /// Supports optional document attachment for initial submissions.
        /// </summary>
        /// <param name="request">The request model containing thesis creation details.</param>
        /// <returns>The created thesis business model with populated data.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the owner is not a student.</exception>
        public async Task<ThesisBusinessLogicModel> CreateThesisAsync(ThesisCreateRequestBusinessLogicModel request)
        {
            if (!await _userBusinessLogicService.UserHasRoleAsync(request.OwnerId, Roles.Student))
            {
                throw new InvalidOperationException("Owner must have the STUDENT role.");
            }
            
            var initialStatus = await _context.ThesisStatuses.FirstAsync(s => s.Name == ThesisStatuses.Registered);
            var initialBillingStatus = await _context.BillingStatuses.FirstAsync(b => b.Name == BillingStatuses.None);

            var thesis = new ThesisDataAccessModel
            {
                Title = request.Title.Trim(),

                Description = request.Description,

                OwnerId = request.OwnerId,
                SubjectAreaId = request.SubjectAreaId,
                StatusId = initialStatus.Id,
                BillingStatusId = initialBillingStatus.Id,
                SecondSupervisorId = null
            };

            _context.Theses.Add(thesis);
            await _context.SaveChangesAsync();

            if (request.DocumentContent != null)
            {
                var document = new ThesisDocumentDataAccessModel
                {
                    FileName = request.DocumentFileName!,
                    ContentType = request.DocumentContentType!,
                    Content = request.DocumentContent,
                    ThesisId = thesis.Id
                };
                _context.ThesisDocuments.Add(document);
                await _context.SaveChangesAsync();
            }

            var createdThesis = await GetByIdAsync(thesis.Id);
            
            return createdThesis!;
        }

        /// <summary>
        /// Updates an existing thesis with new details, enforcing status-based restrictions on modifications.
        /// 
        /// What: Modifies a thesis's title, subject area, or document based on the current thesis status.
        /// How: Retrieves the thesis by ID, throws if not found.
        /// Checks the current status: blocks updates if Submitted or Defended.
        /// For Registered status, blocks subject area changes.
        /// Applies updates conditionally: trims title, assigns subject area if allowed, handles document creation or update.
        /// Saves changes and returns the updated thesis with populated data.
        /// Why: Allows thesis owners to refine their work during early stages while preventing unauthorized changes later.
        /// Enforces business rules to maintain integrity of the thesis process.
        /// Supports document management for drafts and revisions.
        /// </summary>
        /// <param name="id">The unique GUID identifier of the thesis to update.</param>
        /// <param name="request">The request model containing update details.</param>
        /// <returns>The updated thesis business model with populated data.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the thesis is not found.</exception>
        /// <exception cref="InvalidOperationException">Thrown if updates are not allowed based on status.</exception>
        public async Task<ThesisBusinessLogicModel> UpdateThesisAsync(Guid id, ThesisUpdateRequestBusinessLogicModel request)
        {
            var thesis = await _context.Theses.SingleOrDefaultAsync(t => t.Id == id);
            if (thesis == null)
            {
                throw new KeyNotFoundException("Thesis not found.");
            }

            // Only allow updates in early stages
            var currentStatus = await _context.ThesisStatuses.FindAsync(thesis.StatusId);
            if (currentStatus.Name == ThesisStatuses.Submitted || currentStatus.Name == ThesisStatuses.Defended)
            {
                throw new InvalidOperationException("Thesis cannot be modified after submission or defense.");
            }

            if (currentStatus.Name != ThesisStatuses.Registered && request.SubjectAreaId.HasValue)
            {
                throw new InvalidOperationException("Can change subject only if not ins registration.");
            }

            if (request.Title != null) thesis.Title = request.Title.Trim();

            if (request.Description != null) thesis.Description = request.Description;

            if (request.SubjectAreaId.HasValue) thesis.SubjectAreaId = request.SubjectAreaId.Value;

            if (request.DocumentContent != null)
            {
                var existingDocument = await _context.ThesisDocuments.FirstOrDefaultAsync(d => d.ThesisId == id);
                if (existingDocument != null)
                {
                    // Update existing
                    existingDocument.FileName = request.DocumentFileName!;
                    existingDocument.ContentType = request.DocumentContentType!;
                    existingDocument.Content = request.DocumentContent;
                }
                else
                {
                    // Create new
                    var document = new ThesisDocumentDataAccessModel
                    {
                        FileName = request.DocumentFileName!,
                        ContentType = request.DocumentContentType!,
                        Content = request.DocumentContent,
                        ThesisId = id
                    };
                    _context.ThesisDocuments.Add(document);
                }
            }

            await _context.SaveChangesAsync();
            
            var updatedThesis = await GetByIdAsync(id);
            return updatedThesis!;
        }

        /// <summary>
        /// Deletes a thesis from the database by its unique identifier.
        /// 
        /// What: Removes a thesis record and its associated data from the system.
        /// How: Queries the Theses table to find the thesis by ID.
        /// If found, removes the thesis entity from the context and saves changes to the database.
        /// Returns true if deletion was successful, false if the thesis was not found.
        /// Why: Allows authorized users (typically administrators or thesis owners) to remove theses that are no longer needed.
        /// Supports data cleanup and management in the thesis system.
        /// Cascading deletes should be handled by the database or EF configuration for related entities.
        /// </summary>
        /// <param name="id">The unique GUID identifier of the thesis to delete.</param>
        /// <param name="userId">The unique identifier of the user attempting the deletion.</param>
        /// <param name="userRoles">The list of roles assigned to the user.</param>
        /// <returns>The result of the deletion attempt.</returns>
        public async Task<DeleteThesisResult> DeleteThesisAsync(Guid id, Guid userId, List<string> userRoles)
        {
            var thesis = await _context.Theses.SingleOrDefaultAsync(t => t.Id == id);
            if (thesis == null) return DeleteThesisResult.NotFound;

            // Check if user is admin
            if (!userRoles.Contains(Roles.Admin))
            {
                // If not admin, check if user is the owner
                if (thesis.OwnerId != userId)
                {
                    return DeleteThesisResult.NotAuthorized;
                }
            }

            _context.Theses.Remove(thesis);
            await _context.SaveChangesAsync();
            return DeleteThesisResult.Deleted;
        }

        /// <summary>
        /// Retrieves all billing statuses from the database.
        /// Billing statuses are used to track the payment state of theses.
        /// 
        /// What: Fetches all billing status records from the database.
        /// How: Queries the BillingStatuses table and maps to business logic models.
        /// Why: Provides a list of available billing statuses for UI dropdowns or validation.
        /// Ensures data consistency by retrieving from the database rather than hardcoding.
        /// </summary>
        /// <returns>A list of billing status business logic models.</returns>
        public async Task<List<BillingStatusBusinessLogicModel>> GetAllBillingStatusesAsync()
        {
            return await _context.BillingStatuses
                .Select(bs => new BillingStatusBusinessLogicModel
                {
                    Id = bs.Id,
                    Name = bs.Name
                })
                .ToListAsync();
        }
    }
}
