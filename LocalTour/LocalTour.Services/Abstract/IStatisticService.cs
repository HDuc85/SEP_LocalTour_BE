using LocalTour.Services.ViewModel;

namespace LocalTour.Services.Abstract;

public interface IStatisticService
{
    Task<Dictionary<int, int>> GetUserRegistrationByMonthAsync(int year);
    Task<int> GetTotalSuccessfulTravelsAsync(DateTime startDate, DateTime endDate);
    Task<int> GetTotalSchedulesCreatedAsync(DateTime startDate, DateTime endDate);
    Task<int> GetTotalPostsCreatedAsync(DateTime startDate, DateTime endDate);
    Task<List<GetStatsticReponse>> GetModApprovedPlaceByMonthAsync(int year);
    Task<StatsticMonth> GetModApprovedByMonthAsync(int year, string userId);
    Task<int> GetTotalModApprovedAsync(String userId);
    Task<List<PlaceGetStatsticReponse>> GetPlaceByMonthAsync(int year);
    Task<PlaceStatsticMonth> GetPlaceByMonthAsync(int year, string userId);
    Task<PlaceGetStatsticReponse> GetTotalPlaceAsync(String userId);
}