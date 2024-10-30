using LocalTour.Services.ViewModel;

namespace LocalTour.Services.Abstract;

public interface ITraveledPlaceService
{
    Task<bool> AddTraveledPlace(string phoneNumber, int placeId);
    Task<int> CountTraveledPlace(string phoneNumber,int placeId);
    Task<List<(int,int)>> CountTraveledPlaces(string phoneNumber, List<int> placeIds);
    Task<List<TraveledPlaceVM>> GetAllTraveledPlaces(string phoneNumber);
}