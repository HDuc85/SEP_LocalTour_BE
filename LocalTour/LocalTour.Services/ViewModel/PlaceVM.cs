using LocalTour.Domain.Entities;
using LocalTour.Services.Common.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.ViewModel
{
    public class PlaceVM : IMapFrom<Place>
    {
        public int WardId { get; set; }

        public string PhotoDisplay { get; set; } = null!;

        public TimeOnly TimeOpen { get; set; }

        public TimeOnly TimeClose { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public string Status { get; set; }

        public string? ContactLink { get; set; }

        public virtual ICollection<PlaceActivityRequest> PlaceActivities { get; set; }

        public virtual ICollection<PlaceMediumRequest> PlaceMedia { get; set; }
    }
}
