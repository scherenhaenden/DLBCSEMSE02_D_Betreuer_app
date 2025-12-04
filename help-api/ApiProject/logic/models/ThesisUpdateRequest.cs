using ApiProject.Db.Entities;

namespace ApiProject.Logic.Models;

/// <summary>
/// Anfrage-Klasse für die Aktualisierung einer These.
/// </summary>
public sealed class ThesisUpdateRequest
{
    /// <summary>
    /// Der neue Titel (optional).
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Der neue Status (optional).
    /// </summary>
    public ThesisStatus? Status { get; set; }

    /// <summary>
    /// Der neue Fortschritt in Prozent (optional).
    /// </summary>
    public int? ProgressPercent { get; set; }

    /// <summary>
    /// Der neue Pfad zum Exposé (optional).
    /// </summary>
    public string? ExposePath { get; set; }

    /// <summary>
    /// Der neue Abrechnungsstatus (optional).
    /// </summary>
    public BillingStatus? BillingStatus { get; set; }

    /// <summary>
    /// Die neue GUID des Tutors (optional, muss Tutor sein).
    /// </summary>
    public Guid? TutorId { get; set; }

    /// <summary>
    /// Die neue GUID des zweiten Betreuers (optional, muss Tutor sein).
    /// </summary>
    public Guid? SecondSupervisorId { get; set; }

    /// <summary>
    /// Die neue GUID des Themas (optional).
    /// </summary>
    public Guid? TopicId { get; set; }
}