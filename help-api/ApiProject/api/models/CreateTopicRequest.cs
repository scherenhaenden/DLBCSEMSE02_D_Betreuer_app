namespace ApiProject.Api.Models;

/// <summary>
/// Request model for creating a topic.
/// </summary>
public class CreateTopicRequest
{
    /// <summary>
    /// The title of the topic.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// The description of the topic.
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// The subject area of the topic.
    /// </summary>
    public required string SubjectArea { get; set; }

    /// <summary>
    /// The ID of the tutor creating the topic.
    /// </summary>
    public required Guid TutorId { get; set; }
}
