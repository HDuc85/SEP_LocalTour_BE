using LocalTour.Data.Abstract;
using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;
using Microsoft.EntityFrameworkCore;

namespace LocalTour.Services.Services;

public class PlaceSearchHistoryService : IPlaceSearchHistoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserService _userService;
    public PlaceSearchHistoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> AddPlaceSearchHistory(string userId, int placeId)
    {
        var user = await _userService.FindById(userId);
        if (user == null)
        {
            return false;
        }
        var place  = await _unitOfWork.RepositoryPlace.GetById(placeId);
        if (place == null)
        {
            return false;
        }
        var check = await UpdatePlaceSearchHistory(userId, placeId);
        if (check)
        {
            return true;
        }
        await _unitOfWork.RepositoryPlaceSearchHistory.Insert(new PlaceSearchHistory()
        {
            PlaceId = place.Id,
            UserId = user.Id,
            LastSearch = DateTime.Now,
        });
        await _unitOfWork.CommitAsync();
        return true;
    }

    public async Task<List<PlaceSearchHistory>> GetAllPlaceSearchHistory(string phoneNumber,int? pageNumber, int? pageSize, string languageCode)
    {
        var user = await _userService.FindByPhoneNumber(phoneNumber);
        if (user == null)
        {
            return new List<PlaceSearchHistory>();
        }

        if (pageNumber != null && pageSize != null && pageSize > 0 && pageNumber > 0)
        {
            return _unitOfWork.RepositoryPlaceSearchHistory.GetDataQueryable(x => x.UserId == user.Id)
                .Include(y => y.Place)
                .Include(z => z.Place.PlaceTranslations.Where(z => z.LanguageCode == languageCode))
                .Skip((pageNumber.Value - 1) * pageSize.Value)
                .Take(pageSize.Value).OrderByDescending(o => o.LastSearch).ToList();
            
        }
        return _unitOfWork.RepositoryPlaceSearchHistory.GetDataQueryable(x => x.UserId == user.Id)
            .Include(y => y.Place)
            .Include(z => z.Place.PlaceTranslations.Where(z => z.LanguageCode == languageCode))
            .OrderByDescending(o => o.LastSearch).ToList();
    }

    public async Task<bool> DeletePlaceSearchHistory(string userId, int placeId)
    {
        var user = await _userService.FindById(userId);
        if (user == null)
        {
            return false;
        }
        _unitOfWork.RepositoryPlaceSearchHistory.Delete(x => x.UserId == user.Id && x.PlaceId == placeId);
        return true;
    }

    public async Task<bool> DeleteAllPlaceSearchHistory(string userId)
    {
        var user = await _userService.FindById(userId);
        if (user == null)
        {
            return false;
        }
        
        _unitOfWork.RepositoryPlaceSearchHistory.Delete(x => x.UserId == user.Id);
        return true;
    }
    public async Task<bool> UpdatePlaceSearchHistory(string userId, int placeId)
    {
        var user = await _userService.FindById(userId);
        if (user == null)
        {
            return false;
        }
        
        var temp = await _unitOfWork.RepositoryPlaceSearchHistory.GetData(x => x.UserId == user.Id && x.PlaceId == placeId);
        if (temp.Any())
        {
            temp.First().LastSearch = DateTime.Now;
            _unitOfWork.RepositoryPlaceSearchHistory.Update(temp.First());
            return true;
        }
        return false;
    }
}