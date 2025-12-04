using ApiProject.Db.Context;
using ApiProject.Db.Entities;
using ApiProject.Logic.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiProject.Logic.Services;

/// <summary>
/// Implementierung des Topic-Service mit Datenbank-Speicher.
/// </summary>
public sealed class TopicService : ITopicService
{
    private readonly ThesisDbContext _context;
    private readonly IUserService _userService;

    /// <summary>
    /// Initialisiert eine neue Instanz des TopicService.
    /// </summary>
    /// <param name="context">Der Datenbank-Kontext.</param>
    /// <param name="userService">Der User-Service für Validierungen.</param>
    public TopicService(ThesisDbContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    /// <summary>
    /// Gibt alle Themen paginiert zurück.
    /// </summary>
    /// <param name="page">Die Seitennummer (1-basiert).</param>
    /// <param name="pageSize">Die Anzahl der Elemente pro Seite.</param>
    /// <returns>Ein paginiertes Ergebnis mit den Themen.</returns>
    public async Task<PaginatedResult<Topic>> GetAllAsync(int page, int pageSize)
    {
        var totalCount = await _context.Topics.CountAsync();
        var items = await _context.Topics
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<Topic>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    /// <summary>
    /// Gibt ein Thema anhand seiner ID zurück.
    /// </summary>
    /// <param name="id">Die GUID des Themas.</param>
    /// <returns>Das Thema oder null, wenn nicht gefunden.</returns>
    public async Task<Topic?> GetByIdAsync(Guid id)
    {
        return await _context.Topics.SingleOrDefaultAsync(t => t.Id == id);
    }

    /// <summary>
    /// Sucht Themen nach Titel oder Fachbereich.
    /// </summary>
    /// <param name="searchTerm">Der Suchbegriff.</param>
    /// <param name="page">Die Seitennummer (1-basiert).</param>
    /// <param name="pageSize">Die Anzahl der Elemente pro Seite.</param>
    /// <returns>Ein paginiertes Ergebnis mit den gefundenen Themen.</returns>
    public async Task<PaginatedResult<Topic>> SearchAsync(string searchTerm, int page, int pageSize)
    {
        var query = _context.Topics
            .Where(t => t.Title.Contains(searchTerm) || t.SubjectArea.Contains(searchTerm));

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<Topic>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    /// <summary>
    /// Erstellt ein neues Thema.
    /// </summary>
    /// <param name="title">Der Titel des Themas.</param>
    /// <param name="description">Die Beschreibung.</param>
    /// <param name="subjectArea">Der Fachbereich.</param>
    /// <param name="tutorId">Die ID des Tutors.</param>
    /// <returns>Das erstellte Thema.</returns>
    public async Task<Topic> CreateTopicAsync(string title, string description, string subjectArea, Guid tutorId)
    {
        // Validierung: Tutor muss Tutor sein
        if (!await _userService.UserHasRoleAsync(tutorId, "TUTOR"))
        {
            throw new InvalidOperationException("Tutor must have role TUTOR.");
        }

        var topic = new Topic
        {
            Title = title.Trim(),
            Description = description.Trim(),
            SubjectArea = subjectArea.Trim(),
            TutorId = tutorId,
            IsActive = true
        };

        _context.Topics.Add(topic);
        await _context.SaveChangesAsync();
        return topic;
    }

    /// <summary>
    /// Aktualisiert ein bestehendes Thema.
    /// </summary>
    /// <param name="id">Die GUID des zu aktualisierenden Themas.</param>
    /// <param name="title">Der neue Titel.</param>
    /// <param name="description">Die neue Beschreibung.</param>
    /// <param name="subjectArea">Der neue Fachbereich.</param>
    /// <param name="isActive">Ob das Thema aktiv ist.</param>
    /// <returns>Das aktualisierte Thema.</returns>
    public async Task<Topic> UpdateTopicAsync(Guid id, string? title, string? description, string? subjectArea, bool? isActive)
    {
        var topic = await _context.Topics.SingleOrDefaultAsync(t => t.Id == id);
        if (topic is null)
        {
            throw new KeyNotFoundException("Topic not found.");
        }

        if (title is not null)
        {
            topic.Title = title.Trim();
        }
        if (description is not null)
        {
            topic.Description = description.Trim();
        }
        if (subjectArea is not null)
        {
            topic.SubjectArea = subjectArea.Trim();
        }
        if (isActive.HasValue)
        {
            topic.IsActive = isActive.Value;
        }

        await _context.SaveChangesAsync();
        return topic;
    }

    /// <summary>
    /// Löscht ein Thema anhand seiner ID.
    /// </summary>
    /// <param name="id">Die GUID des zu löschenden Themas.</param>
    /// <returns>True, wenn das Thema gelöscht wurde; sonst false.</returns>
    public async Task<bool> DeleteTopicAsync(Guid id)
    {
        var topic = await _context.Topics.SingleOrDefaultAsync(t => t.Id == id);
        if (topic is null)
        {
            return false;
        }

        _context.Topics.Remove(topic);
        await _context.SaveChangesAsync();
        return true;
    }
}
