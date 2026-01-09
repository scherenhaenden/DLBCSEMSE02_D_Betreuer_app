using ApiProject.BusinessLogic.Models;

namespace ApiProject.BusinessLogic.Services
{
    public interface IThesisDocumentBusinessLogicService
    {
        Task<ThesisDocumentBusinessLogicModel?> GetByThesisIdAsync(Guid thesisId, Guid userId, List<string> userRoles);
        Task<ThesisDocumentBusinessLogicModel> UpdateAsync(Guid thesisId, IFormFile document, Guid userId, List<string> userRoles);
        Task<bool> DeleteAsync(Guid thesisId, Guid userId, List<string> userRoles);
    }
}
