using AutoMapper;
using LocalTour.Data.Abstract;
using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;
using LocalTour.Services.Extensions;
using LocalTour.Services.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.Services
{
    public class EventService : IEventService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public EventService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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
                    EventStatus = request.EventStatus,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
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
            existingEvent.EventStatus = request.EventStatus;
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
    }
}
