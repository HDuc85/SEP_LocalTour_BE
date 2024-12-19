using LocalTour.Services.ViewModel;

namespace LocalTour.Services.Abstract;

public interface IStatisticService
{
  public  Task<Dictionary<int, int>> GetUserRegistrationByMonthAsync(int year);
  public  Task<int> GetTotalSuccessfulTravelsAsync(DateTime startDate, DateTime endDate);
  public  Task<int> GetTotalSchedulesCreatedAsync(DateTime startDate, DateTime endDate);
  public  Task<int> GetTotalPostsCreatedAsync(DateTime startDate, DateTime endDate);
    Task<List<GetStatsticReponse>> GetModApprovedPlaceByMonthAsync(int year);
    Task<StatsticMonth> GetModApprovedByMonthAsync(int year, string userId);
    Task<int> GetTotalModApprovedAsync(String userId);
}