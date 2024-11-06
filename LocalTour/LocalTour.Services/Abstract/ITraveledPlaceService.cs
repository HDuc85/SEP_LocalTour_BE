using LocalTour.Services.ViewModel;

namespace LocalTour.Services.Abstract;

public interface ITraveledPlaceService
{
    Task<bool> AddTraveledPlace(string userId, int placeId);
    Task<int> CountTraveledPlace(string userId,int placeId);
    Task<List<(int,int)>> CountTraveledPlaces(string userId, List<int> placeIds);
    Task<List<TraveledPlaceVM>> GetAllTraveledPlaces(string userId);
}