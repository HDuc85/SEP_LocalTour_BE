using System;
using System.Collections.Generic;

namespace LocalTour.Domain.Entities;

public partial class PlaceVideo
{
    public int Id { get; set; }

    public int PlaceId { get; set; }

    public string Url { get; set; } = null!;

    public DateTime CreateDate { get; set; }

    public virtual Place Place { get; set; } = null!;
}
