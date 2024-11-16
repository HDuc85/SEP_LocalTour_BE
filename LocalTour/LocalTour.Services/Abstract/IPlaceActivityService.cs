using LocalTour.Domain.Entities;
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
        Task<PlaceActivity> GetActivityById(int placeid, int activityid, string languageCode);
        Task<PlaceActivityRequest> UpdateActivity(int placeid, int activityid, PlaceActivityRequest request);
        Task<bool> DeletePlaceActivity(int placeid, int activityid);
    }
}
