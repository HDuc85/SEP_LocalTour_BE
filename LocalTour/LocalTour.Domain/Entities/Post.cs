using System;
using System.Collections.Generic;

namespace LocalTour.Domain.Entities;

public partial class Post
{
    public int Id { get; set; }

    public Guid AuthorId { get; set; }

    public int? PlaceId { get; set; }

    public int? ScheduleId { get; set; }

    public double Longitude { get; set; }

    public double Latitude { get; set; }

    public string Title { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public DateTime UpdateDate { get; set; }

    public string Content { get; set; } = null!;

    public bool Public { get; set; }

    public virtual User Author { get; set; } = null!;

    public virtual Place? Place { get; set; }

    public virtual ICollection<PostComment> PostComments { get; set; } = new List<PostComment>();

    public virtual ICollection<PostLike> PostLikes { get; set; } = new List<PostLike>();

    public virtual ICollection<PostPhoto> PostPhotos { get; set; } = new List<PostPhoto>();

    public virtual ICollection<PostVideo> PostVideos { get; set; } = new List<PostVideo>();

    public virtual Schedule? Schedule { get; set; }
}
