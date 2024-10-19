using LocalTour.Services.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.Abstract
{
    public interface IPlaceActivityService
    {
        Task<PlaceActivityRequest> CreatePlaceActivity(int placeid, PlaceActivityRequest request);
        Task<PaginatedList<PlaceActivityRequest>> GetAllActivityByPlaceid(int placeid, GetPlaceActivityRequest request);
    }
}
