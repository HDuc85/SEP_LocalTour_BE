using LocalTour.Services.ViewModel;

namespace LocalTour.Services.Abstract;

public interface IMarkPlaceService
{
    Task<bool> CreateMarkPlace(string phoneNumber, int placeId);
    Task<List<MarkPlaceVM>> GetMarkPlaces(string phoneNumber,string languageCode);
    Task<bool> UpdateMarkPlace(string phoneNumber, int placeId, bool isVisited);
    Task<bool> DeleteMarkPlace(string phoneNumber, int placeId);
}