using LocalTour.Services.ViewModel;

namespace LocalTour.Services.Abstract;

public interface IMarkPlaceService
{
    Task<bool> CreateMarkPlace(string userId, int placeId);
    Task<List<MarkPlaceVM>> GetMarkPlaces(string userId,string languageCode);
    Task<bool> UpdateMarkPlace(string userId, int placeId, bool isVisited);
    Task<bool> DeleteMarkPlace(string userId, int placeId);
}