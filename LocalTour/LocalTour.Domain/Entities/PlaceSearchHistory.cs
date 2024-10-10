using System;
using System.Collections.Generic;

namespace LocalTour.Domain.Entities;

public partial class PlaceSearchHistory
{
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public int PlaceId { get; set; }

    public DateTime LastSearch { get; set; }

    public virtual Place Place { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
