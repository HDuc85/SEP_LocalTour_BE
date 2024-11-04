using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.ViewModel
{
    public class ScheduleLikeRequest
    {
        public int ScheduleId { get; set; }
        public Guid UserId { get; set; }
    }
}

