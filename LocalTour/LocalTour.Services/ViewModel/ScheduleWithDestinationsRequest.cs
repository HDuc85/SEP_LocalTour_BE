using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.ViewModel
{
    public class ScheduleWithDestinationsRequest
    {
        public string ScheduleName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; } 
        public bool IsPublic { get; set; } 

        public List<DestinationRequest> Destinations { get; set; }
    }
}
