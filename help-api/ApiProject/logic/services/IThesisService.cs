using ApiProject.Db.Entities;
using ApiProject.Logic.Models;

namespace ApiProject.Logic.Services;

/// <summary>
/// Schnittstelle für den Thesis-Service, der CRUD-Operationen für Thesen bereitstellt.
/// </summary>
public interface IThesisService
{
    /// <summary>
    /// Gibt alle Thesen zurück.
    /// </summary>
    /// <returns>Eine schreibgeschützte Sammlung aller Thesen.</returns>
    Task<IReadOnlyCollection<Thesis>> GetAllAsync();

    /// <summary>
    /// Gibt eine These anhand ihrer ID zurück.
    /// </summary>
    /// <param name="id">Die GUID der These.</param>
    /// <returns>Die These oder null, wenn nicht gefunden.</returns>
    Task<Thesis?> GetByIdAsync(Guid id);

    /// <summary>
    /// Erstellt eine neue These basierend auf der Anfrage.
    /// </summary>
    /// <param name="request">Die Anfrage mit den Details der neuen These.</param>
    /// <returns>Die erstellte These.</returns>
    /// <exception cref="InvalidOperationException">Wird ausgelöst, wenn Validierungen fehlschlagen.</exception>
    Task<Thesis> CreateThesisAsync(ThesisCreateRequest request);

    /// <summary>
    /// Aktualisiert eine bestehende These.
    /// </summary>
    /// <param name="id">Die GUID der zu aktualisierenden These.</param>
    /// <param name="request">Die Anfrage mit den zu aktualisierenden Feldern.</param>
    /// <returns>Die aktualisierte These.</returns>
    /// <exception cref="KeyNotFoundException">Wird ausgelöst, wenn die These nicht gefunden wird.</exception>
    /// <exception cref="InvalidOperationException">Wird ausgelöst, wenn Validierungen fehlschlagen.</exception>
    Task<Thesis> UpdateThesisAsync(Guid id, ThesisUpdateRequest request);

    /// <summary>
    /// Löscht eine These anhand ihrer ID.
    /// </summary>
    /// <param name="id">Die GUID der zu löschenden These.</param>
    /// <returns>True, wenn die These gelöscht wurde; sonst false.</returns>
    Task<bool> DeleteThesisAsync(Guid id);
}