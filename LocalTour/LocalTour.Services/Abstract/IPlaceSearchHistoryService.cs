using LocalTour.Domain.Entities;
using LocalTour.Services.Model;

namespace LocalTour.Services.Abstract;

public interface IPlaceSearchHistoryService
{
    Task<ServiceResponseModel<bool>> AddPlaceSearchHistory(string userId, int placeId);
    Task<ServiceResponseModel<List<PlaceSearchHistory>>> GetAllPlaceSearchHistory(string userId,int? pageNumber, int? pageSize, string languageCode);
    Task<ServiceResponseModel<bool>> DeletePlaceSearchHistory(string userId, int placeId);
    Task<ServiceResponseModel<bool>> DeleteAllPlaceSearchHistory(string userId);
    Task<bool> UpdatePlaceSearchHistory(string userId, int placeId);
}