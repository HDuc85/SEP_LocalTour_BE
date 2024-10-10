using System;
using System.Collections.Generic;

namespace LocalTour.Domain.Entities;

public partial class ScheduleCommentLike
{
    public int Id { get; set; }

    public int ScheduleCommentId { get; set; }

    public Guid UserId { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual ScheduleComment ScheduleComment { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
