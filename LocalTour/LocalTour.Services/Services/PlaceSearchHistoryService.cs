using LocalTour.Data.Abstract;
using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;
using LocalTour.Services.Model;
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

    public async Task<ServiceResponseModel<bool>> AddPlaceSearchHistory(string userId, int placeId)
    {
        var user = await _userService.FindById(userId);
        if (user == null)
        {
            return new ServiceResponseModel<bool>(false, "User does not exist");
        }
        var place  = await _unitOfWork.RepositoryPlace.GetById(placeId);
        if (place == null)
        {
            return new ServiceResponseModel<bool>(false, "Place does not exist");
        }
        var check = await UpdatePlaceSearchHistory(userId, placeId);
        if (check)
        {
            return new ServiceResponseModel<bool>(true, "Search History Successfully Updated");
        }
        await _unitOfWork.RepositoryPlaceSearchHistory.Insert(new PlaceSearchHistory()
        {
            PlaceId = place.Id,
            UserId = user.Id,
            LastSearch = DateTime.Now,
        });
        await _unitOfWork.CommitAsync();
        return new ServiceResponseModel<bool>(true, "Search History Successfully Added");
    }

    public async Task<ServiceResponseModel<List<PlaceSearchHistory>>> GetAllPlaceSearchHistory(string userId,int? pageNumber, int? pageSize, string languageCode)
    {
        var user = await _userService.FindById(userId);
        if (user == null)
        {
            return new ServiceResponseModel<List<PlaceSearchHistory>>(false,"User is not exists");
        }

        if (pageNumber != null && pageSize != null && pageSize > 0 && pageNumber > 0)
        {
            return new ServiceResponseModel<List<PlaceSearchHistory>>(true,
                _unitOfWork.RepositoryPlaceSearchHistory.GetDataQueryable(x => x.UserId == user.Id)
                    .Include(y => y.Place)
                    .Include(z => z.Place.PlaceTranslations.Where(z => z.LanguageCode == languageCode))
                    .Skip((pageNumber.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value).OrderByDescending(o => o.LastSearch).ToList());

        }
        return new ServiceResponseModel<List<PlaceSearchHistory>>(true,_unitOfWork.RepositoryPlaceSearchHistory.GetDataQueryable(x => x.UserId == user.Id)
            .Include(y => y.Place)
            .Include(z => z.Place.PlaceTranslations.Where(z => z.LanguageCode == languageCode))
            .OrderByDescending(o => o.LastSearch).ToList());
    }

    public async Task<ServiceResponseModel<bool>> DeletePlaceSearchHistory(string userId, int placeId)
    {
        var user = await _userService.FindById(userId);
        if (user == null)
        {
            return new ServiceResponseModel<bool>(false,"User is not exist");
        }
        _unitOfWork.RepositoryPlaceSearchHistory.Delete(x => x.UserId == user.Id && x.PlaceId == placeId);
        return new ServiceResponseModel<bool>(true, "Search History Item Successfully Deleted");
    }

    public async Task<ServiceResponseModel<bool>> DeleteAllPlaceSearchHistory(string userId)
    {
        var user = await _userService.FindById(userId);
        if (user == null)
        {
            return new ServiceResponseModel<bool>(false,"User is not exist");
        }
        
        _unitOfWork.RepositoryPlaceSearchHistory.Delete(x => x.UserId == user.Id);
        return new ServiceResponseModel<bool>(true, "All Search History Successfully Deleted");
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