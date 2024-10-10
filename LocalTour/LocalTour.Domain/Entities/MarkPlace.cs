using System;
using System.Collections.Generic;

namespace LocalTour.Domain.Entities;

public partial class MarkPlace
{
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public int PlaceId { get; set; }

    public DateTime CreatedDate { get; set; }

    public bool IsVisited { get; set; }

    public virtual Place Place { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
