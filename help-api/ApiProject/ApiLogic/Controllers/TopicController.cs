using ApiProject.ApiLogic.Models;
using ApiProject.BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;
using BL = ApiProject.BusinessLogic.Models;

namespace ApiProject.ApiLogic.Controllers
{
    [ApiController]
    [Route("topics")]
    public sealed class TopicController : ControllerBase
    {
        private readonly ITopicService _topicService;

        public TopicController(ITopicService topicService)
        {
            _topicService = topicService;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<TopicResponse>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _topicService.GetAllAsync(page, pageSize);
            var response = new PaginatedResponse<TopicResponse>
            {
                Items = result.Items.Select(MapToResponse).ToList(),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };
            return Ok(response);
        }
        
        [HttpGet("search")]
        public async Task<ActionResult<PaginatedResponse<TopicResponse>>> Search([FromQuery] string q, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _topicService.SearchAsync(q, page, pageSize);
            var response = new PaginatedResponse<TopicResponse>
            {
                Items = result.Items.Select(MapToResponse).ToList(),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TopicResponse>> GetById(Guid id)
        {
            var topic = await _topicService.GetByIdAsync(id);
            if (topic == null)
            {
                return NotFound();
            }
            return Ok(MapToResponse(topic));
        }

        [HttpPost]
        public async Task<ActionResult<TopicResponse>> Create([FromBody] CreateTopicRequest request)
        {
            try
            {
                var created = await _topicService.CreateTopicAsync(new BL.TopicCreateRequestBusinessLogicModel
                {
                    Title = request.Title,
                    Description = request.Description,
                    SubjectArea = request.SubjectArea,
                    TutorIds = request.TutorIds
                });
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToResponse(created));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TopicResponse>> Update(Guid id, [FromBody] UpdateTopicRequest request)
        {
            try
            {
                var updated = await _topicService.UpdateTopicAsync(id, new BL.TopicUpdateRequestBusinessLogicModel
                {
                    Title = request.Title,
                    Description = request.Description,
                    SubjectArea = request.SubjectArea,
                    IsActive = request.IsActive,
                    TutorIds = request.TutorIds
                });
                return Ok(MapToResponse(updated));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var deleted = await _topicService.DeleteTopicAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }

        private static TopicResponse MapToResponse(BL.TopicBusinessLogicModel topic)
        {
            return new TopicResponse
            {
                Id = topic.Id,
                Title = topic.Title,
                Description = topic.Description,
                SubjectArea = topic.SubjectArea,
                IsActive = topic.IsActive,
                TutorIds = topic.TutorIds
            };
        }
    }
}
