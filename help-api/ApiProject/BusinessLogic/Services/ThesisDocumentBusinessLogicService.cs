using ApiProject.BusinessLogic.Models;
using ApiProject.DatabaseAccess.Context;
using ApiProject.DatabaseAccess.Entities;
using ApiProject.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ApiProject.BusinessLogic.Services
{
    public sealed class ThesisDocumentBusinessLogicService : IThesisDocumentBusinessLogicService
    {
        private readonly ThesisDbContext _context;

        public ThesisDocumentBusinessLogicService(ThesisDbContext context)
        {
            _context = context;
        }

        public async Task<ThesisDocumentBusinessLogicModel?> GetByThesisIdAsync(Guid thesisId, Guid userId, List<string> userRoles)
        {
            var thesis = await _context.Theses
                .Include(t => t.Document)
                .FirstOrDefaultAsync(t => t.Id == thesisId);

            if (thesis == null || thesis.Document == null) return null;

            // Check permissions: admin, or owner of the thesis, or tutor/second supervisor assigned
            if (!userRoles.Contains(Roles.Admin) &&
                thesis.OwnerId != userId &&
                thesis.TutorId != userId && 
                thesis.SecondSupervisorId != userId)
            {
                return null; // Not authorized
            }

            return new ThesisDocumentBusinessLogicModel
            {
                Id = thesis.Document.Id,
                FileName = thesis.Document.FileName,
                ContentType = thesis.Document.ContentType,
                Content = thesis.Document.Content,
                ThesisId = thesis.Document.ThesisId
            };
        }

        public async Task<ThesisDocumentBusinessLogicModel> UpdateAsync(Guid thesisId, IFormFile document, Guid userId, List<string> userRoles)
        {
            var thesis = await _context.Theses
                .Include(t => t.Document)
                .FirstOrDefaultAsync(t => t.Id == thesisId);

            if (thesis == null) throw new KeyNotFoundException("Thesis not found");

            // Check permissions: only owner can update document
            if (!userRoles.Contains(Roles.Admin) && thesis.OwnerId != userId)
            {
                throw new UnauthorizedAccessException("Not authorized to update this document");
            }

            // Read document content
            using var memoryStream = new MemoryStream();
            await document.CopyToAsync(memoryStream);
            var content = memoryStream.ToArray();

            if (thesis.Document == null)
            {
                // Create new document
                var newDoc = new ThesisDocumentDataAccessModel
                {
                    Id = Guid.NewGuid(),
                    FileName = document.FileName,
                    ContentType = document.ContentType,
                    Content = content,
                    ThesisId = thesisId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.ThesisDocuments.Add(newDoc);
                await _context.SaveChangesAsync();

                return new ThesisDocumentBusinessLogicModel
                {
                    Id = newDoc.Id,
                    FileName = newDoc.FileName,
                    ContentType = newDoc.ContentType,
                    Content = newDoc.Content,
                    ThesisId = newDoc.ThesisId
                };
            }
            else
            {
                // Update existing document
                thesis.Document.FileName = document.FileName;
                thesis.Document.ContentType = document.ContentType;
                thesis.Document.Content = content;
                thesis.Document.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return new ThesisDocumentBusinessLogicModel
                {
                    Id = thesis.Document.Id,
                    FileName = thesis.Document.FileName,
                    ContentType = thesis.Document.ContentType,
                    Content = thesis.Document.Content,
                    ThesisId = thesis.Document.ThesisId
                };
            }
        }

        public async Task<bool> DeleteAsync(Guid thesisId, Guid userId, List<string> userRoles)
        {
            var thesis = await _context.Theses
                .Include(t => t.Document)
                .FirstOrDefaultAsync(t => t.Id == thesisId);

            if (thesis == null || thesis.Document == null) return false;

            // Check permissions: only owner can delete document
            if (!userRoles.Contains(Roles.Admin) && thesis.OwnerId != userId)
            {
                return false; // Not authorized
            }

            _context.ThesisDocuments.Remove(thesis.Document);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
