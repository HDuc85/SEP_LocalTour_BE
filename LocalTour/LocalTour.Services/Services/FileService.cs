using LocalTour.Services.Abstract;
using LocalTour.Services.Model;
using LocalTour.Services.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace LocalTour.Services.Services
{
    public class FileService : IFileService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public FileService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        private string getRequestUrl()
        {
           return $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
        }
        public async Task<ServiceResponseModel<bool>> DeleteFile(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Media", fileName);

            if (!File.Exists(filePath))
            {
                return new 
                (
                    false,
                    $"File '{fileName}' not found."
                );
            }


            try
            {
                File.Delete(filePath);
                return new 
                (
                    true,
                    $"File '{fileName}' successfully deleted."
                );
            }
            catch (Exception ex)
            {
                
                return new 
                (
                    false,
                    $"Error deleting file: {ex.Message}"
                );
            }
        }

        public async Task<ServiceResponseModel<bool>> DeleteFiles(List<string> fileNames)
        {
            var failedDeletions = new List<string>();
            foreach (var fileName in fileNames)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Media", fileName);

                if (!File.Exists(filePath))
                {
                    failedDeletions.Add(fileName);
                    continue;
                }

                try
                {
                    File.Delete(filePath);
                }
                catch (Exception)
                {
                    failedDeletions.Add(fileName);
                }
            }

            // Build the result message
            if (failedDeletions.Count > 0)
            {
                var message = $"Failed to delete: {string.Join(", ", failedDeletions)}";
                return new 
                (
                    failedDeletions.Count < fileNames.Count,
                    message
                );
            }

            return new 
            (
                true,
                "All files deleted successfully."
            );
        }

        public async Task<ServiceResponseModel<MediaFileStaticVM>> SaveStaticFiles(List<IFormFile> files)
        {
          
            int maxFileCount = _configuration.GetValue<int>("FileUploadSettings:MaxFileCount");
            int maxImageCount = _configuration.GetValue<int>("FileUploadSettings:MaxImageCount");
            int maxVideoCount = _configuration.GetValue<int>("FileUploadSettings:MaxVideoCount");
            long maxFileSize = _configuration.GetValue<long>("FileUploadSettings:MaxFileSize") * 1024 * 1024;

            int imageCount = 0;
            int videoCount = 0;


            foreach (var file in files)
            {
                string fileExtension = Path.GetExtension(file.FileName).ToLower();

                if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png" || fileExtension == ".gif")
                {
                    imageCount++;
                }
                else if (fileExtension == ".mp4" || fileExtension == ".avi" || fileExtension == ".mkv")
                {
                    videoCount++;
                }
                else
                {
                    return new 
                        (
                            false, 
                            $"Invalid file type: {file.FileName}. Only image and video files are allowed."
                        );
                }
            }

            if (imageCount > maxImageCount)
                return new (false,$"You can upload a maximum of {maxImageCount} images.");

            if (videoCount > maxVideoCount)
                return new 
                    (
                        false, 
                        $"You can upload a maximum of {maxVideoCount} videos."
                    );

            imageCount = 0;
            videoCount = 0;

            var imageUrls = new List<string>();
            var videoUrls = new List<string>();

            foreach (var file in files)
            {
                if (file.Length > maxFileSize)
                    return new 
                        (
                            false,
                            $"File {file.FileName} exceeds the maximum allowed size of {maxFileSize / (1024 * 1024)}MB."
                        );

                string fileExtension = Path.GetExtension(file.FileName).ToLower();
                string media;

                if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png" || fileExtension == ".gif")
                {
                    imageCount++;
                    media = "image";
                }
                else
                {
                    videoCount++;
                    media = "video";
                }

                var fileName = media + "_" + Guid.NewGuid().ToString() + fileExtension;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Media", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var fileUrl = $"{getRequestUrl()}/Media/{fileName}";
                if(media == "images")
                {
                    imageUrls.Add(fileUrl);
                }
                else
                {
                    videoUrls.Add(fileUrl);
                }
            }
            var uploadedUrls = new MediaFileStaticVM() 
            {
                videoUrls = videoUrls,
                imageUrls = imageUrls
            };

            return new (uploadedUrls);


        }

        public async Task<ServiceResponseModel<string>> SaveVideoFile(IFormFile file)
        {
            string fileExtension = Path.GetExtension(file.FileName).ToLower();

            if (fileExtension == ".mp4" || fileExtension == ".avi" || fileExtension == ".mkv")
            {
                var fileName = "video_" + Guid.NewGuid().ToString() + fileExtension;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Media", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return new ServiceResponseModel<string>($"{getRequestUrl()}/Media/{fileName}");
            }
            else
            {
                return new
                    (
                        false,
                        $"Invalid file type: {file.FileName}. Only video file is allowed."
                    );
            }
        }

        public async Task<ServiceResponseModel<string>> SaveImageFile(IFormFile file)
        {
            string fileExtension = Path.GetExtension(file.FileName).ToLower();
          
            if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png" || fileExtension == ".gif")
            {
                var fileName = "image_" + Guid.NewGuid().ToString() + fileExtension;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Media", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return new ServiceResponseModel<string>($"{getRequestUrl()}/Media/{fileName}");
            }
            else
            {
                return new
                    (
                        false,
                        $"Invalid file type: {file.FileName}. Only image file is allowed."
                    );
            }
        }
    }
}
