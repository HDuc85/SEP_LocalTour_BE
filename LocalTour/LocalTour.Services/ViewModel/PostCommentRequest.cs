using LocalTour.Domain.Entities;
using LocalTour.Services.Common.Mapping;

namespace LocalTour.Services.ViewModel
{
    public class PostCommentRequest : IMapFrom<PostComment>
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int? ParentId { get; set; } // Nullable for top-level comments
        public Guid UserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public List<PostMediumRequest> Media { get; set; } = new List<PostMediumRequest>(); // List of media associated with the comment
        public int TotalLikes { get; set; } // Total number of likes
        public bool LikedByUser { get; set; } // Indicates if the current user liked the comment
    }
}
