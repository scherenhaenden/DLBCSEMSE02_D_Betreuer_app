namespace ApiProject.Api.Models;

/// <summary>
/// Represents a paginated response for API.
/// </summary>
/// <typeparam name="T">The type of items in the response.</typeparam>
public class PaginatedResponse<T>
{
    /// <summary>
    /// The items on the current page.
    /// </summary>
    public IReadOnlyCollection<T> Items { get; set; } = new List<T>();

    /// <summary>
    /// The total number of items across all pages.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// The current page number (1-based).
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// The number of items per page.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// The total number of pages.
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
