using LocalTour.Data.Abstract;
using LocalTour.Services.Abstract;
using LocalTour.Services.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LocalTour.Services.Services;

public class StatisticService : IStatisticService
{
    readonly private IUnitOfWork _unitOfWork;
    readonly private IConfiguration _configuration; 

    public StatisticService(IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

   
    public async Task<Dictionary<int, int>> GetUserRegistrationByMonthAsync(int year)
    {
        var registrationByMonth = new Dictionary<int, int>();

        for (int month = 1; month <= 12; month++)
        {
            registrationByMonth[month] = 0;
        }

        var users = await _unitOfWork.RepositoryUser.GetDataQueryable()
            .Where(u => u.DateCreated.Year == year)
            .GroupBy(u => u.DateCreated.Month)
            .Select(group => new 
            {
                Month = group.Key,
                Count = group.Count()
            })
            .ToListAsync();

        foreach (var userGroup in users)
        {
            registrationByMonth[userGroup.Month] = userGroup.Count;
        }

        return registrationByMonth;
    }

    public async Task<int> GetTotalSuccessfulTravelsAsync(DateTime startDate, DateTime endDate)
    {
        var total = await _unitOfWork.RepositoryTraveledPlace
            .GetDataQueryable(x => x.TimeArrive >= startDate && x.TimeArrive <= endDate)
            .CountAsync();
        return total;
    }

    public async Task<int> GetTotalSchedulesCreatedAsync(DateTime startDate, DateTime endDate)
    {
        var total = await _unitOfWork.RepositorySchedule
            .GetDataQueryable(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate)
            .CountAsync();
        return total;
    }

    public async Task<int> GetTotalPostsCreatedAsync(DateTime startDate, DateTime endDate)
    {
        var total = await _unitOfWork.RepositoryPost
            .GetDataQueryable(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate)
            .CountAsync();
        return total;
    }
    public async Task<List<GetStatsticReponse>> GetModApprovedPlaceByMonthAsync(int year)
    {
        var registrationByMonth = new List<GetStatsticReponse>();
        var users = await _unitOfWork.RepositoryPlace.GetDataQueryable()
            .Where(u => u.ApprovedTime.HasValue && u.ApprovedTime.Value.Year == year)
            .GroupBy(u => u.ApprovedTime.Value.Month)
            .Select(group => new
            {
                Month = group.Key,
                Count = group.Count()
            })
            .ToListAsync();


        foreach (var userGroup in users)
        {
           registrationByMonth.Add(new GetStatsticReponse()
           {
               Month =  userGroup.Month,
               Total = userGroup.Count,
               TotalPrice = _configuration.GetValue<int>("ModCheckPlacePrice") * userGroup.Count,
           });
        }

        return registrationByMonth;
    }
    public async Task<StatsticMonth> GetModApprovedByMonthAsync(int year, string userId)
    {
        var registrationByMonth = new List<GetStatsticReponse>();
        var userCount = await _unitOfWork.RepositoryPlace.GetDataQueryable()
            .Where(u => u.ApprovedTime.HasValue && u.ApprovedTime.Value.Year == year)
            .Where(u => u.ApproverId == Guid.Parse(userId) )
            .GroupBy(u => u.ApprovedTime.Value.Month)
            .Select(group => new
            {
                Month = group.Key,
                Count = group.Count()
            })
            .ToListAsync();
        foreach (var userGroup in userCount)
        {
            registrationByMonth.Add(new GetStatsticReponse()
            {
                Month = userGroup.Month,
                Total = userGroup.Count,
                TotalPrice = _configuration.GetValue<int>("ModCheckPlacePrice") * userGroup.Count,
            });
        }
        var user = await _unitOfWork.RepositoryUser.GetById(Guid.Parse(userId));
        var result = new StatsticMonth()
        {
            Avatar = user.ProfilePictureUrl,
            list = registrationByMonth,
            UserId = user.Id,
            UserName =  user.UserName,
        };
        return result;
    }
    public async Task<int> GetTotalModApprovedAsync(String userId)
    {
        var total = await _unitOfWork.RepositoryPlace.GetDataQueryable()
            .Where(u => u.ApproverId == Guid.Parse(userId))
            .CountAsync();
        return total;
    }
    public async Task<List<PlaceGetStatsticReponse>> GetPlaceByMonthAsync(int year)
    {
        var registrationByMonth = new List<PlaceGetStatsticReponse>();
        var users = await _unitOfWork.RepositoryPlace.GetDataQueryable()
            .Where(u => u.CreatedDate.Value.Year == year)
            .GroupBy(u => u.CreatedDate.Value.Month)
            .ToListAsync();


        foreach (var userGroup in users)
        {
            
           registrationByMonth.Add(new PlaceGetStatsticReponse()
           {
               Month =  userGroup.Key,
               Total = userGroup.Count(),
               TotalPrice = _configuration.GetValue<int>("PayOS:placeRegisterPrice") * userGroup.Count(),
               TotalPlacePaid = userGroup.Count(x => x.Status != "Unpaid"),
               TotalPlaceUnpaid = userGroup.Count(x => x.Status == "Unpaid"),   
           });
        }

        return registrationByMonth;
    }
    public async Task<PlaceStatsticMonth> GetPlaceByMonthAsync(int year, string userId)
    {
        var registrationByMonth = new List<PlaceGetStatsticReponse>();
        var userCount = await _unitOfWork.RepositoryPlace.GetDataQueryable()
            .Where(u => u.CreatedDate.Value.Year == year)
            .Where(u => u.AuthorId == Guid.Parse(userId))
            .GroupBy(u => u.CreatedDate.Value.Month)
            .ToListAsync();

        foreach (var userGroup in userCount)
        {
            registrationByMonth.Add(new PlaceGetStatsticReponse()
            {
                Month = userGroup.Key,
                Total = userGroup.Count(),
                TotalPrice = _configuration.GetValue<int>("PayOS:placeRegisterPrice") * userGroup.Count(),
                TotalPlacePaid = userGroup.Count(x => x.Status != "Unpaid"),
                TotalPlaceUnpaid = userGroup.Count(x => x.Status == "Unpaid"),   
            });
        }
        var user = await _unitOfWork.RepositoryUser.GetById(Guid.Parse(userId));
        var result = new PlaceStatsticMonth()
        {
            Avatar = user.ProfilePictureUrl,
            list = registrationByMonth,
            UserId = user.Id,
            UserName =  user.UserName,
        };
        return result;
    }
}