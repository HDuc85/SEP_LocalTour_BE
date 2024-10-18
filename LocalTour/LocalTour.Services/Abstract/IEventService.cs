using LocalTour.Services.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.Abstract
{
    public interface IEventService
    {
        Task<EventRequest> CreateEvent(int placeid, EventRequest request);
        Task<EventRequest> UpdateEvent(int placeid, int eventid, EventRequest request);
    }
}
