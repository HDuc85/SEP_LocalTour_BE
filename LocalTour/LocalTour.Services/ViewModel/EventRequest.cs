using LocalTour.Domain.Entities;
using LocalTour.Services.Common.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.ViewModel
{
    public class EventRequest : IMapFrom<Event>
    {
        public string EventName { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string EventStatus { get; set; } = null!;
    }
}
