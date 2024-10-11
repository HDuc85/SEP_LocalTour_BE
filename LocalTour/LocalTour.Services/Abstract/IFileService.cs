using LocalTour.Domain.Common;
using LocalTour.Services.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.Abstract
{
    public interface IFileService
    {
        Task<ServiceResponseModel<MediaFileStaticVM>> SaveStaticFiles(List<IFormFile> files, string requestUrl);
        Task<ServiceResponseModel<bool>> DeleteFile(string fileName);
        Task<ServiceResponseModel<bool>> DeleteFiles(List<string> fileNames);

    }
}
