using System;
using System.Collections.Generic;

namespace LocalTour.Domain.Entities;

public partial class ScheduleUserLike
{
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public int ScheduleId { get; set; }

    public bool? IsPublic { get; set; }

    public virtual Schedule Schedule { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
