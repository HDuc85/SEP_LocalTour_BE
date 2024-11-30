using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LocalTour.Services.Model;
using LocalTour.Domain.Entities;
using LocalTour.Services.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace LocalTour.Services.Abstract
{
        public interface IScheduleService
        {
                Task<PaginatedList<ScheduleRequest>> GetAllSchedulesAsync(GetScheduleRequest request, String userId);
                Task<ScheduleRequest?> GetScheduleByIdAsync(int id);
                Task<List<ScheduleRequest>> GetSchedulesByUserIdAsync(Guid userId);
                Task<ScheduleRequest> CreateScheduleAsync(CreateScheduleRequest request, String userId);
                Task<bool> UpdateScheduleAsync(UpdateScheduleRequest request, string userId);
                Task<bool> DeleteScheduleAsync(int id, string userId);
                Task<ServiceResponseModel<List<DestinationVM>>> GenerateSchedule(SuggestScheduleRequest request, string userId);
                Task<ScheduleRequest?> CloneScheduleFromOtherUserAsync(int scheduleId, Guid newUserId);
                Task<ScheduleRequest> SaveSuggestedSchedule(ScheduleWithDestinationsRequest request, Guid userId);
        }
}
