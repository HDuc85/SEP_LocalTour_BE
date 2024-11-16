using System;

namespace LocalTour.Services.ViewModel
{
    public class CreatePostCommentRequest
    {
        //public int? Id { get; set; }
        public int PostId { get; set; }
        public int? ParentId { get; set; } 
        //public Guid UserId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
