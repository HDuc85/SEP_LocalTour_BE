using AutoMapper;
using LocalTour.Data.Abstract;
using LocalTour.Domain.Common;
using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;
using LocalTour.Services.Extensions;
using LocalTour.Services.Model;
using LocalTour.Services.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.Services
{
    public class PlaceActivityService : IPlaceActivityService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public PlaceActivityService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
        }
        public async Task<PlaceActivityRequest> CreatePlaceActivity(int placeid, PlaceActivityRequest request)
        {
            var places = await _unitOfWork.RepositoryPlace.GetById(placeid);
            if (places == null)
            {
                throw new ArgumentException($"Place with id {placeid} not found.");
            }
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            var placeActivity = new PlaceActivity
            {
                DisplayNumber = request.DisplayNumber,
                PhotoDisplay =request.PhotoDisplay,
                PlaceId = placeid,
            };
            await _unitOfWork.RepositoryPlaceActivity.Insert(placeActivity);
            var photoSaveResult = await SaveStaticFiles(request.PlaceActivityPhotos, "PlaceActivityPhoto");
            if (!photoSaveResult.Success)
            {
                throw new Exception(photoSaveResult.Message);
            }

            foreach (var photoUrl in photoSaveResult.Data.imageUrls)
            {
                var photo = new PlaceActivityPhoto
                {
                    PlaceActivityId = placeActivity.Id,
                    CreateDate = DateTime.Now,
                    Url = photoUrl
                };
                await _unitOfWork.RepositoryPlaceActivityPhoto.Insert(photo);
            }

            var videoSaveResult = await SaveStaticFiles(request.PlaceActivityVideos, "PlaceActivityVideo");
            if (!videoSaveResult.Success)
            {
                throw new Exception(videoSaveResult.Message);
            }

            foreach (var videoUrl in videoSaveResult.Data.videoUrls)
            {
                var video = new PlaceActivityVideo
                {
                    PlaceActivityId = placeActivity.Id,
                    CreateDate = DateTime.Now,
                    Url = videoUrl
                };
                await _unitOfWork.RepositoryPlaceActivityVideo.Insert(video);
            }
            foreach (var translation in request.PlaceActivityTranslations)
            {
                var translationEntity = new PlaceActivityTranslation
                {
                    PlaceActivityId = placeActivity.Id,
                    LanguageCode = translation.LanguageCode,
                    ActivityName = translation.ActivityName,
                    Price = translation.Price,
                    Description = translation.Description,
                    PriceType = translation.PriceType,
                    Discount = translation.Discount,
                    
                };
                await _unitOfWork.RepositoryPlaceActivityTranslation.Insert(translationEntity);
            }
            await _unitOfWork.CommitAsync();
            return request;
        }
        public async Task<ServiceResponseModel<MediaFileStaticVM>> SaveStaticFiles(List<IFormFile> files, string requestUrl)
        {
            int maxFileCount = _configuration.GetValue<int>("FileUploadSettings:MaxFileCount");
            int maxImageCount = _configuration.GetValue<int>("FileUploadSettings:MaxImageCount");
            int maxVideoCount = _configuration.GetValue<int>("FileUploadSettings:MaxVideoCount");
            long maxFileSize = _configuration.GetValue<long>("FileUploadSettings:MaxFileSize");

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
                return new(false, $"You can upload a maximum of {maxImageCount} images.");

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

                var fileUrl = $"{requestUrl}/Media/{fileName}";
                if (media == "images")
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

            return new(uploadedUrls);


        }
        public async Task<PaginatedList<PlaceActivityRequest>> GetAllActivityByPlaceid(int placeid,GetPlaceActivityRequest request)
        {
            var activities = _unitOfWork.RepositoryPlaceActivity.GetAll()
                                                    .Where(e => e.PlaceId == placeid)
                                                    .AsQueryable();

            if (request.SearchTerm is not null)
            {
                activities = activities.Where(x => x.PlaceActivityTranslations.Any(pt => pt.ActivityName.Contains(request.SearchTerm)) ||
                                           x.PlaceActivityTranslations.Any(pt => pt.Description.Contains(request.SearchTerm)));
            }

            if (!string.IsNullOrEmpty(request.SortBy))
            {
                activities = activities.OrderByCustom(request.SortBy, request.SortOrder);
            }

            return await activities
                .ListPaginateWithSortAsync<PlaceActivity, PlaceActivityRequest>(
                request.Page,
                request.Size,
                request.SortBy,
                request.SortOrder,
                _mapper.ConfigurationProvider);
        }
    }
}
