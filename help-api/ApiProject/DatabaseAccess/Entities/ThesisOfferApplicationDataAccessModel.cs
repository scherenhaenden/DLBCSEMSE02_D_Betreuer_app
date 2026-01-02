using System.ComponentModel.DataAnnotations.Schema;

namespace ApiProject.DatabaseAccess.Entities;

[Table("ThesisOfferApplications")]
public class ThesisOfferApplicationDataAccessModel : BaseEntity
{
    /// <summary>
    /// Reference to the thesis offer the student is applying for.
    /// </summary>
    public Guid ThesisOfferId { get; set; }
    public virtual ThesisOfferDataAccessModel? ThesisOffer { get; set; }

    /// <summary>
    /// Student who applies for the thesis offer.
    /// </summary>
    public Guid StudentId { get; set; }
    public virtual UserDataAccessModel? Student { get; set; }

    /// <summary>
    /// Status of the application (PENDING, ACCEPTED, REJECTED, etc.).
    /// Reuses RequestStatus.
    /// </summary>
    public Guid RequestStatusId { get; set; }
    public virtual RequestStatusDataAccessModel? RequestStatus { get; set; }

    /// <summary>
    /// Optional message provided by the student with the application.
    /// </summary>
    public string? Message { get; set; }
}