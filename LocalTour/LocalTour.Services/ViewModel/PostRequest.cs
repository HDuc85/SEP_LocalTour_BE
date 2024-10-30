using LocalTour.Domain.Entities;
using LocalTour.Services.Common.Mapping;

namespace LocalTour.Services.ViewModel
{
    public class PostRequest : IMapFrom<Post>
    {
        public Guid AuthorId { get; set; }
        public int? PlaceId { get; set; }
        public int? ScheduleId { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool Public { get; set; }
        public List<int> Tags { get; set; } = new List<int>();
    }
}
