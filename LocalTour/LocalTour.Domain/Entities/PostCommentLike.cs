using System;
using System.Collections.Generic;

namespace LocalTour.Domain.Entities;

public partial class PostCommentLike
{
    public int Id { get; set; }

    public int PostCommentId { get; set; }

    public Guid UserId { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual PostComment PostComment { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
