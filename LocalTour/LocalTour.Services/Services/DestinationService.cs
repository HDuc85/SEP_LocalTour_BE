using AutoMapper;
using LocalTour.Data.Abstract;
using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;
using LocalTour.Services.Extensions;
using LocalTour.Services.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LocalTour.Services.Services
{
    public class DestinationService : IDestinationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<DestinationService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DestinationService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, ILogger<DestinationService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<List<DestinationRequest>> GetAllDestinations(string languageCode)
        {
            var destinationsQuery = _unitOfWork.RepositoryDestination.GetAll()
                .Include(d => d.Place)
                .ThenInclude(p => p.PlaceTranslations)
                .Include(d => d.Place) // Ensure Place is included to get placePhotoDisplay
                .AsQueryable();

            // Filter by language code if provided
            if (!string.IsNullOrEmpty(languageCode))
            {
                destinationsQuery = destinationsQuery.Where(d =>
                    d.Place.PlaceTranslations.Any(pt => pt.LanguageCode == languageCode));
            }

            // Sort by StartDate from earliest to latest
            destinationsQuery = destinationsQuery.OrderBy(d => d.StartDate);

            // Project to DestinationRequest and fetch PlaceName and PlacePhotoDisplay
            var destinations = await destinationsQuery
                .Select(d => new DestinationRequest
                {
                    Id = d.Id,
                    ScheduleId = d.ScheduleId,
                    PlaceId = d.PlaceId,
                    PlaceName = d.Place.PlaceTranslations
                        .Where(pt => pt.LanguageCode == languageCode)
                        .Select(pt => pt.Name)
                        .FirstOrDefault() ?? "Name not available",
                    PlacePhotoDisplay = d.Place.PhotoDisplay,
                    StartDate = d.StartDate,
                    EndDate = d.EndDate,
                    Detail = d.Detail,
                    IsArrived = d.IsArrived
                })
                .ToListAsync();

            return destinations;
        }

        public async Task<List<DestinationRequest>> GetAllDestinationsByScheduleId(int scheduleId, string? languageCode)
        {
            var destinationsQuery = _unitOfWork.RepositoryDestination.GetAll()
                .Where(d => d.ScheduleId == scheduleId)
                .Include(d => d.Place)
                .ThenInclude(p => p.PlaceTranslations)
                .Include(d => d.Place) // Đảm bảo Place được bao gồm để lấy placePhotoDisplay
                .AsQueryable();

            var destinationRequests = await destinationsQuery
                .Select(d => new DestinationRequest
                {
                    Id = d.Id,
                    ScheduleId = d.ScheduleId,
                    PlaceId = d.PlaceId,
                    StartDate = d.StartDate,
                    EndDate = d.EndDate,
                    Detail = d.Detail,
                    IsArrived = d.IsArrived,
                    PlaceName = d.Place.PlaceTranslations
                        .Where(pt => pt.LanguageCode == languageCode)
                        .Select(pt => pt.Name)
                        .FirstOrDefault() ?? "Name not available",
                    PlacePhotoDisplay = d.Place.PhotoDisplay
                })
                .ToListAsync();

            return destinationRequests;
        }

        public async Task<DestinationRequest> GetDestinationById(int id, string? languageCode)
        {
            var destination = await _unitOfWork.RepositoryDestination.GetAll()
                .Include(d => d.Place)
                .ThenInclude(p => p.PlaceTranslations)
                .Include(d => d.Place) // Ensure Place is included to get placePhotoDisplay
                .FirstOrDefaultAsync(d => d.Id == id);

            if (destination == null) return null;

            // Filter place name based on language code
            var translation = destination.Place.PlaceTranslations
                .FirstOrDefault(pt => pt.LanguageCode == languageCode);

            var destinationRequest = _mapper.Map<DestinationRequest>(destination);
            destinationRequest.PlaceName = translation?.Name ?? "Name not available";
            destinationRequest.PlacePhotoDisplay = destination.Place.PhotoDisplay;

            return destinationRequest;
        }

        public async Task<Destination> CreateDestinationAsync(CreateDestinationRequest request)
        {
            // Ensure StartDate is not in the future
            if (request.StartDate > DateTime.UtcNow)
            {
                throw new ArgumentException("StartDate cannot be in the future.");
            }

            // Ensure EndDate is greater than or equal to StartDate
            if (request.EndDate < request.StartDate)
            {
                throw new ArgumentException("EndDate cannot be earlier than StartDate.");
            }

            // Check if the place exists
            var place = await _unitOfWork.RepositoryPlace.GetById(request.PlaceId);
            if (place == null)
            {
                throw new ArgumentException("Place with the given PlaceId does not exist.");
            }

            // Validate StartDate and EndDate with the schedule
            var schedule = await _unitOfWork.RepositorySchedule.GetById(request.ScheduleId);
            if (schedule == null)
            {
                throw new Exception("Schedule not found.");
            }

            if (request.StartDate < schedule.StartDate || request.EndDate > schedule.EndDate)
            {
                throw new ArgumentException("Destination dates must fall within the schedule dates.");
            }

            // Get the UserId from the token
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Check if the Schedule belongs to the logged-in user
            if (schedule.UserId.ToString() != userId)
            {
                throw new UnauthorizedAccessException("You do not have permission to create a destination for this schedule.");
            }

            // Map and save the destination
            var destination = _mapper.Map<Destination>(request);
            await _unitOfWork.RepositoryDestination.Insert(destination);
            await _unitOfWork.CommitAsync();

            return destination;
        }

        public async Task<bool> UpdateDestinationAsync(int id, CreateDestinationRequest request)
        {
            var destination = await _unitOfWork.RepositoryDestination.GetById(id);
            if (destination == null) return false;

            // Ensure StartDate is not in the future
            if (request.StartDate > DateTime.UtcNow)
            {
                throw new ArgumentException("StartDate cannot be in the future.");
            }

            // Ensure EndDate is greater than or equal to StartDate
            if (request.EndDate < request.StartDate)
            {
                throw new ArgumentException("EndDate cannot be earlier than StartDate.");
            }

            // Validate StartDate and EndDate with the schedule
            var schedule = await _unitOfWork.RepositorySchedule.GetById(request.ScheduleId);
            if (schedule == null)
            {
                throw new Exception("Schedule not found.");
            }

            if (request.StartDate < schedule.StartDate || request.EndDate > schedule.EndDate)
            {
                throw new ArgumentException("Destination dates must fall within the schedule dates.");
            }

            // Check if the place exists
            var place = await _unitOfWork.RepositoryPlace.GetById(request.PlaceId);
            if (place == null)
            {
                throw new ArgumentException("Place with the given PlaceId does not exist.");
            }

            _mapper.Map(request, destination);

            _unitOfWork.RepositoryDestination.Update(destination);
            await _unitOfWork.CommitAsync();

            return true;
        }

        public async Task<bool> DeleteDestinationAsync(int id)
        {
            var destination = await _unitOfWork.RepositoryDestination.GetById(id);
            if (destination == null) return false;

            _unitOfWork.RepositoryDestination.Delete(destination);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
