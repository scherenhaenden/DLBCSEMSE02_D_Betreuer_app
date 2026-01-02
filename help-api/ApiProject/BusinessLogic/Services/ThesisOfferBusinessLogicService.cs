using ApiProject.BusinessLogic.Mappers;
using ApiProject.BusinessLogic.Models;
using ApiProject.DatabaseAccess.Context;
using ApiProject.DatabaseAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiProject.BusinessLogic.Services;

public class ThesisOfferBusinessLogicService : IThesisOfferBusinessLogicService
{
    private readonly ThesisDbContext _context;

    public ThesisOfferBusinessLogicService(ThesisDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResultBusinessLogicModel<ThesisOfferBusinessLogicModel>> GetAllAsync(int page, int pageSize, Guid userId, List<string> userRoles)
    {
        var query = _context.ThesisOffers
            .Include(to => to.ThesisOfferStatus)
            .AsQueryable();

        if (!userRoles.Contains("ADMIN"))
        {
            if (userRoles.Contains("TUTOR"))
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

    public async Task<ThesisOfferBusinessLogicModel> CreateAsync(ThesisOfferCreateRequestBusinessLogicModel request)
    {
        var openStatus = await _context.ThesisOfferStatuses.SingleAsync(s => s.Name == "OPEN");

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

    public async Task<(ThesisOfferApplicationBusinessLogicModel?, string?)> CreateApplicationAsync(ThesisOfferApplicationCreateRequestBusinessLogicModel request)
    {
        var thesisOffer = await _context.ThesisOffers
            .Include(to => to.ThesisOfferStatus)
            .FirstOrDefaultAsync(to => to.Id == request.ThesisOfferId);

        if (thesisOffer == null)
        {
            return (null, "Thesis offer not found.");
        }

        if (thesisOffer.ThesisOfferStatus?.Name != "OPEN")
        {
            return (null, "Thesis offer does not have open state anymore.");
        }

        var pendingStatus = await _context.RequestStatuses.SingleAsync(s => s.Name == "PENDING");

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
}
