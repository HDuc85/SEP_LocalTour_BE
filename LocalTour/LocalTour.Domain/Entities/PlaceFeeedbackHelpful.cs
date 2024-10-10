using System;
using System.Collections.Generic;

namespace LocalTour.Domain.Entities;

public partial class PlaceFeeedbackHelpful
{
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public int PlaceFeedBackId { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual PlaceFeeedback PlaceFeedBack { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
