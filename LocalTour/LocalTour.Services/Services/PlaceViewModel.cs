using LocalTour.Services.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.Services
{
    public class PlaceViewModel
    {
        public int Id { get; set; }

        public string PhotoDisplay { get; set; } = null!;

        public TimeOnly TimeOpen { get; set; }

        public TimeOnly TimeClose { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public string? ContactLink { get; set; }

        public Guid AuthorId { get; set; }
        public bool isUserFeedbacked { get; set; }
        public virtual ICollection<PlaceActivityRequest> PlaceActivities { get; set; }

        public virtual ICollection<PlaceMediumRequest> PlaceMedia { get; set; }
        public virtual ICollection<EventRequest> Event { get; set; }
        public virtual ICollection<PlaceTranslationRequest> PlaceTranslation { get; set; }
    }
}
