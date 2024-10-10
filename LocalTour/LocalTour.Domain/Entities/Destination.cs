using System;
using System.Collections.Generic;

namespace LocalTour.Domain.Entities;

public partial class Destination
{
    public int Id { get; set; }

    public int ScheduleId { get; set; }

    public int PlaceId { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Detail { get; set; }

    public bool IsArrived { get; set; }

    public virtual Place Place { get; set; } = null!;

    public virtual Schedule Schedule { get; set; } = null!;
}
