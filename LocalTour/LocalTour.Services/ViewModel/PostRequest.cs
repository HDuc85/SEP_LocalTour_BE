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

        public void Mapping(AutoMapper.Profile profile)
        {
            profile.CreateMap<PostRequest, Post>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Ignore Id if it's auto-generated
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow)) // Set created date
                .ForMember(dest => dest.UpdateDate, opt => opt.MapFrom(src => DateTime.UtcNow)); // Set update date
        }
    }
}
