using ApiProject.ApiLogic.Mappers;
using ApiProject.ApiLogic.Models;
using ApiProject.BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiProject.ApiLogic.Controllers
{
    [ApiController]
    [Route("thesis-offers")]
    [Authorize]
    public class ThesisOfferController : ControllerBase
    {
        private readonly IThesisOfferBusinessLogicService _thesisOfferService;
        private readonly IThesisOfferApiMapper _thesisOfferApiMapper;

        public ThesisOfferController(IThesisOfferBusinessLogicService thesisOfferService, IThesisOfferApiMapper thesisOfferApiMapper)
        {
            _thesisOfferService = thesisOfferService;
            _thesisOfferApiMapper = thesisOfferApiMapper;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<ThesisOfferResponse>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _thesisOfferService.GetAllAsync(page, pageSize);

            var response = new PaginatedResponse<ThesisOfferResponse>
            {
                Items = result.Items.Select(_thesisOfferApiMapper.MapToResponse).ToList(),
                TotalCount = result.TotalCount,
                Page = page,
                PageSize = pageSize
            };
            return Ok(response);
        }
    }
}
