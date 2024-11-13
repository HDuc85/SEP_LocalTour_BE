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

            _mapper.Map(request, schedule);

            _unitOfWork.RepositorySchedule.Update(schedule);
            schedule.CreatedDate = DateTime.UtcNow;
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

        public async Task<ScheduleRequest?> CloneScheduleFromOtherUserAsync(int scheduleId, Guid newUserId)
        {
            // Lấy lịch trình gốc cùng với các Destinations
            var originalSchedule = await _unitOfWork.RepositorySchedule
                .GetByIdForDestination(scheduleId, includeProperties: "Destinations");

            if (originalSchedule == null)
            {
                return null;  // Nếu không tìm thấy lịch trình gốc, trả về null
            }

            // Tạo bản sao của lịch trình với thông tin người dùng mới
            var clonedSchedule = new Schedule
            {
                UserId = newUserId, // Gán UserId của người dùng mới
                ScheduleName = $"{originalSchedule.ScheduleName} - Copy",  // Thêm "Copy" vào tên lịch trình
                StartDate = originalSchedule.StartDate,
                EndDate = originalSchedule.EndDate,
                CreatedDate = DateTime.UtcNow,  // Thời gian tạo mới
                Status = originalSchedule.Status,
                IsPublic = originalSchedule.IsPublic
            };

            // Lưu bản sao của lịch trình vào cơ sở dữ liệu
            await _unitOfWork.RepositorySchedule.Insert(clonedSchedule);
            await _unitOfWork.CommitAsync(); // Lưu thay đổi vào cơ sở dữ liệu

            // Clone các Destinations từ lịch trình gốc sang lịch trình mới
            if (originalSchedule.Destinations != null && originalSchedule.Destinations.Any())
            {
                // Nếu có Destinations, sao chép chúng
                foreach (var destination in originalSchedule.Destinations)
                {
                    var clonedDestination = new Destination
                    {
                        ScheduleId = clonedSchedule.Id,  // Gán ScheduleId mới của lịch trình sao chép
                        PlaceId = destination.PlaceId,
                        StartDate = destination.StartDate,
                        EndDate = destination.EndDate,
                        Detail = destination.Detail,
                        IsArrived = destination.IsArrived
                    };

                    // Lưu bản sao của Destination vào cơ sở dữ liệu
                    await _unitOfWork.RepositoryDestination.Insert(clonedDestination);
                }
            }
            else
            {
                // Nếu không có Destinations, tạo một danh sách rỗng
                clonedSchedule.Destinations = new List<Destination>();
            }

            // Commit tất cả các thay đổi, bao gồm cả lịch trình và destinations
            await _unitOfWork.CommitAsync();

            // Map bản sao của lịch trình sang DTO ScheduleRequest và trả về
            return _mapper.Map<ScheduleRequest>(clonedSchedule);
        }

    }
}
