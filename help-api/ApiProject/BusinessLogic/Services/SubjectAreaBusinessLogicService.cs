using ApiProject.BusinessLogic.Mappers;

using ApiProject.BusinessLogic.Models;
using ApiProject.DatabaseAccess.Context;
using ApiProject.DatabaseAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiProject.BusinessLogic.Services
{
    public sealed class SubjectAreaBusinessLogicService : ISubjectAreaBusinessLogicService
    {
        private readonly ThesisDbContext _context;
        private readonly IUserBusinessLogicService _userBusinessLogicService;

        public SubjectAreaBusinessLogicService(ThesisDbContext context, IUserBusinessLogicService userBusinessLogicService)
        {
            _context = context;
            _userBusinessLogicService = userBusinessLogicService;
        }

        /// <summary>
        /// Retrieves a paginated list of subject areas from the database, including their associated tutors.
        /// This method supports pagination to handle large datasets efficiently.
        /// 
        /// What: Fetches subject areas with their UserToSubjectAreas (tutors) for eager loading.
        /// How: Builds a queryable starting from SubjectAreas table, includes UserToSubjectAreas.
        /// Counts total subject areas for pagination, skips and takes the appropriate page, maps to business models.
        /// Why: Essential for subject area management interfaces, ensuring tutors are loaded for display.
        /// Supports efficient pagination for large datasets.
        /// </summary>
        /// <param name="page">The page number to retrieve (1-based).</param>
        /// <param name="pageSize">The number of subject areas per page.</param>
        /// <returns>A paginated result containing the list of subject areas and metadata.</returns>
        public async Task<PaginatedResultBusinessLogicModel<SubjectAreaBusinessLogicModel>> GetAllAsync(int page, int pageSize)
        {
            var query = _context.SubjectAreas
                .Include(t => t.UserToSubjectAreas);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResultBusinessLogicModel<SubjectAreaBusinessLogicModel>
            {
                Items = items.Select(SubjectAreaBusinessLogicMapper.ToBusinessModel).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// Retrieves a single subject area by its unique identifier, including its associated tutors.
        /// 
        /// What: Fetches detailed information about a specific subject area from the database.
        /// How: Queries the SubjectAreas table with eager loading of UserToSubjectAreas entities to include tutor information.
        /// Uses SingleOrDefaultAsync to find the subject area by ID, returning null if not found.
        /// Maps the data access model to the business model using SubjectAreaBusinessLogicMapper.
        /// Why: Allows detailed viewing of subject area information for authorized users, such as administrators or tutors.
        /// Ensures associated tutors are loaded efficiently to avoid additional database queries.
        /// </summary>
        /// <param name="id">The unique GUID identifier of the subject area.</param>
        /// <returns>The subject area business model if found, otherwise null.</returns>
        public async Task<SubjectAreaBusinessLogicModel?> GetByIdAsync(Guid id)
        {
            var subjectArea = await _context.SubjectAreas
                .Include(t => t.UserToSubjectAreas)
                .SingleOrDefaultAsync(t => t.Id == id);

            return SubjectAreaBusinessLogicMapper.ToBusinessModel(subjectArea);
        }

        /// <summary>
        /// Searches for subject areas by title, returning a paginated list including associated tutors.
        /// 
        /// What: Fetches subject areas whose titles contain the search term, with their UserToSubjectAreas (tutors).
        /// How: Builds a queryable from SubjectAreas table, includes UserToSubjectAreas, filters by title containing searchTerm.
        /// Counts total matching subject areas for pagination, skips and takes the appropriate page, maps to business models.
        /// Why: Enables users to find specific subject areas by name in management interfaces.
        /// Supports partial matching for user-friendly search functionality.
        /// Maintains pagination for performance with large result sets.
        /// </summary>
        /// <param name="searchTerm">The term to search for in subject area titles.</param>
        /// <param name="page">The page number to retrieve (1-based).</param>
        /// <param name="pageSize">The number of subject areas per page.</param>
        /// <returns>A paginated result containing the matching subject areas and metadata.</returns>
        public async Task<PaginatedResultBusinessLogicModel<SubjectAreaBusinessLogicModel>> SearchAsync(string searchTerm, int page, int pageSize)
        {
            var query = _context.SubjectAreas
                .Include(t => t.UserToSubjectAreas)
                .Where(t => t.Title.Contains(searchTerm) );

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResultBusinessLogicModel<SubjectAreaBusinessLogicModel>
            {
                Items = items.Select(SubjectAreaBusinessLogicMapper.ToBusinessModel).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// Creates a new subject area with the specified details, validating tutors and setting it as active.
        /// 
        /// What: Creates a new subject area record in the database with provided title, description, and assigned tutors.
        /// How: Validates that the title is not empty, checks each tutor has TUTOR role using user service.
        /// Creates a new SubjectAreaDataAccessModel with trimmed title/description, sets IsActive to true.
        /// Adds UserToSubjectAreas for each tutor, saves to database, then fetches and returns the created subject area with tutors populated.
        /// Why: Enables administrators to define new subject areas and assign qualified tutors.
        /// Ensures data integrity with role validation and uniqueness constraints.
        /// Supports multi-tutor assignment for flexible subject area coverage.
        /// </summary>
        /// <param name="request">The request model containing subject area creation details.</param>
        /// <returns>The created subject area business model with tutors populated.</returns>
        /// <exception cref="ArgumentException">Thrown if the title is null, empty, or whitespace.</exception>
        /// <exception cref="InvalidOperationException">Thrown if any specified user does not have the TUTOR role.</exception>
        public async Task<SubjectAreaBusinessLogicModel> CreateSubjectAreaAsync(SubjectAreaCreateRequestBusinessLogicModel request)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                throw new ArgumentException("Title cannot be empty.", nameof(request.Title));
            }

            foreach (var tutorId in request.TutorIds)
            {
                if (!await _userBusinessLogicService.UserHasRoleAsync(tutorId, "TUTOR"))
                {
                    throw new InvalidOperationException($"User with ID {tutorId} must have the TUTOR role.");
                }
            }

            var subjectArea = new SubjectAreaDataAccessModel
            {
                Title = request.Title.Trim(),
                Description = request.Description.Trim(),
                IsActive = true
            };

            foreach (var tutorId in request.TutorIds)
            {
                subjectArea.UserToSubjectAreas.Add(new UserToSubjectAreas { SubjectArea = subjectArea, UserId = tutorId });
            }

            _context.SubjectAreas.Add(subjectArea);
            await _context.SaveChangesAsync();

            var createdSubjectArea = await GetByIdAsync(subjectArea.Id);
            return createdSubjectArea!;
        }

        /// <summary>
        /// Updates an existing subject area with new details, validating tutors and preserving data integrity.
        /// 
        /// What: Modifies a subject area's title, description, active status, and tutor assignments based on the request.
        /// How: Retrieves the subject area by ID with UserToSubjectAreas included, throws if not found.
        /// Validates title is not empty if provided, applies updates conditionally (trimming strings).
        /// If tutor IDs provided, validates each has TUTOR role, clears existing assignments, adds new ones.
        /// Saves changes, then fetches and returns the updated subject area with tutors populated.
        /// Why: Allows administrators to modify subject area details and reassign tutors as needed.
        /// Ensures data consistency with validation checks and proper relationship management.
        /// Supports partial updates for flexibility in editing operations.
        /// </summary>
        /// <param name="id">The unique GUID identifier of the subject area to update.</param>
        /// <param name="request">The request model containing update details.</param>
        /// <returns>The updated subject area business model with tutors populated.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the subject area is not found.</exception>
        /// <exception cref="ArgumentException">Thrown if the title is provided but is null, empty, or whitespace.</exception>
        /// <exception cref="InvalidOperationException">Thrown if any specified user does not have the TUTOR role.</exception>
        public async Task<SubjectAreaBusinessLogicModel> UpdateSubjectAreaAsync(Guid id, SubjectAreaUpdateRequestBusinessLogicModel request)
        {
            var subjectArea = await _context.SubjectAreas
                .Include(t => t.UserToSubjectAreas)
                .SingleOrDefaultAsync(t => t.Id == id);

            if (subjectArea == null)
            {
                throw new KeyNotFoundException("Subject Area not found.");
            }

            if (request.Title != null && string.IsNullOrWhiteSpace(request.Title))
            {
                throw new ArgumentException("Title cannot be empty.", nameof(request.Title));
            }

            if (request.Title != null) subjectArea.Title = request.Title.Trim();
            if (request.Description != null) subjectArea.Description = request.Description.Trim();
            if (request.IsActive.HasValue) subjectArea.IsActive = request.IsActive.Value;

            if (request.TutorIds != null)
            {
                foreach (var tutorId in request.TutorIds)
                {
                    if (!await _userBusinessLogicService.UserHasRoleAsync(tutorId, "TUTOR"))
                    {
                        throw new InvalidOperationException($"User with ID {tutorId} must have the TUTOR role.");
                    }
                }
                
                subjectArea.UserToSubjectAreas.Clear();
                foreach (var tutorId in request.TutorIds)
                {
                    subjectArea.UserToSubjectAreas.Add(new UserToSubjectAreas { SubjectAreaId = subjectArea.Id, UserId = tutorId });
                }
            }

            await _context.SaveChangesAsync();
            
            var updatedSubjectArea = await GetByIdAsync(id);
            return updatedSubjectArea!;
        }

        /// <summary>
        /// Deletes a subject area from the database by its unique identifier.
        /// 
        /// What: Removes a subject area record and its associated data from the system.
        /// How: Queries the SubjectAreas table to find the subject area by ID.
        /// If found, removes the subject area entity from the context and saves changes to the database.
        /// Returns true if deletion was successful, false if the subject area was not found.
        /// Why: Allows authorized users (typically administrators) to remove subject areas that are no longer needed.
        /// Supports data cleanup and management in the thesis system.
        /// Cascading deletes should be handled by the database or EF configuration for related entities.
        /// </summary>
        /// <param name="id">The unique GUID identifier of the subject area to delete.</param>
        /// <returns>True if the subject area was deleted, false if it was not found.</returns>
        public async Task<bool> DeleteSubjectAreaAsync(Guid id)
        {
            var subjectArea = await _context.SubjectAreas.SingleOrDefaultAsync(t => t.Id == id);
            if (subjectArea == null)
            {
                return false;
            }

            _context.SubjectAreas.Remove(subjectArea);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
