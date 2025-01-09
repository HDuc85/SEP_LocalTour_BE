using LocalTour.Data.Abstract;
using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;
using LocalTour.Services.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace LocalTour.Services.Services;

public class BannerService : IBannerService
{
    private IUnitOfWork _unitOfWork;
    private IUserService _userService;
    private IFileService _fileService;
    
    public  BannerService(IUnitOfWork unitOfWork, IUserService userService, IFileService fileService)
    {
        _fileService = fileService;
        _userService = userService;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<BannerRespone>> GetAllAsync(GetAllBannerRequest request)
    {
        
        var listban = _unitOfWork.RepositoryBanner.GetAll()
            .Include(y => y.BannerHistories)
            .Include(x => x.Author).ToList();

        if (request.UserId != null)
        {
            listban = listban.Where(x => x.AuthorId == Guid.Parse(request.UserId)).ToList();
        }

        if (request.UserName != null)
        {
            listban = listban.Where(x => x.Author.UserName.ToLower().Contains(request.UserName.ToLower())).ToList();
        }

        if (request.BannerName != null)
        {
            listban = listban.Where(x => x.BannerName.ToLower().Contains(request.BannerName.ToLower())).ToList();
        }

        
        List<BannerRespone> bannerRespone = new List<BannerRespone>();
        
        foreach (var item in listban)
        {
            BannerRespone newBanner = new BannerRespone();
            
            if (item.BannerHistories.Count > 0)
            {
                newBanner.BannerHistories =  item.BannerHistories
                    .Where(history => request.DateStart == null || request.DateStart <= history.TimeStart || request.DateEnd == null || request.DateEnd >= history.TimeEnd)
                    .Select(history =>
                         new BannerHistoryResponse()
                        {
                            Id = history.Id,
                            Status = history.Status,
                            BannerId = history.BannerId,
                            TimeEnd = history.TimeEnd,
                            TimeStart = history.TimeStart,
                        }
                ).ToList();
            }
            
            newBanner.BannerUrl = item.BannerUrl;
            newBanner.AuthorId = item.AuthorId;
            newBanner.CreatedDate = item.CreatedDate;
            newBanner.UpdatedDate = item.UpdatedDate;
            newBanner.BannerName = item.BannerName;
            newBanner.AuthorName = item.Author.UserName;
            newBanner.Id = item.Id;
            bannerRespone.Add(newBanner);
        }
        
        
        return bannerRespone;
    }
    public async Task<BannerRespone> GetByIdAsync(Guid id, String userId)
    {
        var user = await _userService.FindById(userId);
        
        var listban = _unitOfWork.RepositoryBanner.GetAll()
            .Where(x => x.AuthorId == user.Id && x.Id == id)
            .Include(y => y.BannerHistories).ToList();
        BannerRespone bannerRespone = new BannerRespone();
        if (listban.IsNullOrEmpty())
        {
            throw new Exception("Id or User is not found");
        }
            var item = listban.FirstOrDefault(x => x.Id == id);
            BannerRespone newBanner = new BannerRespone();
            
            if (listban.First().BannerHistories.Count > 0)
            {
                newBanner.BannerHistories =  item.BannerHistories.Select(history =>
                    new BannerHistoryResponse()
                    {
                        Id = history.Id,
                        Status = history.Status,
                        BannerId = history.BannerId,
                        TimeEnd = history.TimeEnd,
                        TimeStart = history.TimeStart,
                    }
                ).ToList();
            }
            
            newBanner.BannerUrl = item.BannerUrl;
            newBanner.AuthorId = item.AuthorId;
            newBanner.CreatedDate = item.CreatedDate;
            newBanner.UpdatedDate = item.UpdatedDate;
            newBanner.BannerName = item.BannerName;
            newBanner.AuthorName = user.UserName;
            newBanner.Id = item.Id;
        
        
        return bannerRespone;
    }

    public async Task<BannerRespone> CreateAsync(BannerRequest bannerRequest, String userId)
    {
        var user = await _userService.FindById(userId);
        
        Banner banner = new Banner();
        banner.AuthorId = user.Id;
        banner.CreatedDate = DateTime.Now;
        banner.BannerName = bannerRequest.BannerName;
        banner.UpdatedDate = DateTime.Now;
        var url =  await _fileService.SaveImageFile(bannerRequest.BannerUrl);
        banner.BannerUrl = url.Data;
        
        await _unitOfWork.RepositoryBanner.Insert(banner);
        await _unitOfWork.CommitAsync();
        BannerRespone response = new BannerRespone()
        {
            Id = banner.Id,
            BannerName = banner.BannerName,
            BannerUrl = banner.BannerUrl,
            UpdatedDate = DateTime.Now,
            CreatedDate = DateTime.Now,
            AuthorId = user.Id,
            AuthorName = user.UserName,
        };
        return response;
    }

    public async Task<BannerRespone> UpdateAsync(BannerUpdateRequest bannerRequest, String userId, string id)
    {
        var user = await _userService.FindById(userId);
        
        var banner = await _unitOfWork.RepositoryBanner.GetData(x => x.Id == Guid.Parse(id));

        if (banner == null)
        {
            throw new Exception("Banner not found");
        }

        if (banner.First().AuthorId != user.Id)
        {
            throw new Exception("User does not belong to this banner");
        }

        if (bannerRequest.BannerName != null)
        {
            banner.First().BannerName = bannerRequest.BannerName;
        }

        if (bannerRequest.BannerUrl != null)
        {
            banner.First().BannerUrl = bannerRequest.BannerUrl;
        }
        banner.First().UpdatedDate = DateTime.Now;
         _unitOfWork.RepositoryBanner.Update(banner.First());
         await _unitOfWork.CommitAsync();
         
         var result = await GetByIdAsync(banner.First().Id, userId);
         
         return result;
    }

    public async Task<bool> DeleteAsync(Guid id, string userId)
    {
        var user = await _userService.FindById(userId);
        var banner = await _unitOfWork.RepositoryBanner.GetById(id);
        
        if (banner == null)
        {
            throw new Exception("Banner not found");
        }

        if (banner.AuthorId != user.Id)
        {
            throw new Exception("User does not belong to this banner");
        }

        _unitOfWork.RepositoryBannerHistory.Delete(x => x.BannerId == id);
        await _fileService.DeleteFile(banner.BannerUrl);
        _unitOfWork.RepositoryBanner.Delete(banner);
        await _unitOfWork.CommitAsync();
        return true;
    }

    public async Task<BannerHistory> CreateHistoryBanner(BannerHistoryRequest historyRequest, String userId)
    {
        var banner = await _unitOfWork.RepositoryBanner.GetById(historyRequest.BannerId);
        var user = await _userService.FindById(userId);
        if (banner == null)
        {
            throw new Exception("Banner not found");
        }
        BannerHistory bannerHistory = new BannerHistory();
        bannerHistory.BannerId = banner.Id;
        bannerHistory.TimeStart = historyRequest.TimeStart;
        bannerHistory.TimeEnd = historyRequest.TimeEnd;
        bannerHistory.Status = "Active";
        bannerHistory.ApproverId = user.Id;
        
        await _unitOfWork.RepositoryBannerHistory.Insert(bannerHistory);
        await _unitOfWork.CommitAsync();
        return bannerHistory;
    }
    public async Task<BannerUrlResponse> GetPublicBannerActive()
    {
        BannerUrlResponse bannerUrlResponse = new BannerUrlResponse();
        
        DateTime now = DateTime.Now;

        var listbanner =  _unitOfWork.RepositoryBanner
            .GetDataQueryable()
            .Include(x => x.BannerHistories)
            .ToList();
        foreach (var banner in listbanner)
        {
            if (banner.BannerHistories.Count > 0)
            {
                foreach (var item in banner.BannerHistories)
                {
                    if (now >= item.TimeEnd)
                    {
                        item.Status = "Completed";
                        _unitOfWork.RepositoryBannerHistory.Update(item);
                    }
                }
            }
        }
        await _unitOfWork.CommitAsync();
        
        
        listbanner = listbanner
            .Where(y => y.BannerHistories
                .Any(z => z.Status == "Active" && now >= z.TimeStart && now < z.TimeEnd)).ToList();

        
       bannerUrlResponse.bannerUrls =listbanner.Select(x => x.BannerUrl).ToList();
       return bannerUrlResponse;
    }

    public async Task<bool> UpdateBannerHistoryStatus(Guid bannerHistoryId, String status)
    {
        var banner = await _unitOfWork.RepositoryBannerHistory.GetById(bannerHistoryId);
        
        banner.Status = status;
        
        await _unitOfWork.CommitAsync();
        return true;
    }
    
}