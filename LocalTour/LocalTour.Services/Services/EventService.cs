using AutoMapper;
using LocalTour.Data.Abstract;
using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;
using LocalTour.Services.Extensions;
using LocalTour.Services.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.Services
{
    public class EventService : IEventService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFileService _fileService;
        public EventService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, IFileService fileService)
        {
            _fileService = fileService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<PaginatedList<EventResponseVM>> GetAllEventByPlaceid(int placeid, GetEventRequest request)
        {
            var events = _unitOfWork.RepositoryEvent.GetAll()
                                                    .Where(e => e.PlaceId == placeid)
                                                    .AsQueryable();

            if (request.SearchTerm is not null)
            {
                events = events.Where(e => e.EventName.Contains(request.SearchTerm) ||
                                           e.Description.Contains(request.SearchTerm));
            }

            if (!string.IsNullOrEmpty(request.SortBy))
            {
                events = events.OrderByCustom(request.SortBy, request.SortOrder);
            }

            return await events
                .ListPaginateWithSortAsync<Event, EventResponseVM>(
                request.Page,
                request.Size,
                request.SortBy,
                request.SortOrder,
                _mapper.ConfigurationProvider);
        }

        public async Task<PaginatedList<EventResponse>> GetAllEventByVisitor(int placeid, GetEventRequest request)
        {
            var events = _unitOfWork.RepositoryEvent.GetAll()
                .Where(e => e.PlaceId == placeid && e.EventStatus == "Approved")
                .AsQueryable();

            if (request.SearchTerm is not null)
            {
                events = events.Where(e => e.EventName.Contains(request.SearchTerm) ||
                                           e.Description.Contains(request.SearchTerm));
            }

            if (!string.IsNullOrEmpty(request.SortBy))
            {
                events = events.OrderByCustom(request.SortBy, request.SortOrder);
            }

            return await events
                .ListPaginateWithSortAsync<Event, EventResponse>(
                    request.Page,
                    request.Size,
                    request.SortBy,
                    request.SortOrder,
                    _mapper.ConfigurationProvider);
        }

        public async Task<PaginatedList<EventSearchResponse>> SearchEvent(int? placeId, GetEventRequest request)  
        {
            var events = _unitOfWork.RepositoryEvent.GetAll()
                .Include(e => e.Place)
                .ThenInclude(ee => ee.PlaceTranslations)
                .ToList();
            events = events.Where(x => x.EventStatus == "Approved").ToList();

            if (placeId is not null)
            {
                events = events.Where(e => e.PlaceId == placeId).ToList();
            }
            
            if (request.SearchTerm is not null)
            {
                events = events.Where(e => e.EventName.ToLower().Contains(request.SearchTerm.ToLower()) 
                        || e.Description.ToLower().Contains(request.SearchTerm)
                        || e.Place.PlaceTranslations.Any(x => x.Name.ToLower().Contains(request.SearchTerm.ToLower()))).ToList();
            }
            events = events.Where(e => e.Place.PlaceTranslations.Any(x => x.LanguageCode == request.languageCode)).ToList();

            List<EventSearchResponse> eventSearchResponses = events.Select(x =>
            new EventSearchResponse
            {
                EventName = x.EventName,
                Description = x.Description,
                PlaceId = x.PlaceId,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                EventStatus = x.EventStatus,
                Longitude = x.Place.Longitude,
                Latitude = x.Place.Latitude,
                EventPhoto = x.EventPhotoDisplay,
                PlaceName = x.Place.PlaceTranslations.FirstOrDefault().Name,
                Distance = CalculateDistance(request.latitude, request.longitude, x.Place.Latitude, x.Place.Longitude)
            }).ToList();
            

            if (request.SortBy is not null)
            {
                if (request.SortBy.ToLower() == "distance")
                {
                    if (request.SortOrder.ToLower() == "asc")
                    {
                        eventSearchResponses = eventSearchResponses.OrderBy(x => x.Distance).ToList();
                    }
                    else
                    {
                        eventSearchResponses = eventSearchResponses.OrderByDescending(x => x.Distance).ToList();
                    }
                }

                if (request.SortOrder.ToLower() == "created_by")
                {
                    if (request.SortOrder.ToLower() == "asc")
                    {
                        eventSearchResponses = eventSearchResponses.OrderByDescending(x => x.StartDate).ToList();
                    }
                    else
                    {
                        eventSearchResponses = eventSearchResponses.OrderBy(x => x.StartDate).ToList();
                    }
                }
            }
            int length = eventSearchResponses.Count;
            if (request.Size != null && request.Page != null)
            {
                int pageSize = request.Size??1;
                int pageNumber = request.Page ?? 10;
                eventSearchResponses = eventSearchResponses.
                    Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
            }

            var result = new PaginatedList<EventSearchResponse>(eventSearchResponses, eventSearchResponses.Count, request.Page ?? 1, length);
            return result;
        }

        public async Task<PaginatedList<EventViewModel>> GetAllEvent(GetEventRequest request)
        {
            var user = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(user) || !Guid.TryParse(user, out var userId))
            {
                throw new UnauthorizedAccessException("User not found or invalid User ID.");
            }
            var events = _unitOfWork.RepositoryEvent.GetAll()
                        .Include(e => e.Place)
                        .ThenInclude(p => p.PlaceTranslations)
                        .Include(e => e.Place)
                        .ThenInclude(p => p.Ward)
                        .ThenInclude(w => w.DistrictNcity)
                        .AsQueryable();
            var userTags =  _unitOfWork.RepositoryModTag.GetAll()
                    .Where(mt => mt.UserId == userId)
                    .Select(mt => mt.DistrictNcityId)
                    .ToList();
            events = events.Where(e => userTags.Contains(e.Place.Ward.DistrictNcityId)).AsQueryable();
            if (request.SearchTerm is not null)
            {
                events = events.Where(e => e.EventName.Contains(request.SearchTerm) ||
                                           e.Description.Contains(request.SearchTerm)).AsQueryable();
            }

            if (request.status != null)
            {
                events = events.Where(e => e.EventStatus.Contains(request.status)).AsQueryable();
            }

            if (!string.IsNullOrEmpty(request.SortBy))
            {
                events = events.OrderByCustom(request.SortBy, request.SortOrder);
            }

            return await events.ListPaginateWithSortAsync<Event, EventViewModel>(
                request.Page,
                request.Size,
                request.SortBy,
                request.SortOrder,
                _mapper.ConfigurationProvider);
        }
        public async Task<EventRequest> CreateEvent(int placeid, EventRequest request)
        {
            var places = await _unitOfWork.RepositoryPlace.GetById(placeid);
            if (places == null)
            {
                throw new ArgumentException($"Place with id {placeid} not found.");
            }
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            
            
            var events = new Event
            {
                EventName = request.EventName,
                Description = request.Description,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                EventStatus = "Pending",
                CreatedAt = DateTime.Now,
                //UpdatedAt = DateTime.Now,
                PlaceId = placeid,
            };
            
            if (request.EventPhoto != null)
            {
                var photoUrl = await _fileService.SaveImageFile(request.EventPhoto);
                events.EventPhotoDisplay = photoUrl.Data;
            }
            
            await _unitOfWork.RepositoryEvent.Insert(events);
            await _unitOfWork.CommitAsync();
            return request;
        }

        public async Task<EventRequest> UpdateEvent(int placeid, int eventid, EventRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            var place = await _unitOfWork.RepositoryPlace.GetById(placeid);
            if (place == null)
            {
                throw new ArgumentException($"Place with id {placeid} not found.");
            }
            var existingEvent = await _unitOfWork.RepositoryEvent.GetById(eventid);
            if (existingEvent == null)
            {
                throw new ArgumentException($"Event with id {eventid} not found.");
            }
            if (existingEvent.PlaceId != placeid)
            {
                throw new InvalidOperationException($"Event with id {eventid} does not belong to place with id {placeid}.");
            }

            existingEvent.EventName = request.EventName;
            existingEvent.Description = request.Description;
            existingEvent.StartDate = request.StartDate;
            existingEvent.EndDate = request.EndDate;
            existingEvent.EventStatus = "Pending";
            existingEvent.UpdatedAt = DateTime.Now;
            
            if (request.EventPhoto != null)
            {
                var photoUrl = await _fileService.SaveImageFile(request.EventPhoto);
                existingEvent.EventPhotoDisplay = photoUrl.Data;
            }
            
            _unitOfWork.RepositoryEvent.Update(existingEvent);
            await _unitOfWork.CommitAsync();
            return request;
        }

        public async Task<Event> GetEventById(int placeid, int eventid)
        {
            var place = await _unitOfWork.RepositoryPlace.GetById(placeid);
            if (place == null)
            {
                throw new KeyNotFoundException($"Place with ID {placeid} not found.");
            }

            var eventEntity = await _unitOfWork.RepositoryEvent.GetAll()
                .FirstOrDefaultAsync(e => e.Id == eventid && e.PlaceId == placeid);

            if (eventEntity == null)
            {
                throw new KeyNotFoundException($"Event with ID {eventid} for Place ID {placeid} not found.");
            }

            return eventEntity;
        }
        public async Task<bool> DeleteEvent(int placeid, int eventid)
        {
            var places = await _unitOfWork.RepositoryPlace.GetById(placeid);
            if (places == null)
            {
                throw new ArgumentException($"Place with id {placeid} not found.");
            }
            var eventEntity = await _unitOfWork.RepositoryEvent.GetById(eventid);
            if (eventEntity != null)
            {
                _unitOfWork.RepositoryEvent.Delete(eventEntity);
            }
            await _unitOfWork.CommitAsync();
            return true;

        }
        public async Task<Event> ChangeStatusEvent(int placeid, int eventid, string status)
        {
            var existingPlace = await _unitOfWork.RepositoryPlace.GetById(placeid);
            if (existingPlace == null)
            {
                throw new ArgumentException($" {placeid} not found.");
            }
            var existingEvent = await _unitOfWork.RepositoryEvent.GetById(eventid);
            if (existingEvent == null)
            {
                throw new ArgumentException($" {eventid} not found.");
            }
            existingEvent.EventStatus = status;
            _unitOfWork.RepositoryEvent.Update(existingEvent);
            await _unitOfWork.CommitAsync();
            return existingEvent;
        }
        
        public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371; // Radius of Earth in kilometers
            var dLat = (lat2 - lat1) * Math.PI / 180;
            var dLon = (lon2 - lon1) * Math.PI / 180;
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c; // Distance in kilometers
        }
    }
}
