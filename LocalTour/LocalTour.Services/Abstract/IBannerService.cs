using LocalTour.Domain.Entities;
using LocalTour.Services.ViewModel;

namespace LocalTour.Services.Abstract;

public interface IBannerService
{
    Task<List<BannerRespone>> GetAllAsync(GetAllBannerRequest request);
    Task<BannerRespone> GetByIdAsync(Guid id, String userId);
    Task<Banner> CreateAsync(BannerRequest bannerRequest, String userId);
    Task<BannerRespone> UpdateAsync(BannerRequest bannerRequest, String userId, string id);
    Task<bool> DeleteAsync(Guid id, String userId);
    Task<BannerHistory> CreateHistoryBanner(BannerHistoryRequest historyRequest, String userId);
    Task<BannerUrlResponse> GetPublicBannerActive();
}