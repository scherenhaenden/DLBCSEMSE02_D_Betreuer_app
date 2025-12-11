using ApiProject.BusinessLogic.Mappers;
using ApiProject.BusinessLogic.Models;
using ApiProject.DatabaseAccess.Context;
using ApiProject.DatabaseAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiProject.BusinessLogic.Services
{
    /// <summary>
    /// Implementation of the Thesis Service using a database context.
    /// </summary>
    public sealed class ThesisService : IThesisService
    {
        private readonly ThesisDbContext _context;
        private readonly IUserBusinessLogicService _userBusinessLogicService;

        public ThesisService(ThesisDbContext context, IUserBusinessLogicService userBusinessLogicService)
        {
            _context = context;
            _userBusinessLogicService = userBusinessLogicService;
        }

        public async Task<PaginatedResultBusinessLogicModel<ThesisBusinessLogicModel>> GetAllAsync(int page, int pageSize)
        {
            var query = _context.Theses
                .Include(t => t.Status)
                .Include(t => t.BillingStatus)
                .Include(t => t.Document);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResultBusinessLogicModel<ThesisBusinessLogicModel>
            {
                Items = items.Select(ThesisBusinessLogicMapper.ToBusinessModel).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<ThesisBusinessLogicModel?> GetByIdAsync(Guid id)
        {
            var thesis = await _context.Theses
                .Include(t => t.Status)
                .Include(t => t.BillingStatus)
                .Include(t => t.Document)
                .SingleOrDefaultAsync(t => t.Id == id);

            return ThesisBusinessLogicMapper.ToBusinessModel(thesis);
        }

        public async Task<ThesisBusinessLogicModel> CreateThesisAsync(ThesisCreateRequestBusinessLogicModel request)
        {
            if (!await _userBusinessLogicService.UserHasRoleAsync(request.OwnerId, "STUDENT"))
            {
                throw new InvalidOperationException("Owner must have the STUDENT role.");
            }
            if (!await _userBusinessLogicService.UserHasRoleAsync(request.TutorId, "TUTOR"))
            {
                throw new InvalidOperationException("Tutor must have the TUTOR role.");
            }
            if (request.SecondSupervisorId.HasValue && !await _userBusinessLogicService.UserHasRoleAsync(request.SecondSupervisorId.Value, "TUTOR"))
            {
                throw new InvalidOperationException("Second supervisor must have the TUTOR role.");
            }

            var initialStatus = await _context.ThesisStatuses.FirstAsync(s => s.Name == "PendingApproval");
            var initialBillingStatus = await _context.BillingStatuses.FirstAsync(b => b.Name == "None");

            var thesis = new ThesisDataAccessModel
            {
                Title = request.Title.Trim(),
                SubjectArea = request.SubjectArea.Trim(),
                OwnerId = request.OwnerId,
                TutorId = request.TutorId,
                SecondSupervisorId = request.SecondSupervisorId,
                TopicId = request.TopicId,
                StatusId = initialStatus.Id,
                BillingStatusId = initialBillingStatus.Id
            };

            _context.Theses.Add(thesis);
            await _context.SaveChangesAsync();

            var createdThesis = await GetByIdAsync(thesis.Id);
            return createdThesis!;
        }

        public async Task<ThesisBusinessLogicModel> UpdateThesisAsync(Guid id, ThesisUpdateRequestBusinessLogicModel request)
        {
            var thesis = await _context.Theses.SingleOrDefaultAsync(t => t.Id == id);
            if (thesis == null)
            {
                throw new KeyNotFoundException("Thesis not found.");
            }

            if (request.TutorId.HasValue && !await _userBusinessLogicService.UserHasRoleAsync(request.TutorId.Value, "TUTOR"))
            {
                throw new InvalidOperationException("Tutor must have the TUTOR role.");
            }
            if (request.SecondSupervisorId.HasValue && !await _userBusinessLogicService.UserHasRoleAsync(request.SecondSupervisorId.Value, "TUTOR"))
            {
                throw new InvalidOperationException("Second supervisor must have the TUTOR role.");
            }

            if (request.Title != null) thesis.Title = request.Title.Trim();
            if (request.SubjectArea != null) thesis.SubjectArea = request.SubjectArea.Trim();
            // TODO: fix this too
            //if (request.StatusId.HasValue) thesis.StatusId = request.StatusId.Value;
            //if (request.BillingStatusId.HasValue) thesis.BillingStatusId = request.BillingStatusId.Value;
            if (request.TutorId.HasValue) thesis.TutorId = request.TutorId.Value;
            if (request.SecondSupervisorId.HasValue) thesis.SecondSupervisorId = request.SecondSupervisorId.Value;
            if (request.TopicId.HasValue) thesis.TopicId = request.TopicId.Value;

            if (request.StatusName != null)
            {
                var status = await _context.ThesisStatuses.SingleOrDefaultAsync(s => s.Name == request.StatusName);
                if (status != null)
                {
                    thesis.StatusId = status.Id;
                }
            }
            if (request.BillingStatusName != null)
            {
                var billingStatus = await _context.BillingStatuses.SingleOrDefaultAsync(b => b.Name == request.BillingStatusName);
                if (billingStatus != null)
                {
                    thesis.BillingStatusId = billingStatus.Id;
                }
            }

            await _context.SaveChangesAsync();
            
            var updatedThesis = await GetByIdAsync(id);
            return updatedThesis!;
        }

        public async Task<bool> DeleteThesisAsync(Guid id)
        {
            var thesis = await _context.Theses.SingleOrDefaultAsync(t => t.Id == id);
            if (thesis == null)
            {
                return false;
            }

            _context.Theses.Remove(thesis);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
