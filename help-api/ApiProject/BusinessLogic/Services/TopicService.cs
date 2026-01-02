using ApiProject.BusinessLogic.Mappers;

using ApiProject.BusinessLogic.Models;
using ApiProject.DatabaseAccess.Context;
using ApiProject.DatabaseAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiProject.BusinessLogic.Services
{
    public sealed class TopicService : ITopicService
    {
        private readonly ThesisDbContext _context;
        private readonly IUserBusinessLogicService _userBusinessLogicService;

        public TopicService(ThesisDbContext context, IUserBusinessLogicService userBusinessLogicService)
        {
            _context = context;
            _userBusinessLogicService = userBusinessLogicService;
        }

        public async Task<PaginatedResultBusinessLogicModel<TopicBusinessLogicModel>> GetAllAsync(int page, int pageSize)
        {
            var query = _context.Topics
                .Include(t => t.UserToSubjectAreas);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResultBusinessLogicModel<TopicBusinessLogicModel>
            {
                Items = items.Select(TopicBusinessLogicMapper.ToBusinessModel).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<TopicBusinessLogicModel?> GetByIdAsync(Guid id)
        {
            var topic = await _context.Topics
                .Include(t => t.UserToSubjectAreas)
                .SingleOrDefaultAsync(t => t.Id == id);

            return TopicBusinessLogicMapper.ToBusinessModel(topic);
        }

        public async Task<PaginatedResultBusinessLogicModel<TopicBusinessLogicModel>> SearchAsync(string searchTerm, int page, int pageSize)
        {
            var query = _context.Topics
                .Include(t => t.UserToSubjectAreas)
                .Where(t => t.Title.Contains(searchTerm) );

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResultBusinessLogicModel<TopicBusinessLogicModel>
            {
                Items = items.Select(TopicBusinessLogicMapper.ToBusinessModel).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<TopicBusinessLogicModel> CreateTopicAsync(TopicCreateRequestBusinessLogicModel request)
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

            _context.Topics.Add(topic);
            await _context.SaveChangesAsync();

            var createdTopic = await GetByIdAsync(topic.Id);
            return createdTopic!;
        }

        public async Task<TopicBusinessLogicModel> UpdateTopicAsync(Guid id, TopicUpdateRequestBusinessLogicModel request)
        {
            var topic = await _context.Topics
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
            var topic = await _context.Topics.SingleOrDefaultAsync(t => t.Id == id);
            if (topic == null)
            {
                return false;
            }

            _context.Topics.Remove(topic);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
