using AutoMapper;
using LocalTour.Data.Abstract;
using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;
using LocalTour.Services.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LocalTour.Services.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ScheduleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ScheduleRequest?> GetScheduleByIdAsync(int id)
        {
            var schedule = await _unitOfWork.RepositorySchedule.GetById(id);
            return schedule == null ? null : _mapper.Map<ScheduleRequest>(schedule);
        }

        public async Task<List<ScheduleRequest>> GetSchedulesByUserIdAsync(Guid userId)
        {
            Expression<Func<Schedule, bool>> expression = schedule => schedule.UserId == userId;
            var schedules = await _unitOfWork.RepositorySchedule.GetData(expression);
            return _mapper.Map<List<ScheduleRequest>>(schedules.ToList());
        }

        public async Task<ScheduleRequest> CreateScheduleAsync(ScheduleRequest request)
        {
            var schedule = _mapper.Map<Schedule>(request);
            await _unitOfWork.RepositorySchedule.Insert(schedule);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<ScheduleRequest>(schedule);
        }

        public async Task<bool> UpdateScheduleAsync(int id, ScheduleRequest request)
        {
            var schedule = await _unitOfWork.RepositorySchedule.GetById(id);
            if (schedule == null) return false;

            _mapper.Map(request, schedule);
            _unitOfWork.RepositorySchedule.Update(schedule);
            await _unitOfWork.CommitAsync();
            return true;
        }

        public async Task<bool> DeleteScheduleAsync(int id)
        {
            var schedule = await _unitOfWork.RepositorySchedule.GetById(id);
            if (schedule == null) return false;

            _unitOfWork.RepositorySchedule.Delete(schedule);
            await _unitOfWork.CommitAsync();
            return true;
        }

        public async Task<ScheduleRequest?> CloneScheduleAsync(int id, Guid newUserId)
        {
            // Fetch the original schedule by its ID
            var schedule = await _unitOfWork.RepositorySchedule.GetById(id);
            if (schedule == null) return null; // Return null if the schedule does not exist

            // Create a cloned schedule object
            var clonedSchedule = new Schedule
            {
                UserId = newUserId, // Assign the new user ID
                ScheduleName = $"{schedule.ScheduleName} - Copy", // Modify the schedule name
                StartDate = schedule.StartDate,
                EndDate = schedule.EndDate,
                CreatedDate = DateTime.UtcNow,
                Status = schedule.Status,
                IsPublic = schedule.IsPublic
            };

            // Insert the cloned schedule into the database
            await _unitOfWork.RepositorySchedule.Insert(clonedSchedule);
            await _unitOfWork.CommitAsync(); // Commit changes to save the cloned schedule

            // Clone each associated destination
            foreach (var destination in schedule.Destinations)
            {
                var clonedDestination = new Destination
                {
                    ScheduleId = clonedSchedule.Id, // Link to the new schedule ID
                    PlaceId = destination.PlaceId,
                    StartDate = destination.StartDate,
                    EndDate = destination.EndDate,
                    Detail = destination.Detail,
                    IsArrived = destination.IsArrived
                };
                await _unitOfWork.RepositoryDestination.Insert(clonedDestination);
            }

            await _unitOfWork.CommitAsync(); // Commit changes to save the cloned destinations

            // Map the cloned schedule to ScheduleRequest DTO for return
            return _mapper.Map<ScheduleRequest>(clonedSchedule);
        }

    }
}
