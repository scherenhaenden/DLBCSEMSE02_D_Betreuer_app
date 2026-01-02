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

        public async Task<SubjectAreaBusinessLogicModel?> GetByIdAsync(Guid id)
        {
            var subjectArea = await _context.SubjectAreas
                .Include(t => t.UserToSubjectAreas)
                .SingleOrDefaultAsync(t => t.Id == id);

            return SubjectAreaBusinessLogicMapper.ToBusinessModel(subjectArea);
        }

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
