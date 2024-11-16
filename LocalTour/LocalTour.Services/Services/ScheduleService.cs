using AutoMapper;
using LocalTour.Data.Abstract;
using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;
using LocalTour.Services.ViewModel;
using Microsoft.EntityFrameworkCore;
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

        public async Task<PaginatedList<ScheduleRequest>> GetAllSchedulesAsync(GetScheduleRequest request)
        {
            var schedulesQuery = _unitOfWork.RepositorySchedule.GetAll()
                .Include(s => s.ScheduleLikes)
                .Include(s => s.Destinations)
                .AsQueryable();

            if (request.UserId != null)
            {
                schedulesQuery = schedulesQuery.Where(s => s.UserId == request.UserId);
            }

            if (request.SortBy == "like")
            {
                schedulesQuery = request.SortOrder == "asc"
                    ? schedulesQuery.OrderBy(s => s.ScheduleLikes.Count)
                    : schedulesQuery.OrderByDescending(s => s.ScheduleLikes.Count);
            }
            else if (request.SortBy == "date")
            {
                schedulesQuery = request.SortOrder == "asc"
                    ? schedulesQuery.OrderBy(s => s.CreatedDate)
                    : schedulesQuery.OrderByDescending(s => s.CreatedDate);
            }

            var totalCount = await schedulesQuery.CountAsync();
            var items = await schedulesQuery
                .Skip((request.Page - 1) * request.Size ?? 0)
                .Take(request.Size ?? 10)
                .ToListAsync();

            // Map the result to ScheduleRequest DTO
            var scheduleRequests = _mapper.Map<List<ScheduleRequest>>(items);

            foreach (var scheduleRequest in scheduleRequests)
            {
                // Populate additional properties like total likes
                var totalLikes = await _unitOfWork.RepositoryScheduleLike.GetData(l => l.ScheduleId == scheduleRequest.Id);
                scheduleRequest.TotalLikes = totalLikes.Count();
            }

            return new PaginatedList<ScheduleRequest>(scheduleRequests, totalCount, request.Page ?? 1, request.Size ?? 10);
        }

        public async Task<ScheduleRequest?> GetScheduleByIdAsync(int id)
        {
            var schedule = await _unitOfWork.RepositorySchedule.GetById(id);
            if (schedule == null)
            {
                return null;
            }

            var scheduleRequest = _mapper.Map<ScheduleRequest>(schedule);

            var destinations = await _unitOfWork.RepositoryDestination.GetData(d => d.ScheduleId == schedule.Id);
            scheduleRequest.Destinations = _mapper.Map<List<DestinationRequest>>(destinations);

            var totalLikes = await _unitOfWork.RepositoryScheduleLike.GetData(l => l.ScheduleId == schedule.Id);
            scheduleRequest.TotalLikes = totalLikes.Count();

            return scheduleRequest;
        }

        public async Task<List<ScheduleRequest>> GetSchedulesByUserIdAsync(Guid userId)
        {
            Expression<Func<Schedule, bool>> expression = schedule => schedule.UserId == userId;
            var schedules = await _unitOfWork.RepositorySchedule.GetData(expression);

            var scheduleRequests = new List<ScheduleRequest>();

            foreach (var schedule in schedules)
            {
                var scheduleRequest = _mapper.Map<ScheduleRequest>(schedule);

                // Fetch destinations associated with the schedule
                var destinations = await _unitOfWork.RepositoryDestination.GetData(d => d.ScheduleId == schedule.Id);
                scheduleRequest.Destinations = _mapper.Map<List<DestinationRequest>>(destinations);

                // Fetch total likes for the schedule
                var totalLikes = await _unitOfWork.RepositoryScheduleLike.GetData(l => l.ScheduleId == schedule.Id);
                scheduleRequest.TotalLikes = totalLikes.Count();

                scheduleRequests.Add(scheduleRequest);
            }

            return scheduleRequests;
        }

        public async Task<ScheduleRequest> CreateScheduleAsync(CreateScheduleRequest request)
        {

            // Validate StartDate is not in the past
            if (request.StartDate <= DateTime.UtcNow)
            {
                throw new ArgumentException("StartDate cannot be in the past.");
            }

            // Validate StartDate < EndDate
            if (request.StartDate >= request.EndDate)
            {
                throw new ArgumentException("StartDate must be less than EndDate.");
            }

            var schedule = _mapper.Map<Schedule>(request);

            schedule.CreatedDate = DateTime.UtcNow;
            await _unitOfWork.RepositorySchedule.Insert(schedule);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<ScheduleRequest>(schedule);
        }

        public async Task<bool> UpdateScheduleAsync(int id, CreateScheduleRequest request)
        {
            var schedule = await _unitOfWork.RepositorySchedule.GetById(id);
            if (schedule == null) return false;

            // Validate StartDate is not in the past
            if (request.StartDate <= DateTime.UtcNow)
            {
                throw new ArgumentException("StartDate cannot be in the past.");
            }

            // Validate StartDate < EndDate
            if (request.StartDate >= request.EndDate)
            {
                throw new ArgumentException("StartDate must be less than EndDate.");
            }

            _mapper.Map(request, schedule);

            _unitOfWork.RepositorySchedule.Update(schedule);
            schedule.CreatedDate = DateTime.UtcNow;
            await _unitOfWork.CommitAsync();
            return true;
        }

        public async Task<bool> DeleteScheduleAsync(int id)
        {
            var schedule = await _unitOfWork.RepositorySchedule.GetById(id);
            if (schedule == null)
            {
                throw new KeyNotFoundException("Schedule not found.");
            }

            // Delete all related destinations
            var destinations = await _unitOfWork.RepositoryDestination.GetData(d => d.ScheduleId == schedule.Id);
            foreach (var destination in destinations)
            {
                _unitOfWork.RepositoryDestination.Delete(destination);
            }

            _unitOfWork.RepositorySchedule.Delete(schedule);
            await _unitOfWork.CommitAsync();
            return true;
        }

        public async Task<ScheduleRequest?> CloneScheduleFromOtherUserAsync(int scheduleId, Guid newUserId)
        {
            var originalSchedule = await _unitOfWork.RepositorySchedule
                .GetByIdForDestination(scheduleId, includeProperties: "Destinations");

            if (originalSchedule == null)
            {
                return null;  // If no schedule found, return null
            }

            var clonedSchedule = new Schedule
            {
                UserId = newUserId,
                ScheduleName = $"{originalSchedule.ScheduleName} - Copy",
                StartDate = originalSchedule.StartDate,
                EndDate = originalSchedule.EndDate,
                CreatedDate = DateTime.UtcNow,
                Status = originalSchedule.Status,
                IsPublic = originalSchedule.IsPublic
            };

            await _unitOfWork.RepositorySchedule.Insert(clonedSchedule);
            await _unitOfWork.CommitAsync();

            if (originalSchedule.Destinations != null && originalSchedule.Destinations.Any())
            {
                foreach (var destination in originalSchedule.Destinations)
                {
                    var clonedDestination = new Destination
                    {
                        ScheduleId = clonedSchedule.Id,
                        PlaceId = destination.PlaceId,
                        StartDate = destination.StartDate,
                        EndDate = destination.EndDate,
                        Detail = destination.Detail,
                        IsArrived = destination.IsArrived
                    };

                    await _unitOfWork.RepositoryDestination.Insert(clonedDestination);
                }
            }

            await _unitOfWork.CommitAsync();

            return _mapper.Map<ScheduleRequest>(clonedSchedule);
        }

        public async Task<ScheduleRequest> SaveSuggestedSchedule(ScheduleWithDestinationsRequest request, Guid userId)
        {
            // Validate StartDate < EndDate
            if (request.StartDate >= request.EndDate)
            {
                throw new ArgumentException("StartDate must be less than EndDate.");
            }

            // Validate StartDate is not in the past
            if (request.StartDate < DateTime.UtcNow)
            {
                throw new ArgumentException("StartDate cannot be in the past.");
            }

            var newSchedule = new Schedule
            {
                UserId = userId,
                ScheduleName = request.ScheduleName,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                CreatedDate = DateTime.UtcNow,
                IsPublic = request.IsPublic
            };

            await _unitOfWork.RepositorySchedule.Insert(newSchedule);
            await _unitOfWork.CommitAsync();

            foreach (var destination in request.Destinations)
            {
                // Validate PlaceId exists
                var place = await _unitOfWork.RepositoryPlace.GetById(destination.PlaceId);
                if (place == null)
                {
                    throw new ArgumentException($"Place with Id {destination.PlaceId} does not exist.");
                }

                var newDestination = new Destination
                {
                    ScheduleId = newSchedule.Id,
                    PlaceId = destination.PlaceId,
                    StartDate = destination.StartDate,
                    EndDate = destination.EndDate,
                    IsArrived = destination.IsArrived
                };

                await _unitOfWork.RepositoryDestination.Insert(newDestination);
            }

            await _unitOfWork.CommitAsync();

            return _mapper.Map<ScheduleRequest>(newSchedule);
        }
    }
}
