using LocalTour.Data.Abstract;
using LocalTour.Domain.Entities;
using LocalTour.Services.ViewModel;
using LocalTour.Services.Abstract;
using Microsoft.EntityFrameworkCore;

namespace LocalTour.Services.Services;

public class TraveledPlaceService : ITraveledPlaceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserService _userService;
    public TraveledPlaceService(IUnitOfWork unitOfWork, IUserService userService)
    {
        _unitOfWork = unitOfWork;
        _userService = userService;
    }

    public async Task<bool> AddTraveledPlace(string userId, int placeId)
    {
        var user = await _userService.FindById(userId);
        if (user == null)
        {
            return false;
        }
        var place = await _unitOfWork.RepositoryPlace.GetById(placeId);
        if (place == null)
        {
            return false;
        }

        await _unitOfWork.RepositoryTraveledPlace.Insert(new TraveledPlace()
        {
            UserId = user.Id,
            PlaceId = place.Id,
            TimeArrive = DateTime.Now,
        });
        
        return true;
    }

    public async Task<int> CountTraveledPlace(string userId,int placeId)
    {
        var user = await _userService.FindById(userId);
        if (user == null)
        {
            return -1;
        }
        var place = await _unitOfWork.RepositoryPlace.GetById(placeId);
        if (place == null)
        {
            return -1;
        }
        int count = _unitOfWork.RepositoryTraveledPlace.GetDataQueryable(x => x.UserId == user.Id && x.PlaceId == place.Id).Count();
        return count;
    }

    public async Task<List<(int,int)>> CountTraveledPlaces(string userId, List<int> placeIds)
    {
        var user = await _userService.FindById(userId);
        if (user == null)
        {
            return new List<(int, int)>();
        }
        
        var temp = _unitOfWork.
            RepositoryTraveledPlace.
            GetDataQueryable(x => x.UserId == user.Id)
            .GroupBy(x => x.PlaceId)
            .Select(y => new
            {
                PlaceId = y.Key,
                VisitTimes = y.Count(),
            }).ToList();
        var result = new List<(int, int)>();
        foreach (var item in temp)
        {
            result.Add((item.PlaceId, item.VisitTimes));
        }
        return result;
    }

    public async Task<List<TraveledPlaceVM>> GetAllTraveledPlaces(string userId)
    {
        var user = await _userService.FindById(userId);
        if (user == null)
        {
            return new List<TraveledPlaceVM>();
        }
        var temp = _unitOfWork.
                        RepositoryTraveledPlace.
                        GetDataQueryable(x => x.UserId == user.Id)
                        .GroupBy(x => x.PlaceId)
                        .Select(y => new
                        {
                            PlaceId = y.Key,
                            VisitTimes = y.Count(),
                        }).ToList();
        var places = _unitOfWork.RepositoryPlace.GetDataQueryable().Include(x => x.PlaceTranslations);
        var result = new List<TraveledPlaceVM>();
        foreach (var item in temp)
        {
            result.Add(new TraveledPlaceVM()
            {
                Place = places.Single(x => x.Id == item.PlaceId),
                TraveledTimes = item.VisitTimes,
            });
        }
        return result;
    }
}