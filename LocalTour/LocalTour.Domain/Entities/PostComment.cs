using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LocalTour.Domain.Entities;

public partial class PostComment
{
    public int Id { get; set; }

    public int PostId { get; set; }

    public int? ParentId { get; set; }

    public Guid UserId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public virtual ICollection<PostComment> InverseParent { get; set; } = new List<PostComment>();

    [JsonIgnore]
    public virtual PostComment? Parent { get; set; }

    public virtual Post Post { get; set; } = null!;

    public virtual ICollection<PostCommentLike> PostCommentLikes { get; set; } = new List<PostCommentLike>();

    public virtual User User { get; set; } = null!;
}
