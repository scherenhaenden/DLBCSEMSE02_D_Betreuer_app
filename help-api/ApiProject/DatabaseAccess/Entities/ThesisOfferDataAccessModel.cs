using System.ComponentModel.DataAnnotations.Schema;

namespace ApiProject.DatabaseAccess.Entities;

[Table("ThesisOffers")]
public class ThesisOfferDataAccessModel : BaseEntity
{
    /// <summary>
    /// Title of the offered thesis topic.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Detailed description of the thesis topic.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Supervision / subject area this offer belongs to.
    /// </summary>
    public Guid SubjectAreaId { get; set; }
    public virtual SubjectAreaDataAccessModel? SubjectArea { get; set; }

    /// <summary>
    /// Tutor who offers this thesis topic.
    /// </summary>
    public Guid TutorId { get; set; }

    /// <summary>
    /// Status of the thesis offer (OPEN, CLOSED, ARCHIVED).
    /// </summary>
    public Guid ThesisOfferStatusId { get; set; }
    public virtual ThesisOfferStatusDataAccessModel? ThesisOfferStatus { get; set; }

    /// <summary>
    /// Optional maximum number of students that can be assigned
    /// to this offer (typically 1).
    /// </summary>
    public int? MaxStudents { get; set; }

    /// <summary>
    /// Optional expiration date of the offer.
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    /* ---------- Navigation properties ---------- */



    public virtual UserDataAccessModel? Tutor { get; set; }

    public virtual ICollection<ThesisOfferApplicationDataAccessModel> Applications { get; set; }
        = new List<ThesisOfferApplicationDataAccessModel>();
}