using LocalTour.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.ViewModel
{
    public class CreateDestinationRequest
    {
        public int ScheduleId { get; set; }
        public int PlaceId { get; set; }

        //public string PlaceName { get; set; }

        //public string placePhotoDisplay { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Detail { get; set; }
        public bool IsArrived { get; set; }
    }
}
