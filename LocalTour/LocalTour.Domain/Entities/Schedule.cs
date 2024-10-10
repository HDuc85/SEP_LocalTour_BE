using System;
using System.Collections.Generic;

namespace LocalTour.Domain.Entities;

public partial class Schedule
{
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public string ScheduleName { get; set; } = null!;

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime CreatedDate { get; set; }

    public string Status { get; set; } = null!;

    public bool? IsPublic { get; set; }

    public virtual ICollection<Destination> Destinations { get; set; } = new List<Destination>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual ICollection<ScheduleComment> ScheduleComments { get; set; } = new List<ScheduleComment>();

    public virtual ICollection<ScheduleLike> ScheduleLikes { get; set; } = new List<ScheduleLike>();

    public virtual ICollection<ScheduleUserLike> ScheduleUserLikes { get; set; } = new List<ScheduleUserLike>();

    public virtual User User { get; set; } = null!;
}
