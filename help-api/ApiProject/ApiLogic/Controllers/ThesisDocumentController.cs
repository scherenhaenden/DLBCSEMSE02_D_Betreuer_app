using ApiProject.ApiLogic.Mappers;
using ApiProject.ApiLogic.Models;
using ApiProject.BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApiProject.ApiLogic.Controllers
{
    [ApiController]
    [Route("theses")]
    [Authorize]
    public sealed class ThesisDocumentController : ControllerBase
    {
        private readonly IThesisDocumentBusinessLogicService _thesisDocumentBusinessLogicService;
        private readonly IThesisDocumentApiMapper _thesisDocumentApiMapper;

        public ThesisDocumentController(IThesisDocumentBusinessLogicService thesisDocumentBusinessLogicService, IThesisDocumentApiMapper thesisDocumentApiMapper)
        {
            _thesisDocumentBusinessLogicService = thesisDocumentBusinessLogicService;
            _thesisDocumentApiMapper = thesisDocumentApiMapper;
        }

        [HttpGet("{thesisId}/document")]
        public async Task<ActionResult<ThesisDocumentResponse>> GetByThesisId(Guid thesisId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            var document = await _thesisDocumentBusinessLogicService.GetByThesisIdAsync(thesisId, userId, userRoles);
            if (document == null) return NotFound();
            return Ok(_thesisDocumentApiMapper.MapToResponse(document, userId));
        }

        [HttpGet("{thesisId}/document/download")]
        public async Task<IActionResult> DownloadByThesisId(Guid thesisId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            var document = await _thesisDocumentBusinessLogicService.GetByThesisIdAsync(thesisId, userId, userRoles);
            if (document == null) return NotFound();
            return File(fileContents: document.Content, contentType: document.ContentType, fileDownloadName: document.FileName);
        }

        [HttpPut("{thesisId}/document")]
        public async Task<ActionResult<ThesisDocumentResponse>> Update(Guid thesisId, [FromForm] UpdateThesisDocumentRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            try
            {
                var updated = await _thesisDocumentBusinessLogicService.UpdateAsync(thesisId, request.Document, userId, userRoles);
                return Ok(_thesisDocumentApiMapper.MapToResponse(updated, userId));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpDelete("{thesisId}/document")]
        public async Task<IActionResult> Delete(Guid thesisId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            var result = await _thesisDocumentBusinessLogicService.DeleteAsync(thesisId, userId, userRoles);
            return result ? NoContent() : NotFound();
        }
    }
}
