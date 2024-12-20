using LocalTour.Data.Abstract;
using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;
using LocalTour.Services.Extensions;
using LocalTour.Services.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace LocalTour.Services.Services;

public class ModCheckService : IModCheckService
{
    readonly private IUnitOfWork _unitOfWork;
    readonly private IFileService _fileService;

    public ModCheckService(IUnitOfWork unitOfWork, IFileService fileService)
    {
        _unitOfWork = unitOfWork;
        _fileService = fileService;
    }
    
    public async Task<PaginatedList<ModCheckReponse>> GetAllModChecks(GetAllModRequest queryParams)
    {
        var modChecks = _unitOfWork.RepositoryModCheckPlace.GetDataQueryable()
            .Include(u => u.Place)
            .ThenInclude(y => y.Ward)
            .Where(x => (queryParams.DistricNCityId == null || x.Place.Ward.DistrictNcityId == queryParams.DistricNCityId))
            .Include(x => x.Place)
            .ThenInclude(y => y.PlaceTranslations)
            .Include(z => z.Place)
            .ThenInclude(t => t.PlaceMedia)
            .Include(o => o.Mod)
            .GroupBy(x => new { x.ModId, x.PlaceId })
            .AsQueryable();
       
        var pageNumber = queryParams.Page.GetValueOrDefault(1);
        var sizeNumber = queryParams.Size.GetValueOrDefault(10);

        var count = await modChecks.CountAsync();
          
        var list = modChecks.ToList();
        list = list.Skip((pageNumber - 1) * sizeNumber).Take(sizeNumber).ToList();
        
        var result = new List<ModCheckReponse>();
        foreach (var group in list)
        {
           result.Add(new ModCheckReponse()
           {
               PlaceId = group.First().PlaceId,
               PlaceTranslations =  group.First().Place.PlaceTranslations.ToList(),
               ModId = group.First().ModId,
               ModName = group.First().Mod.UserName,
               PlaceMediums = group.First().Place.PlaceMedia.ToList(),
               ModeCheckImages = group.Select(x => x.ImageUrl).ToList()
           });
        }
        
        
        return new PaginatedList<ModCheckReponse>(result, count, pageNumber, sizeNumber);
    }

    public async Task<bool> CreateModCheck(CreateModCheckRequest request, string userId)
    {
        var place = await _unitOfWork.RepositoryPlace.GetById(request.PlaceId);
        if (place == null)
        {
            throw new Exception("Place not found");
        }
        
        _unitOfWork.RepositoryModCheckPlace.Delete(x => x.PlaceId == request.PlaceId);
        await _unitOfWork.CommitAsync();
        
        foreach (var item in request.Files)
        {
            var url = await _fileService.SaveImageFile(item);

            _unitOfWork.RepositoryModCheckPlace.Insert(new ModCheckPlace()
            {
                PlaceId = request.PlaceId,
                ModId = Guid.Parse(userId),
                ImageUrl = url.Data ?? "",
                CreatedDate = DateTime.Now
            });
        }

        await _unitOfWork.CommitAsync();
        return true;
    }

    public async Task<bool> Delete(int placeId)
    {
        var place = await _unitOfWork.RepositoryPlace.GetById(placeId);
        if (place == null)
        {
            throw new Exception("Place not found");
        }
        
        _unitOfWork.RepositoryModCheckPlace.Delete(x => x.PlaceId == placeId);
        await _unitOfWork.CommitAsync();
        return true;
    }
}