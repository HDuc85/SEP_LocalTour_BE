using System;
using System.Collections.Generic;

namespace LocalTour.Domain.Entities;

public partial class PlaceTag
{
    public int Id { get; set; }

    public int TagId { get; set; }

    public int PlaceId { get; set; }

    public virtual Place Place { get; set; } = null!;

    public virtual Tag Tag { get; set; } = null!;
}
