using System;
using System.Collections.Generic;

namespace LocalTour.Domain.Entities;

public partial class PlaceFeeedback
{
    public int Id { get; set; }

    public int PlaceId { get; set; }

    public Guid UserId { get; set; }

    public int Rating { get; set; }

    public string? Content { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual Place Place { get; set; } = null!;

    public virtual ICollection<PlaceFeeedbackHelpful> PlaceFeeedbackHelpfuls { get; set; } = new List<PlaceFeeedbackHelpful>();

    public virtual ICollection<PlaceFeeedbackPhoto> PlaceFeeedbackPhotos { get; set; } = new List<PlaceFeeedbackPhoto>();

    public virtual PlaceFeeedbackVideo? PlaceFeeedbackVideo { get; set; }

    public virtual User User { get; set; } = null!;
}
