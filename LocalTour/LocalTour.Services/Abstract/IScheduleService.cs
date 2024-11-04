using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LocalTour.Services.ViewModel;

namespace LocalTour.Services.Abstract
{
    public interface IScheduleService
    {
        Task<ScheduleRequest?> GetScheduleByIdAsync(int id);
        Task<List<ScheduleRequest>> GetSchedulesByUserIdAsync(Guid userId);
        Task<ScheduleRequest> CreateScheduleAsync(ScheduleRequest request);
        Task<bool> UpdateScheduleAsync(int id, ScheduleRequest request);
        Task<bool> DeleteScheduleAsync(int id);
        Task<ScheduleRequest?> CloneScheduleAsync(int id, Guid userId);
    }
}
