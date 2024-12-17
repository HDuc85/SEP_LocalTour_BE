using LocalTour.Services.ViewModel;

namespace LocalTour.Services.Abstract;

public interface IModCheckService
{
    Task<PaginatedList<ModCheckReponse>> GetAllModChecks(GetAllModRequest queryParams);
    Task<bool> CreateModCheck(CreateModCheckRequest request, string userId);
    Task<bool> Delete(int placeId);
}