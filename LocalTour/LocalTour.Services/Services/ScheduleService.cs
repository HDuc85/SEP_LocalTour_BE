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
using LocalTour.Services.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LocalTour.Services.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private double ToRadians(double deg) => deg * (Math.PI / 180);

        public ScheduleService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<PaginatedList<ScheduleRequest>> GetAllSchedulesAsync(GetScheduleRequest request, String userId)
        {
            var schedulesQuery = _unitOfWork.RepositorySchedule.GetAll()
                .Include(s => s.ScheduleLikes)
                .Include(s => s.Destinations)
                .ThenInclude(x => x.Place)
                .ThenInclude(y => y.PlaceTranslations)
                .ToList();

            if (request.UserId != null)
            {
                schedulesQuery = schedulesQuery.Where(s => s.UserId == request.UserId).ToList();
            }
     
            if (userId != null)
            {
                if (userId != request.UserId.ToString())
                {
                    schedulesQuery = schedulesQuery.Where(x => x.IsPublic == true).ToList();
                }
            }

            if (request.SortBy == "liked")
            {
                schedulesQuery = request.SortOrder == "asc"
                    ? schedulesQuery.OrderBy(s => s.ScheduleLikes.Count).ToList()
                    : schedulesQuery.OrderByDescending(s => s.ScheduleLikes.Count).ToList();
            }
            else if (request.SortBy == "created_by")
            {
                schedulesQuery = request.SortOrder == "asc"
                    ? schedulesQuery.OrderBy(s => s.CreatedDate).ToList()
                    : schedulesQuery.OrderByDescending(s => s.CreatedDate).ToList();
            }

            var totalCount = schedulesQuery.Count;
            var items =  schedulesQuery
                .Skip((request.Page - 1) * request.Size ?? 0)
                .Take(request.Size ?? 10)
                .ToList();

            // Map the result to ScheduleRequest DTO
           // var scheduleRequests = _mapper.Map<List<ScheduleRequest>>(items);
            var author = await _unitOfWork.RepositoryUser.GetById(request.UserId);
            List<ScheduleRequest> scheduleRequests = new List<ScheduleRequest>();
            foreach (var scheduleRequest in items)
            {
                scheduleRequests.Add( new ScheduleRequest()
                {
                    Id = scheduleRequest.Id,
                    CreatedDate = scheduleRequest.CreatedDate,
                    UserName = author.UserName,
                    ScheduleName = scheduleRequest.ScheduleName,
                    StartDate = scheduleRequest.StartDate,
                    EndDate = scheduleRequest.EndDate,
                    IsPublic = scheduleRequest.IsPublic,
                    Status = scheduleRequest.Status,
                    TotalLikes = scheduleRequest.ScheduleLikes.Count,
                    UserProfileImage = author.ProfilePictureUrl,
                    UserId = author.Id,
                    IsLiked = userId != null ? schedulesQuery.Any(x => x.ScheduleLikes.Any(y => y.ScheduleId == scheduleRequest.Id && y.UserId.ToString() == userId)) : false,
                    Destinations = scheduleRequest.Destinations.Select(x => new DestinationRequest()
                    {
                        PlaceId = x.PlaceId,
                        StartDate = x.StartDate,
                        EndDate = x.EndDate,
                        Detail = x.Detail,
                        IsArrived = x.IsArrived,
                        ScheduleId = x.ScheduleId,
                        Id = x.Id,
                        PlaceName = x.Place.PlaceTranslations.FirstOrDefault(y => y.LanguageCode == request.languageCode).Name,
                        PlacePhotoDisplay = x.Place.PhotoDisplay
                    }).ToList(),
                });
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

        public async Task<ServiceResponseModel<List<DestinationVM>>> GenerateSchedule(SuggestScheduleRequest request, string userId)
        {
            List<DestinationVM> schedule = new List<DestinationVM>();
            var userTags = await _unitOfWork.RepositoryUserPreferenceTags.GetData(x => x.UserId == Guid.Parse(userId));
            if (!userTags.Any())
            {
                return new ServiceResponseModel<List<DestinationVM>>(false, "There are no users tag provided");
            }
            var userTagIds = userTags.Select(ut => ut.TagId).ToList();
            int maxRange = int.Parse(_configuration.GetValue<int>("SuggestPlace:Distance").ToString());

            var places = _unitOfWork.RepositoryPlace.GetDataQueryable().Include(y => y.PlaceTags).ToList();

            places = places.Where(place => CalculateDistance(request.userLatitude,
                request.userLongitude,
                place.Latitude,
                place.Longitude) <= maxRange).ToList();
            if (places.Count() == 0)
            {
                return new ServiceResponseModel<List<DestinationVM>>(false, "No places to suggest for user");
            }
            else
            {
                var timeSlots = _configuration.GetSection("SuggestPlace:TimeSlots").Get<List<ScheduleTimeSlotSuggest>>();

                HashSet<int> selectedPlaceIds = new HashSet<int>();

                var placesWithMatchCount = places
                    .Select(place => new
                    {
                        Place = place,
                        MatchCount = place.PlaceTags.Count(tag => userTagIds.Contains(tag.TagId))
                    }).ToList();


                for (int day = 0; day < request.days; day++)
                {
                    DateTime dayStart = request.startDate.Date.AddDays(day);

                    foreach (var timeSlot in timeSlots)
                    {
                        var selectedPlace = placesWithMatchCount
                            .Where(p => p.Place.TimeOpen.ToTimeSpan() <= dayStart.AddHours(timeSlot.Start).TimeOfDay &&
                                        p.Place.TimeClose.ToTimeSpan() >= dayStart.AddHours(timeSlot.End).TimeOfDay &&
                                        !selectedPlaceIds.Contains(p.Place.Id))
                            .OrderByDescending(p => p.MatchCount)
                            .ThenBy(_ => Guid.NewGuid())
                            .Select(p => p.Place)
                            .FirstOrDefault();

                        if (selectedPlace != null)
                        {
                            selectedPlaceIds.Add(selectedPlace.Id);
                            schedule.Add(new DestinationVM()
                            {
                                PlaceId = selectedPlace.Id,
                                StartDate = dayStart.AddHours(timeSlot.Start),
                                EndDate = dayStart.AddHours(timeSlot.End),
                                IsArrived = false
                            });
                        }
                    }
                }

                return new ServiceResponseModel<List<DestinationVM>>(true, schedule);
            }


        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            // Haversine formula to calculate distance in kilometers between two lat/lon points
            double R = 6371; // Radius of the earth in km
            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distance = R * c; // Distance in km
            return distance;
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
