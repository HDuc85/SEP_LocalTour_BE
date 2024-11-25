using LocalTour.Domain.Entities;
using LocalTour.Services.Common.Mapping;

namespace LocalTour.Services.ViewModel
{
    public class PostRequest : IMapFrom<Post>
    {
        public int Id { get; set; }
        public Guid AuthorId { get; set; }
        public string AuthorFullName { get; set; }
        public string AuthorProfilePictureUrl { get; set; }
        public int? PlaceId { get; set; }
        public string? PlacePhotoDisplay { get; set; }
        public string PlaceName { get; set; }
        public int? ScheduleId { get; set; }
        public string ScheduleName { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Content { get; set; }
        public bool Public { get; set; }
        public bool isLiked { get; set; } 
        public List<PostCommentRequest> Comments { get; set; } = new List<PostCommentRequest>();
        public int TotalLikes { get; set; }
        public int TotalComments { get; set; }
        public List<PostMedium> Media { get; set; } = new List<PostMedium>();
    }

}
