using LocalTour.Services.Model;
using Microsoft.AspNetCore.Http;
using LocalTour.Services.ViewModel;
using LocalTour.Domain.Common;

namespace LocalTour.Services.Abstract
{
    public interface IFileService
    {
        Task<ServiceResponseModel<MediaFileStaticVM>> SaveStaticFiles(List<IFormFile> files);
        Task<ServiceResponseModel<bool>> DeleteFile(string fileName);
        Task<ServiceResponseModel<bool>> DeleteFiles(List<string> fileNames);
        Task<ServiceResponseModel<string>> SaveImageFile(IFormFile file);
        Task<ServiceResponseModel<string>> SaveVideoFile(IFormFile file);
    }
}