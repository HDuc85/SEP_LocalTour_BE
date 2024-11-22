using LocalTour.Domain.Entities;
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
        Task<PaginatedList<EventRequest>> GetAllEventByPlaceid(int placeid, GetEventRequest request);
        Task<PaginatedList<EventResponse>> GetAllEventByVisitor(int placeid, GetEventRequest request);
        Task<Event> GetEventById(int placeid, int eventid);
        Task<EventRequest> CreateEvent(int placeid, EventRequest request);
        Task<EventRequest> UpdateEvent(int placeid, int eventid, EventRequest request);
        Task<bool> DeleteEvent(int placeid, int eventid);
        Task<Event> ChangeStatusEvent(int placeid, int eventid, string status);
    }
}
