using ApiProject.BusinessLogic.Mappers;

using ApiProject.BusinessLogic.Models;
using ApiProject.DatabaseAccess.Context;
using ApiProject.DatabaseAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiProject.BusinessLogic.Services
{
    public sealed class SubjectAreaService : ISubjectAreaService
    {
        private readonly ThesisDbContext _context;
        private readonly IUserBusinessLogicService _userBusinessLogicService;

        public SubjectAreaService(ThesisDbContext context, IUserBusinessLogicService userBusinessLogicService)
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
            var topic = await _context.SubjectAreas
                .Include(t => t.UserToSubjectAreas)
                .SingleOrDefaultAsync(t => t.Id == id);

            return SubjectAreaBusinessLogicMapper.ToBusinessModel(topic);
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

        public async Task<SubjectAreaBusinessLogicModel> CreateTopicAsync(SubjectAreaCreateRequestBusinessLogicModel request)
        {
            foreach (var tutorId in request.TutorIds)
            {
                if (!await _userBusinessLogicService.UserHasRoleAsync(tutorId, "TUTOR"))
                {
                    throw new InvalidOperationException($"User with ID {tutorId} must have the TUTOR role.");
                }
            }

            var topic = new SubjectAreaDataAccessModel
            {
                Title = request.Title.Trim(),
                Description = request.Description.Trim(),
                IsActive = true
            };

            foreach (var tutorId in request.TutorIds)
            {
                topic.UserToSubjectAreas.Add(new UserToSubjectAreas { SubjectArea = topic, UserId = tutorId });
            }

            _context.SubjectAreas.Add(topic);
            await _context.SaveChangesAsync();

            var createdTopic = await GetByIdAsync(topic.Id);
            return createdTopic!;
        }

        public async Task<SubjectAreaBusinessLogicModel> UpdateTopicAsync(Guid id, SubjectAreaUpdateRequestBusinessLogicModel request)
        {
            var topic = await _context.SubjectAreas
                .Include(t => t.UserToSubjectAreas)
                .SingleOrDefaultAsync(t => t.Id == id);

            if (topic == null)
            {
                throw new KeyNotFoundException("Topic not found.");
            }

            if (request.Title != null) topic.Title = request.Title.Trim();
            if (request.Description != null) topic.Description = request.Description.Trim();
            if (request.IsActive.HasValue) topic.IsActive = request.IsActive.Value;

            if (request.TutorIds != null)
            {
                foreach (var tutorId in request.TutorIds)
                {
                    if (!await _userBusinessLogicService.UserHasRoleAsync(tutorId, "TUTOR"))
                    {
                        throw new InvalidOperationException($"User with ID {tutorId} must have the TUTOR role.");
                    }
                }
                
                topic.UserToSubjectAreas.Clear();
                foreach (var tutorId in request.TutorIds)
                {
                    topic.UserToSubjectAreas.Add(new UserToSubjectAreas { UserToSubjectAreaId = topic.Id, UserId = tutorId });
                }
            }

            await _context.SaveChangesAsync();
            
            var updatedTopic = await GetByIdAsync(id);
            return updatedTopic!;
        }

        public async Task<bool> DeleteTopicAsync(Guid id)
        {
            var topic = await _context.SubjectAreas.SingleOrDefaultAsync(t => t.Id == id);
            if (topic == null)
            {
                return false;
            }

            _context.SubjectAreas.Remove(topic);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
