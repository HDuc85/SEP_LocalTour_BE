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
        Task<FeedbackRequest> CreateFeedback(FeedbackRequest request);
        Task<FeedbackRequest> UpdateFeedback(int placeid,int feedbackid, FeedbackRequest request);
        Task<bool> DeleteFeedback(int placeid, int feedbackid);
        Task<PaginatedList<PlaceFeedbackRequest>> GetAllFeedbackByPlace(GetPlaceFeedbackRequest request);
    }
}
