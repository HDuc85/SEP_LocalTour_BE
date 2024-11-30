using LocalTour.Domain.Entities;
using LocalTour.Services.Common.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LocalTour.Services.ViewModel
{
    public class EventViewModel : IMapFrom<Event>
    {
        public int Id { get; set; }
        public int PlaceId { get; set; }

        public string PlaceName { get; set; }

        public string EventName { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string EventStatus { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public string? EventPhotoDisplay { get; set; }
        [JsonIgnore]

        public virtual Place Place { get; set; } = null!;
    }
}
