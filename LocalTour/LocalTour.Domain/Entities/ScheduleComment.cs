using System;
using System.Collections.Generic;

namespace LocalTour.Domain.Entities;

public partial class ScheduleComment
{
    public int Id { get; set; }

    public int ScheduleId { get; set; }

    public Guid UserId { get; set; }

    public int? ParentFeedBackId { get; set; }

    public string? Content { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual ICollection<ScheduleComment> InverseParentFeedBack { get; set; } = new List<ScheduleComment>();

    public virtual ScheduleComment? ParentFeedBack { get; set; }

    public virtual Schedule Schedule { get; set; } = null!;

    public virtual ICollection<ScheduleCommentLike> ScheduleCommentLikes { get; set; } = new List<ScheduleCommentLike>();

    public virtual ICollection<ScheduleCommentPhoto> ScheduleCommentPhotos { get; set; } = new List<ScheduleCommentPhoto>();

    public virtual ICollection<ScheduleCommentVideo> ScheduleCommentVideos { get; set; } = new List<ScheduleCommentVideo>();

    public virtual User User { get; set; } = null!;
}
