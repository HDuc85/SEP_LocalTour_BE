using System;
using System.Collections.Generic;

namespace LocalTour.Domain.Entities;

public partial class PostLike
{
    public int Id { get; set; }

    public int PostId { get; set; }

    public Guid UserId { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual Post Post { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
