using System;
using System.Collections.Generic;

namespace LocalTour.Domain.Entities;

public partial class Event
{
    public int Id { get; set; }

    public int PlaceId { get; set; }

    public string EventName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string EventStatus { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Place Place { get; set; } = null!;
}
