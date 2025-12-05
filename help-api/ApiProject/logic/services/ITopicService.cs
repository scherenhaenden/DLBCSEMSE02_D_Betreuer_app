using ApiProject.Db.Entities;
using ApiProject.Logic.Models;

namespace ApiProject.Logic.Services;

/// <summary>
/// Schnittstelle für den Topic-Service, der CRUD-Operationen für Themen bereitstellt.
/// </summary>
public interface ITopicService
{
    /// <summary>
    /// Gibt alle Themen paginiert zurück.
    /// </summary>
    /// <param name="page">Die Seitennummer (1-basiert).</param>
    /// <param name="pageSize">Die Anzahl der Elemente pro Seite.</param>
    /// <returns>Ein paginiertes Ergebnis mit den Themen.</returns>
    Task<PaginatedResult<Topic>> GetAllAsync(int page, int pageSize);

    /// <summary>
    /// Gibt ein Thema anhand seiner ID zurück.
    /// </summary>
    /// <param name="id">Die GUID des Themas.</param>
    /// <returns>Das Thema oder null, wenn nicht gefunden.</returns>
    Task<Topic?> GetByIdAsync(Guid id);

    /// <summary>
    /// Sucht Themen nach Titel oder Fachbereich.
    /// </summary>
    /// <param name="searchTerm">Der Suchbegriff.</param>
    /// <param name="page">Die Seitennummer (1-basiert).</param>
    /// <param name="pageSize">Die Anzahl der Elemente pro Seite.</param>
    /// <returns>Ein paginiertes Ergebnis mit den gefundenen Themen.</returns>
    Task<PaginatedResult<Topic>> SearchAsync(string searchTerm, int page, int pageSize);

    /// <summary>
    /// Erstellt ein neues Thema.
    /// </summary>
    /// <param name="title">Der Titel des Themas.</param>
    /// <param name="description">Die Beschreibung.</param>
    /// <param name="subjectArea">Der Fachbereich.</param>
    /// <param name="tutorId">Die ID des Tutors.</param>
    /// <returns>Das erstellte Thema.</returns>
    Task<Topic> CreateTopicAsync(string title, string description, string subjectArea, Guid tutorId);

    /// <summary>
    /// Aktualisiert ein bestehendes Thema.
    /// </summary>
    /// <param name="id">Die GUID des zu aktualisierenden Themas.</param>
    /// <param name="title">Der neue Titel.</param>
    /// <param name="description">Die neue Beschreibung.</param>
    /// <param name="subjectArea">Der neue Fachbereich.</param>
    /// <param name="isActive">Ob das Thema aktiv ist.</param>
    /// <returns>Das aktualisierte Thema.</returns>
    Task<Topic> UpdateTopicAsync(Guid id, string? title, string? description, string? subjectArea, bool? isActive);

    /// <summary>
    /// Löscht ein Thema anhand seiner ID.
    /// </summary>
    /// <param name="id">Die GUID des zu löschenden Themas.</param>
    /// <returns>True, wenn das Thema gelöscht wurde; sonst false.</returns>
    Task<bool> DeleteTopicAsync(Guid id);
}
