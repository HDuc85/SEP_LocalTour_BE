using AutoMapper;
using LocalTour.Data.Abstract;
using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;
using LocalTour.Services.ViewModel;
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
            
            var places =  _unitOfWork.RepositoryPlace.GetDataQueryable().Include(y => y.PlaceTags).ToList();

            places = places.Where(place => CalculateDistance(request.userLatitude,
                request.userLongitude,
                place.Latitude,
                place.Longitude) <= maxRange).ToList();
            if (places.Count() == null)
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

    }
}
