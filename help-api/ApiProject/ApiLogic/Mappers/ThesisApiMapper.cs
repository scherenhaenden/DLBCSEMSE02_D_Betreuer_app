using ApiProject.ApiLogic.Models;
using ApiProject.BusinessLogic.Models;

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

        public async Task<ThesisCreateRequestBusinessLogicModel> MapToCreateBusinessModel(CreateThesisApiRequest request, Guid ownerId)
        {
            string? fileName = null;
            string? contentType = null;
            byte[]? content = null;

            if (request.Document != null)
            {
                fileName = request.Document.FileName;
                contentType = request.Document.ContentType;
                using var memoryStream = new MemoryStream();
                await request.Document.CopyToAsync(memoryStream);
                content = memoryStream.ToArray();
            }

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
            string? fileName = null;
            string? contentType = null;
            byte[]? content = null;

            if (request.Document != null)
            {
                fileName = request.Document.FileName;
                contentType = request.Document.ContentType;
                using var memoryStream = new MemoryStream();
                await request.Document.CopyToAsync(memoryStream);
                content = memoryStream.ToArray();
            }

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
