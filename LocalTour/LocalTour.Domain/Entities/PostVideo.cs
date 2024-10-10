using System;
using System.Collections.Generic;

namespace LocalTour.Domain.Entities;

public partial class PostVideo
{
    public int Id { get; set; }

    public int PostId { get; set; }

    public string Url { get; set; } = null!;

    public DateTime CreateDate { get; set; }

    public virtual Post Post { get; set; } = null!;
}
