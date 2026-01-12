using ApiProject.ApiLogic.Models;
using ApiProject.BusinessLogic.Models;
using Microsoft.AspNetCore.Http;

namespace ApiProject.ApiLogic.Mappers
{
    public class ThesisApiMapper : IThesisApiMapper
    {
        public ThesisResponse MapToResponse(ThesisBusinessLogicModel thesis)
        {
            if (thesis == null) return null;

            return new ThesisResponse
            {
                Id = thesis.Id,
                Title = thesis.Title,

                Description = thesis.Description,

                Status = thesis.Status,
                BillingStatus = thesis.BillingStatus?.Name,
                OwnerId = thesis.OwnerId,
                TutorId = thesis.TutorId,
                SecondSupervisorId = thesis.SecondSupervisorId,
                SubjectAreaId = thesis.SubjectAreaId,
                DocumentFileName = thesis.DocumentFileName,
                DocumentId = thesis.DocumentId
            };
        }

        private async Task<DocumentData> MapDocumentAsync(IFormFile? document)
        {
            if (document is null)
            {
                return new DocumentData(null, null, null);
            }

            using var memoryStream = new MemoryStream();
            await document.CopyToAsync(memoryStream);

            return new DocumentData(
                document.FileName,
                document.ContentType,
                memoryStream.ToArray());
        }

        private sealed record DocumentData(
            string? FileName,
            string? ContentType,
            byte[]? Content);

        public async Task<ThesisCreateRequestBusinessLogicModel> MapToCreateBusinessModel(CreateThesisApiRequest request, Guid ownerId)
        {
            var document = await MapDocumentAsync(request.Document);
            string? fileName = document.FileName;
            string? contentType = document.ContentType;
            byte[]? content = document.Content;

            return new ThesisCreateRequestBusinessLogicModel
            {
                Title = request.Title,
                Description = request.Description,
                OwnerId = ownerId,
                SubjectAreaId = request.SubjectAreaId,
                DocumentFileName = fileName,
                DocumentContentType = contentType,
                DocumentContent = content
            };
        }

        public async Task<ThesisUpdateRequestBusinessLogicModel> MapToUpdateBusinessModel(UpdateThesisRequest request)
        {
            var document = await MapDocumentAsync(request.Document);
            string? fileName = document.FileName;
            string? contentType = document.ContentType;
            byte[]? content = document.Content;

            return new ThesisUpdateRequestBusinessLogicModel
            {
                Title = request.Title,
                Description = request.Description,
                SubjectAreaId = request.SubjectAreaId,
                DocumentFileName = fileName,
                DocumentContentType = contentType,
                DocumentContent = content
            };
        }
    }
}
