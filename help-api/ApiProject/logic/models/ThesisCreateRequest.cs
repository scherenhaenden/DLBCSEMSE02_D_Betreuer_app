using ApiProject.Db.Entities;

namespace ApiProject.Logic.Models;

/// <summary>
/// Anfrage-Klasse für die Erstellung einer neuen These.
/// </summary>
public sealed class ThesisCreateRequest
{
    /// <summary>
    /// Der Titel der These.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Die GUID des Besitzers (muss Student sein).
    /// </summary>
    public Guid OwnerId { get; set; }

    /// <summary>
    /// Die GUID des Tutors (muss Tutor sein).
    /// </summary>
    public Guid TutorId { get; set; }

    /// <summary>
    /// Die GUID des zweiten Betreuers (optional, muss Tutor sein).
    /// </summary>
    public Guid? SecondSupervisorId { get; set; }

    /// <summary>
    /// Die GUID des Themas (optional).
    /// </summary>
    public Guid? TopicId { get; set; }

    /// <summary>
    /// Der Fortschritt in Prozent (0-100).
    /// </summary>
    public int ProgressPercent { get; set; }

    /// <summary>
    /// Optionaler Pfad zum Exposé.
    /// </summary>
    public string? ExposePath { get; set; }

    /// <summary>
    /// Der Abrechnungsstatus (Standard: None).
    /// </summary>
    public BillingStatus BillingStatus { get; set; } = BillingStatus.None;
}