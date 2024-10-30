using LocalTour.Domain.Entities;
using LocalTour.Services.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.Abstract
{
    public interface IPlaceFeedbackHelpfulService
    {
        Task<PlaceFeeedbackHelpful> CreateEvent(int placeid, EventRequest request);
    }
}
