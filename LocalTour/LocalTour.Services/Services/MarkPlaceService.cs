using LocalTour.Data.Abstract;
using LocalTour.Domain.Entities;
using LocalTour.Services.ViewModel;
using LocalTour.Services.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace LocalTour.Services.Services;

public class MarkPlaceService : IMarkPlaceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserService _userService;
    private readonly ITraveledPlaceService _traveledPlaceService;
    public MarkPlaceService(IUnitOfWork unitOfWork, IUserService userService, ITraveledPlaceService traveledPlaceService)
    {
        _unitOfWork = unitOfWork;
        _userService = userService;
        _traveledPlaceService = traveledPlaceService;
    }

    public async Task<bool> CreateMarkPlace(string userId, int placeId)
    {
        var user = await _userService.FindById(userId);
        if (user == null)
        {
            return false;
        }
        
        var check = await _unitOfWork.RepositoryMarkPlace.GetData(x => x.PlaceId == placeId && x.UserId == user.Id);
        if (check.Any())
        {
            return false;
        }

        await _unitOfWork.RepositoryMarkPlace.Insert(new MarkPlace
        {
            UserId = user.Id,
            PlaceId = placeId,
            CreatedDate = DateTime.Now,
            IsVisited = false,
        });
        await _unitOfWork.CommitAsync();
        return true;
    }
    public async Task<List<MarkPlaceVM>> GetMarkPlaces(string userId, string languageCode)
    {
        var user = await _userService.FindById(userId);
        _userService.FindById(userId);
        if (user == null)
        {
            return null;
        }

        var listMarkPlaces = _unitOfWork.RepositoryMarkPlace.GetDataQueryable(x => x.UserId == user.Id)
            .Include(y => y.Place)
            .Include(z => z.Place.PlaceTranslations).ToList();
        var listPlaceId = listMarkPlaces.Select(x => x.PlaceId).ToList();
        var visitPlaces = await _traveledPlaceService.
                CountTraveledPlaces(userId, listPlaceId);
            
        var results = new List<MarkPlaceVM>();
        foreach (var markPlace in listMarkPlaces)
        {
            results.Add(new MarkPlaceVM()
            {
                PlaceId = markPlace.PlaceId,
                UserId = markPlace.UserId,
                IsVisited = markPlace.IsVisited,
                Id = markPlace.Id,
                PhotoDisplay = markPlace.Place.PhotoDisplay,
                PlaceName = markPlace.Place.PlaceTranslations.Single(z => z.LanguageCode == languageCode).Name,
                VisitTime = visitPlaces.SingleOrDefault(x => x.Item1 == markPlace.PlaceId).Item2,
            });
        }
        
        return results;
    }

    public async Task<bool> UpdateMarkPlace(string userId, int placeId, bool isVisited)
    {
        var user = await _userService.FindById(userId);
        if (user == null)
        {
            return false;
        }
        
        var markPlace = await _unitOfWork.RepositoryMarkPlace.GetData(x => x.PlaceId == placeId && x.UserId == user.Id);
        if (markPlace.Any())
        {
            markPlace.First().IsVisited = isVisited;
            _unitOfWork.RepositoryMarkPlace.Update(markPlace.First());
            await _unitOfWork.CommitAsync();
            return true;
        }
        else
        {
            return false;
        }
        
    }

    public async Task<bool> DeleteMarkPlace(string userId, int placeId)
    {
        var user = await _userService.FindById(userId);
        if (user == null)
        {
            return false;
        }
        var markPlace = await _unitOfWork.RepositoryMarkPlace.GetData(x => x.PlaceId == placeId && x.UserId == user.Id);
        if (markPlace.Any())
        {
            _unitOfWork.RepositoryMarkPlace.Delete(markPlace.First());
            _unitOfWork.CommitAsync();
            return true;
        }
        return false;
    }
}