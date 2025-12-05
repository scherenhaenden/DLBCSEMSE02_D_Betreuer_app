namespace ApiProject.Api.Models;

/// <summary>
/// Request model for updating a topic.
/// </summary>
public class UpdateTopicRequest
{
    /// <summary>
    /// The new title of the topic.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// The new description of the topic.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The new subject area of the topic.
    /// </summary>
    public string? SubjectArea { get; set; }

    /// <summary>
    /// Whether the topic is active.
    /// </summary>
    public bool? IsActive { get; set; }
}
