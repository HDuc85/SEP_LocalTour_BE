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
        public int? ScheduleId { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Content { get; set; }
        public bool Public { get; set; }

        public List<PostCommentRequest> Comments { get; set; } = new List<PostCommentRequest>();
        public int TotalLikes { get; set; }
        public List<PostMediumRequest> Media { get; set; } = new List<PostMediumRequest>();
    }

}
