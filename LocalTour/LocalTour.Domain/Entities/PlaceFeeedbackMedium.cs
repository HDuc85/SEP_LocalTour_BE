using System;
using System.Collections.Generic;

namespace LocalTour.Domain.Entities;

public partial class PlaceFeeedbackMedium
{
    public int Id { get; set; }

    public int FeedbackId { get; set; }

    public string Type { get; set; } = null!;

    public string Url { get; set; } = null!;

    public DateTime CreateDate { get; set; }

    public virtual PlaceFeeedback Feedback { get; set; } = null!;
}
