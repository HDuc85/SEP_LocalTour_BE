using LocalTour.Domain.Entities;
using LocalTour.Services.Common.Mapping;

public class PostCommentRequest
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; }
    public DateTime CreatedDate { get; set; }
    public int? ParentId { get; set; } // Nullable for parent comments
    public List<PostCommentRequest>? ChildComments { get; set; } // Nullable property
    public int TotalLikes { get; set; }
    public bool LikedByUser { get; set; }
    public string UserFullName { get; set; }
    public string UserProfilePictureUrl { get; set; }
}
