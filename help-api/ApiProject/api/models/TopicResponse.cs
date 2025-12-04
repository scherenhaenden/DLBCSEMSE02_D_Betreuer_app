namespace ApiProject.Api.Models;

/// <summary>
/// Response model for Topic.
/// </summary>
public class TopicResponse
{
    /// <summary>
    /// The unique identifier of the topic.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The title of the topic.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The description of the topic.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The subject area of the topic.
    /// </summary>
    public string SubjectArea { get; set; } = string.Empty;

    /// <summary>
    /// Whether the topic is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// The ID of the tutor who created the topic.
    /// </summary>
    public Guid TutorId { get; set; }
}
