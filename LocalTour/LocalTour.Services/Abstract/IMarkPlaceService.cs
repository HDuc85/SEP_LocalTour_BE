using LocalTour.Services.Model;
using LocalTour.Services.ViewModel;

namespace LocalTour.Services.Abstract;

public interface IMarkPlaceService
{
    Task<ServiceResponseModel<bool>> CreateMarkPlace(string userId, int placeId);
    Task<ServiceResponseModel<List<MarkPlaceVM>>> GetMarkPlaces(string userId,string languageCode);
    Task<ServiceResponseModel<bool>> UpdateMarkPlace(string userId, int placeId, bool isVisited);
    Task<ServiceResponseModel<bool>> DeleteMarkPlace(string userId, int placeId);
}