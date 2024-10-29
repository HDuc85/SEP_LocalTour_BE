using AutoMapper;
using Azure;
using Azure.Core;
using LocalTour.Data.Abstract;
using LocalTour.Domain.Common;
using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;
using LocalTour.Services.Extensions;
using LocalTour.Services.Model;
using LocalTour.Services.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.Services
{
    public class PlaceService : IPlaceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        public PlaceService (IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
        }
        [HttpPost]
        public async Task<PlaceRequest> CreatePlace(PlaceRequest place)
        {
            if (place == null)
            {
                throw new ArgumentNullException(nameof(place));
            }
            var placeEntity = new Place
            {
                TimeOpen = place.TimeOpen,
                TimeClose = place.TimeClose,
                Longitude = place.Longitude,
                Latitude = place.Latitude,
                PhotoDisplay = place.PhotoDisplay,
                Status = "0",
            };
            await _unitOfWork.RepositoryPlace.Insert(placeEntity);
            foreach (var tags in place.Tags)
            {
                var tagEntity = await _unitOfWork.RepositoryTag.GetById(tags);
                if (tagEntity != null)
                {
                    var placetag = new PlaceTag
                    {
                        PlaceId = placeEntity.Id,
                        TagId = tags,
                    };
                   await _unitOfWork.RepositoryPlaceTag.Insert(placetag);
                };
                
            }
            foreach (var translation in place.PlaceTranslation)
            {
                var translationEntity = new PlaceTranslation
                {
                    LanguageCode = translation.LanguageCode,
                    Name = translation.Name,
                    Description = translation.Description,
                    Address = translation.Address,
                    Contact = translation.Contact
                };
                await _unitOfWork.RepositoryPlaceTranslation.Insert(translationEntity);
            }
            var photoSaveResult = await SaveStaticFiles(place.PlaceMedia, "PlaceMedia");
            if (!photoSaveResult.Success)
            {
                throw new Exception(photoSaveResult.Message);
            }

            foreach (var photoUrl in photoSaveResult.Data.imageUrls)
            {
                var photo = new PlaceMedium
                {
                    PlaceId = placeEntity.Id,
                    CreateDate = DateTime.Now,
                    Type = "Image",
                    Url = photoUrl
                };
                await _unitOfWork.RepositoryPlaceMedium.Insert(photo);
            }
            foreach (var mediaUrl in photoSaveResult.Data.videoUrls)
            {
                var media = new PlaceMedium
                {
                    PlaceId = placeEntity.Id,
                    CreateDate = DateTime.Now,
                    Type = "Video",
                    Url = mediaUrl
                };
                await _unitOfWork.RepositoryPlaceMedium.Insert(media);
            }
            await _unitOfWork.CommitAsync();
            return place;
        }

        public async Task<PaginatedList<PlaceRequest>> GetAllPlace(GetPlaceRequest request)
        {
            var places = _unitOfWork.RepositoryPlace.GetAll().Include(x => x.PlaceTranslations)
           .AsQueryable();

            if (request.SearchTerm is not null)
            {
                places = places.Where(x => x.PlaceTranslations.Any(pt => pt.Name.Contains(request.SearchTerm)) ||
                                           x.PlaceTranslations.Any(pt => pt.Address.Contains(request.SearchTerm)));
            }
            var placesWithDistance = places.Select(place => new
            {
                Place = place,
                Distance = QueryableExtensions.GetDistance(request.CurrentLatitude, request.CurrentLongitude, place.Latitude, place.Longitude) // Đảm bảo Place có Latitude và Longitude
            });

            // Sắp xếp theo khoảng cách
            placesWithDistance = placesWithDistance.OrderBy(x => x.Distance);

            // Chuyển đổi lại về IQueryable<Place>
            var sortedPlaces = placesWithDistance.Select(x => x.Place).AsQueryable();
            return await places
                .ListPaginateWithSortAsync<Place, PlaceRequest>(
                    request.Page,
                    request.Size,
                    request.SortBy,
                    request.SortOrder,
                    _mapper.ConfigurationProvider);
        }

        public async Task<Place> GetPlaceById(int placeid)
        {
            var placeEntity = await _unitOfWork.RepositoryPlace.GetAll()
                .FirstOrDefaultAsync(e => e.Id == placeid);

            if (placeEntity == null)
            {
                throw new KeyNotFoundException($"Place with ID {placeid} not found.");
            }

            return placeEntity;
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

        public async Task<PlaceRequest> UpdatePlace(int placeid, PlaceRequest request)
        {
            var existingPlace = await _unitOfWork.RepositoryPlace.GetById(placeid);
            if (existingPlace == null)
            {
                throw new ArgumentException($"Event with id {placeid} not found.");
            }

            existingPlace.TimeOpen = request.TimeOpen;
            existingPlace.TimeClose = request.TimeClose;
            existingPlace.Longitude = request.Longitude;
            existingPlace.Latitude = request.Latitude;
            existingPlace.PhotoDisplay = request.PhotoDisplay;
            existingPlace.Status = "0";
            var existingMedia = await _unitOfWork.RepositoryPlaceMedium.GetAll()
                                                                 .Where(e => e.PlaceId == placeid)
                                                                 .ToListAsync();
            foreach (var media in existingMedia)
            {
                _unitOfWork.RepositoryPlaceMedium.Delete(media);
            }

            var existingTranslations = await _unitOfWork.RepositoryPlaceTranslation.GetAll()
                                                                       .Where(e => e.PlaceId == placeid)
                                                                       .ToListAsync();
            foreach (var translation in existingTranslations)
            {
                _unitOfWork.RepositoryPlaceTranslation.Delete(translation);
            }
            var existingTags = await _unitOfWork.RepositoryPlaceTag.GetAll()
                                                           .Where(e => e.PlaceId == placeid)
                                                           .ToListAsync();
            foreach (var tag in existingTags)
            {
                _unitOfWork.RepositoryPlaceTag.Delete(tag);
            }
            foreach (var tags in request.Tags)
            {
                var tagEntity = await _unitOfWork.RepositoryTag.GetById(tags);
                if (tagEntity != null)
                {
                    var placetag = new PlaceTag
                    {
                        PlaceId = existingPlace.Id,
                        TagId = tags,
                    };
                    await _unitOfWork.RepositoryPlaceTag.Insert(placetag);
                };

            }
            foreach (var translation in request.PlaceTranslation)
            {
                var translationEntity = new PlaceTranslation
                {
                    LanguageCode = translation.LanguageCode,
                    Name = translation.Name,
                    Description = translation.Description,
                    Address = translation.Address,
                    Contact = translation.Contact
                };
                await _unitOfWork.RepositoryPlaceTranslation.Insert(translationEntity);
            }
            var photoSaveResult = await SaveStaticFiles(request.PlaceMedia, "PlaceMedia");
            if (!photoSaveResult.Success)
            {
                throw new Exception(photoSaveResult.Message);
            }

            foreach (var photoUrl in photoSaveResult.Data.imageUrls)
            {
                var photo = new PlaceMedium
                {
                    PlaceId = existingPlace.Id,
                    CreateDate = DateTime.Now,
                    Type = "Image",
                    Url = photoUrl
                };
                await _unitOfWork.RepositoryPlaceMedium.Insert(photo);
            }
            foreach (var mediaUrl in photoSaveResult.Data.videoUrls)
            {
                var media = new PlaceMedium
                {
                    PlaceId = existingPlace.Id,
                    CreateDate = DateTime.Now,
                    Type = "Video",
                    Url = mediaUrl
                };
                await _unitOfWork.RepositoryPlaceMedium.Insert(media);
            }
            _unitOfWork.RepositoryPlace.Update(existingPlace);
            await _unitOfWork.CommitAsync();
            return request;
        }
    }
    
}
