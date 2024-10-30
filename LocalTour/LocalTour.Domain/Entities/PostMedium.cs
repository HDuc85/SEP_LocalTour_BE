using System;
using System.Collections.Generic;

namespace LocalTour.Domain.Entities;

public partial class PostMedium
{
    public int Id { get; set; }

    public int PostId { get; set; }

    // Photo or Video
    public string Type { get; set; } = null!; 

    public string Url { get; set; } = null!;

    public DateTime CreateDate { get; set; }

    public virtual Post Post { get; set; } = null!;
}
