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
        public EventService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<PaginatedList<EventRequest>> GetAllEventByPlaceid(int placeid, GetEventRequest request)
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
                .ListPaginateWithSortAsync<Event, EventRequest>(
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
                        .Include(e => e.Place.PlaceTags)
                        .AsQueryable();
            var userTags = await _unitOfWork.RepositoryModTag.GetAll()
                    .Where(mt => mt.UserId == userId)
                    .Select(mt => mt.TagId)
                    .ToListAsync();
            events = events.Where(e => e.Place.PlaceTags.Any(pt => userTags.Contains(pt.TagId)));
            if (request.SearchTerm is not null)
            {
                events = events.Where(e => e.EventName.Contains(request.SearchTerm) ||
                                           e.Description.Contains(request.SearchTerm));
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
    }
}
