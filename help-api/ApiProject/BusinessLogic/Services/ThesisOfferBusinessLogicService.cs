using ApiProject.BusinessLogic.Mappers;
using ApiProject.BusinessLogic.Models;
using ApiProject.DatabaseAccess.Context;
using Microsoft.EntityFrameworkCore;

namespace ApiProject.BusinessLogic.Services;

public class ThesisOfferBusinessLogicService : IThesisOfferBusinessLogicService
{
    private readonly ThesisDbContext _context;

    public ThesisOfferBusinessLogicService(ThesisDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResultBusinessLogicModel<ThesisOfferBusinessLogicModel>> GetAllAsync(int page, int pageSize)
    {
        var query = _context.ThesisOffers
            .Include(to => to.ThesisOfferStatus)
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
