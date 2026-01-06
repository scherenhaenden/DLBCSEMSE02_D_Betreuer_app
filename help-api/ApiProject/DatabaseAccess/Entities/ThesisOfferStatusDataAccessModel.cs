using System.ComponentModel.DataAnnotations.Schema;

namespace ApiProject.DatabaseAccess.Entities;

[Table("ThesisOfferStatuses")]
public class ThesisOfferStatusDataAccessModel : BaseEntity
{
    /// <summary>
    /// Status name of the thesis offer.
    /// Examples: "OPEN", "CLOSED", "ARCHIVED".
    /// </summary>
    public required string Name { get; set; }

    /* ---------- Navigation properties ---------- */

    public virtual ICollection<ThesisOfferDataAccessModel> ThesisOffers { get; set; }
        = new List<ThesisOfferDataAccessModel>();
}