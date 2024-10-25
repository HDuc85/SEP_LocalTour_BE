using System;
using System.Collections.Generic;

namespace LocalTour.Domain.Entities;

public partial class PlaceMedium
{
    public int Id { get; set; }

    public int PlaceId { get; set; }

    public string Type { get; set; } = null!;

    public string Url { get; set; } = null!;

    public DateTime CreateDate { get; set; }

    public virtual Place Place { get; set; } = null!;
}
