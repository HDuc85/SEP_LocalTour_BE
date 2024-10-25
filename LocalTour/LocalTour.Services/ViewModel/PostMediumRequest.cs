using LocalTour.Domain.Entities;
using LocalTour.Services.Common.Mapping;

namespace LocalTour.Services.ViewModel
{
    public class PostMediumRequest : IMapFrom<PostMedium>
    {
        public int? Id { get; set; }
        public int PostId { get; set; }
        public string Type { get; set; } = null!; // Set to either "Photo" or "Video"
        public string Url { get; set; } = null!;
        public DateTime CreateDate { get; set; }
    }
}
