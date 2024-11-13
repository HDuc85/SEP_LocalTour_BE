using LocalTour.Services.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.Abstract
{
    public interface IPlaceFeedbackService
    {
        Task<PlaceFeedbackRequest> CreateFeedback(int placeid, PlaceFeedbackRequest request);
        Task<PlaceFeedbackRequest> UpdateFeedback(int placeid,int feedbackid, PlaceFeedbackRequest request);
        Task<bool> DeleteFeedback(int placeid, int feedbackid);
        Task<PaginatedList<PlaceFeedbackRequest>> GetAllFeedbackByPlace(int placeid, GetPlaceFeedbackRequest request);
    }
}
