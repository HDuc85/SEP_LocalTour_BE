using System;
using System.Collections.Generic;

namespace LocalTour.Domain.Entities;

public partial class PlaceActivityMedium
{
    public int Id { get; set; }

    public int PlaceActivityId { get; set; }

    public string Type { get; set; } = null!;

    public string Url { get; set; } = null!;

    public DateTime CreateDate { get; set; }

    public virtual PlaceActivity PlaceActivity { get; set; } = null!;
}
