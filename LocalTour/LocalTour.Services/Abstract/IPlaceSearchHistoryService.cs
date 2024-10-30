using LocalTour.Domain.Entities;

namespace LocalTour.Services.Abstract;

public interface IPlaceSearchHistoryService
{
    Task<bool> AddPlaceSearchHistory(string userId, int placeId);
    Task<List<PlaceSearchHistory>> GetAllPlaceSearchHistory(string phoneNumber,int? pageNumber, int? pageSize, string languageCode);
    Task<bool> DeletePlaceSearchHistory(string userId, int placeId);
    Task<bool> DeleteAllPlaceSearchHistory(string userId);
    Task<bool> UpdatePlaceSearchHistory(string userId, int placeId);
}