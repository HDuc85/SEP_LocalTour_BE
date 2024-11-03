using LocalTour.Services.Model;
using Microsoft.AspNetCore.Http;
using LocalTour.Services.ViewModel;

namespace LocalTour.Services.Abstract
{
    public interface IFileService
    {
        Task<ServiceResponseModel<MediaFileStaticVM>> SaveStaticFiles(List<IFormFile> files, string requestUrl);
        Task<ServiceResponseModel<bool>> DeleteFile(string fileName);
        Task<ServiceResponseModel<bool>> DeleteFiles(List<string> fileNames);
        Task<ServiceResponseModel<string>> SaveVideoFile(IFormFile file, string requestUrl);
        Task<ServiceResponseModel<string>> SaveImageFile(IFormFile file, string requestUrl);
    }
}
