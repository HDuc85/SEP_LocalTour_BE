using System;
using System.Collections.Generic;

namespace LocalTour.Domain.Entities;

public partial class ScheduleLike
{
    public int Id { get; set; }

    public int ScheduleId { get; set; }

    public Guid UserId { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual Schedule Schedule { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
