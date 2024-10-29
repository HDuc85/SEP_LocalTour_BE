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
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.Services
{
    public class PlaceFeedbackService : IPlaceFeedbackService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PlaceFeedbackService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PlaceFeedbackRequest> CreateFeedback(int placeid, PlaceFeedbackRequest request)
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
            var userid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userid, out var userId))
            {
                throw new InvalidOperationException("User ID is not a valid GUID.");
            }
            var feedback = new PlaceFeeedback
            {
                PlaceId = placeid,
                UserId = userId,
                Rating = request.Rating,
                Content = request.Content,
                CreatedDate = DateTime.UtcNow,
            };
            await _unitOfWork.RepositoryPlaceFeeedback.Insert(feedback);
            var mediaSaveResult = await SaveStaticFiles(request.PlaceFeedbackMedia, "PlaceFeedbackMedia");
            if (!mediaSaveResult.Success)
            {
                throw new Exception(mediaSaveResult.Message);
            }

            foreach (var photoUrl in mediaSaveResult.Data.imageUrls)
            {
                var photo = new PlaceFeeedbackMedium
                {
                    FeedbackId = feedback.Id,
                    Type = "Image",
                    CreateDate = DateTime.Now,
                    Url = photoUrl
                };
                await _unitOfWork.RepositoryPlaceFeeedbackMedium.Insert(photo);
            }
            foreach (var mediaUrl in mediaSaveResult.Data.videoUrls)
            {
                var media = new PlaceFeeedbackMedium
                {
                    FeedbackId = feedback.Id,
                    Type = "Video",
                    CreateDate = DateTime.Now,
                    Url = mediaUrl
                };
                await _unitOfWork.RepositoryPlaceFeeedbackMedium.Insert(media);
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
        public Task<int> DeleteFeedback(int placeid, int feedbackid)
        {
            throw new NotImplementedException();
        }

        public Task<PlaceFeedbackRequest> UpdateFeedback(int placeid,int feedbackid, PlaceFeedbackRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<PaginatedList<PlaceFeedbackRequest>> GetAllFeedbackByPlace(int placeid, GetPlaceFeedbackRequest request)
        {
                var feedbacks = _unitOfWork.RepositoryPlaceFeeedback.GetAll()
                                                        .Where(e => e.PlaceId == placeid)
                                                        .AsQueryable();

                if (request.SearchTerm is not null)
                {
                    feedbacks = feedbacks.Where(e => e.Content.Contains(request.SearchTerm));
                }

                if (!string.IsNullOrEmpty(request.SortBy))
                {
                    feedbacks = feedbacks.OrderByCustom(request.SortBy, request.SortOrder);
                }

                return await feedbacks
                    .ListPaginateWithSortAsync<PlaceFeeedback, PlaceFeedbackRequest>(
                    request.Page,
                    request.Size,
                    request.SortBy,
                    request.SortOrder,
                    _mapper.ConfigurationProvider);
            }
        
    }
}
