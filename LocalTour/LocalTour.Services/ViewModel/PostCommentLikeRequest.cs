using System;

namespace LocalTour.Services.ViewModel
{
    public class PostCommentLikeRequest
    {
        public int PostCommentId { get; set; } // Id of the comment to like/unlike
        public Guid UserId { get; set; } // Id of the user performing the action
    }
}
