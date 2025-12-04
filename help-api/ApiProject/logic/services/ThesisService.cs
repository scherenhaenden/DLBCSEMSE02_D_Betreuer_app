using ApiProject.Db.Context;
using ApiProject.Db.Entities;
using ApiProject.Logic.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiProject.Logic.Services;

/// <summary>
/// Implementierung des Thesis-Service mit Datenbank-Speicher.
/// </summary>
public sealed class ThesisService : IThesisService
{
    private readonly ThesisDbContext _context;
    private readonly IUserService _userService;

    /// <summary>
    /// Initialisiert eine neue Instanz des ThesisService.
    /// </summary>
    /// <param name="context">Der Datenbank-Kontext.</param>
    /// <param name="userService">Der User-Service für Validierungen.</param>
    public ThesisService(ThesisDbContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    /// <summary>
    /// Gibt alle Thesen paginiert zurück.
    /// </summary>
    /// <param name="page">Die Seitennummer (1-basiert).</param>
    /// <param name="pageSize">Die Anzahl der Elemente pro Seite.</param>
    /// <returns>Ein paginiertes Ergebnis mit den Thesen.</returns>
    public async Task<PaginatedResult<Thesis>> GetAllAsync(int page, int pageSize)
    {
        var totalCount = await _context.Theses.CountAsync();
        var items = await _context.Theses
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<Thesis>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    /// <summary>
    /// Gibt eine These anhand ihrer ID zurück.
    /// </summary>
    /// <param name="id">Die GUID der These.</param>
    /// <returns>Die These oder null, wenn nicht gefunden.</returns>
    public async Task<Thesis?> GetByIdAsync(Guid id)
    {
        return await _context.Theses.SingleOrDefaultAsync(t => t.Id == id);
    }

    /// <summary>
    /// Erstellt eine neue These basierend auf der Anfrage.
    /// </summary>
    /// <param name="request">Die Anfrage mit den Details der neuen These.</param>
    /// <returns>Die erstellte These.</returns>
    /// <exception cref="InvalidOperationException">Wird ausgelöst, wenn Validierungen fehlschlagen.</exception>
    public async Task<Thesis> CreateThesisAsync(ThesisCreateRequest request)
    {
        // Validierung: Besitzer muss Student sein
        if (!await _userService.UserHasRoleAsync(request.OwnerId, "STUDENT"))
        {
            throw new InvalidOperationException("Owner must have role STUDENT.");
        }

        // Validierung: Tutor muss Tutor sein
        if (!await _userService.UserHasRoleAsync(request.TutorId, "TUTOR"))
        {
            throw new InvalidOperationException("Tutor must have role TUTOR.");
        }

        // Validierung: Zweiter Betreuer muss Tutor sein, falls angegeben
        if (request.SecondSupervisorId.HasValue &&
            !await _userService.UserHasRoleAsync(request.SecondSupervisorId.Value, "TUTOR"))
        {
            throw new InvalidOperationException(
                "Second supervisor must have role TUTOR when defined.");
        }

        // Neue These erstellen und initialisieren
        var thesis = new Thesis
        {
            Title              = request.Title.Trim(),
            OwnerId            = request.OwnerId,
            TutorId            = request.TutorId,
            SecondSupervisorId = request.SecondSupervisorId,
            TopicId            = request.TopicId,
            ProgressPercent    = request.ProgressPercent,
            ExposePath         = request.ExposePath,
            BillingStatus      = request.BillingStatus,
            Status             = ThesisStatus.Draft // Standardstatus
        };

        // Zur Datenbank hinzufügen
        _context.Theses.Add(thesis);
        await _context.SaveChangesAsync();
        return thesis;
    }

    /// <summary>
    /// Aktualisiert eine bestehende These.
    /// </summary>
    /// <param name="id">Die GUID der zu aktualisierenden These.</param>
    /// <param name="request">Die Anfrage mit den zu aktualisierenden Feldern.</param>
    /// <returns>Die aktualisierte These.</returns>
    /// <exception cref="KeyNotFoundException">Wird ausgelöst, wenn die These nicht gefunden wird.</exception>
    /// <exception cref="InvalidOperationException">Wird ausgelöst, wenn Validierungen fehlschlagen.</exception>
    public async Task<Thesis> UpdateThesisAsync(Guid id, ThesisUpdateRequest request)
    {
        // These finden
        var thesis = await _context.Theses.SingleOrDefaultAsync(t => t.Id == id);
        if (thesis is null)
        {
            throw new KeyNotFoundException("Thesis not found.");
        }

        // Validierung: Tutor muss Tutor sein, falls aktualisiert
        if (request.TutorId.HasValue && !await _userService.UserHasRoleAsync(request.TutorId.Value, "TUTOR"))
        {
            throw new InvalidOperationException("Tutor must have role TUTOR.");
        }

        // Validierung: Zweiter Betreuer muss Tutor sein, falls aktualisiert
        if (request.SecondSupervisorId.HasValue &&
            !await _userService.UserHasRoleAsync(request.SecondSupervisorId.Value, "TUTOR"))
        {
            throw new InvalidOperationException("Second supervisor must have role TUTOR.");
        }

        // Felder aktualisieren, falls angegeben
        if (request.Title is not null)
        {
            thesis.Title = request.Title.Trim();
        }
        if (request.Status.HasValue)
        {
            thesis.Status = request.Status.Value;
        }
        if (request.ProgressPercent.HasValue)
        {
            thesis.ProgressPercent = request.ProgressPercent.Value;
        }
        if (request.ExposePath is not null)
        {
            thesis.ExposePath = request.ExposePath;
        }
        if (request.BillingStatus.HasValue)
        {
            thesis.BillingStatus = request.BillingStatus.Value;
        }
        if (request.TutorId.HasValue)
        {
            thesis.TutorId = request.TutorId.Value;
        }
        if (request.SecondSupervisorId.HasValue)
        {
            thesis.SecondSupervisorId = request.SecondSupervisorId.Value;
        }
        if (request.TopicId.HasValue)
        {
            thesis.TopicId = request.TopicId.Value;
        }

        await _context.SaveChangesAsync();
        return thesis;
    }

    /// <summary>
    /// Löscht eine These anhand ihrer ID.
    /// </summary>
    /// <param name="id">Die GUID der zu löschenden These.</param>
    /// <returns>True, wenn die These gelöscht wurde; sonst false.</returns>
    public async Task<bool> DeleteThesisAsync(Guid id)
    {
        // These finden
        var thesis = await _context.Theses.SingleOrDefaultAsync(t => t.Id == id);
        if (thesis is null)
        {
            return false;
        }
        // Aus Datenbank entfernen
        _context.Theses.Remove(thesis);
        await _context.SaveChangesAsync();
        return true;
    }
}
