using ApiProject.BusinessLogic.Mappers;
using ApiProject.BusinessLogic.Models;
using ApiProject.DatabaseAccess.Context;
using ApiProject.DatabaseAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
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
            if (!await _userBusinessLogicService.UserHasRoleAsync(request.OwnerId, Roles.Student))
            {
                throw new InvalidOperationException("Owner must have the STUDENT role.");
            }
            
            var initialStatus = await _context.ThesisStatuses.FirstAsync(s => s.Name == ThesisStatuses.Registered);
            var initialBillingStatus = await _context.BillingStatuses.FirstAsync(b => b.Name == BillingStatuses.None);

            var thesis = new ThesisDataAccessModel
            {
                Title = request.Title.Trim(),
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

            if (currentStatus.Name == ThesisStatuses.Registered && request.SubjectAreaId.HasValue)
            {
                throw new InvalidOperationException("Cannot change subject area after registration.");
            }

            if (request.Title != null) thesis.Title = request.Title.Trim();
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

        public async Task<bool> DeleteThesisAsync(Guid id)
        {
            var thesis = await _context.Theses.SingleOrDefaultAsync(t => t.Id == id);
            if (thesis == null) return false;

            _context.Theses.Remove(thesis);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
