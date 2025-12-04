using ApiProject.Db;

namespace ApiProject.Logic;

/// <summary>
/// Implementierung des Thesis-Service mit In-Memory-Speicher.
/// </summary>
public sealed class ThesisService : IThesisService
{
    private readonly IUserService _userService;
    private readonly List<Thesis> _theses = new();

    /// <summary>
    /// Initialisiert eine neue Instanz des ThesisService.
    /// </summary>
    /// <param name="userService">Der User-Service für Validierungen.</param>
    public ThesisService(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Gibt alle Thesen zurück.
    /// </summary>
    /// <returns>Eine schreibgeschützte Sammlung aller Thesen.</returns>
    public IReadOnlyCollection<Thesis> GetAll()
    {
        return _theses.AsReadOnly();
    }

    /// <summary>
    /// Gibt eine These anhand ihrer ID zurück.
    /// </summary>
    /// <param name="id">Die GUID der These.</param>
    /// <returns>Die These oder null, wenn nicht gefunden.</returns>
    public Thesis? GetById(Guid id)
    {
        return _theses.SingleOrDefault(t => t.Id == id);
    }

    /// <summary>
    /// Erstellt eine neue These basierend auf der Anfrage.
    /// </summary>
    /// <param name="request">Die Anfrage mit den Details der neuen These.</param>
    /// <returns>Die erstellte These.</returns>
    /// <exception cref="InvalidOperationException">Wird ausgelöst, wenn Validierungen fehlschlagen.</exception>
    public Thesis CreateThesis(ThesisCreateRequest request)
    {
        // Validierung: Besitzer muss Student sein
        if (!_userService.UserHasRole(request.OwnerId, "STUDENT"))
        {
            throw new InvalidOperationException("Owner must have role STUDENT.");
        }

        // Validierung: Tutor muss Tutor sein
        if (!_userService.UserHasRole(request.TutorId, "TUTOR"))
        {
            throw new InvalidOperationException("Tutor must have role TUTOR.");
        }

        // Validierung: Zweiter Betreuer muss Tutor sein, falls angegeben
        if (request.SecondSupervisorId.HasValue &&
            !_userService.UserHasRole(request.SecondSupervisorId.Value, "TUTOR"))
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

        // Zur Liste hinzufügen
        _theses.Add(thesis);
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
    public Thesis UpdateThesis(Guid id, ThesisUpdateRequest request)
    {
        // These finden
        var thesis = GetById(id);
        if (thesis is null)
        {
            throw new KeyNotFoundException("Thesis not found.");
        }

        // Validierung: Tutor muss Tutor sein, falls aktualisiert
        if (request.TutorId.HasValue && !_userService.UserHasRole(request.TutorId.Value, "TUTOR"))
        {
            throw new InvalidOperationException("Tutor must have role TUTOR.");
        }

        // Validierung: Zweiter Betreuer muss Tutor sein, falls aktualisiert
        if (request.SecondSupervisorId.HasValue &&
            !_userService.UserHasRole(request.SecondSupervisorId.Value, "TUTOR"))
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

        return thesis;
    }

    /// <summary>
    /// Löscht eine These anhand ihrer ID.
    /// </summary>
    /// <param name="id">Die GUID der zu löschenden These.</param>
    /// <returns>True, wenn die These gelöscht wurde; sonst false.</returns>
    public bool DeleteThesis(Guid id)
    {
        // These finden
        var thesis = GetById(id);
        if (thesis is null)
        {
            return false;
        }
        // Aus Liste entfernen
        _theses.Remove(thesis);
        return true;
    }
}
