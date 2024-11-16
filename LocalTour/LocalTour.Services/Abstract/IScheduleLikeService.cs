using LocalTour.Services.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.Abstract
{
    public interface IScheduleLikeService
    {
        Task<int> LikeScheduleAsync(int scheduleId, Guid userId);
        Task<bool> UnlikeScheduleAsync(int ScheduleId, Guid userId);
        Task<int> GetTotalLikesAsync(int scheduleId);
        Task<List<Guid>> GetUsersLikedAsync(int scheduleId);
    }
}
