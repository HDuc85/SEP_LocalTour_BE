using AutoMapper;
using LocalTour.Data.Abstract;
using LocalTour.Domain.Common;
using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;
using LocalTour.Services.Extensions;
using LocalTour.Services.Model;
using LocalTour.Services.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        private readonly IFileService _fileService;

        public PlaceActivityService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
            _fileService = fileService;
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
            var photos = await _fileService.SaveImageFile(request.PhotoDisplay, "PlaceActivityMedia");
            var placeActivity = new PlaceActivity
            {
                DisplayNumber = request.DisplayNumber,
                PhotoDisplay =photos.Data,
                PlaceId = placeid,
            };
            await _unitOfWork.RepositoryPlaceActivity.Insert(placeActivity);
            await _unitOfWork.CommitAsync();
            var photoSaveResult = await _fileService.SaveStaticFiles(request.PlaceActivityMedium, "PlaceActivityMedia");
            if (!photoSaveResult.Success)
            {
                throw new Exception(photoSaveResult.Message);
            }

            foreach (var photoUrl in photoSaveResult.Data.imageUrls)
            {
                var photo = new PlaceActivityMedium
                {
                    PlaceActivityId = placeActivity.Id,
                    CreateDate = DateTime.Now,
                    Type = "Image",
                    Url = photoUrl
                };
                await _unitOfWork.RepositoryPlaceActivityMedium.Insert(photo);
            }
            foreach (var mediaUrl in photoSaveResult.Data.videoUrls)
            {
                var media = new PlaceActivityMedium
                {
                    PlaceActivityId = placeActivity.Id,
                    CreateDate = DateTime.Now,
                    Type = "Video",
                    Url = mediaUrl
                };
                await _unitOfWork.RepositoryPlaceActivityMedium.Insert(media);
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

        public async Task<PlaceActivity> GetActivityById(int placeid, int activityid, string languageCode)
        {
            var place = await _unitOfWork.RepositoryPlace.GetById(placeid);
            if (place == null)
            {
                throw new KeyNotFoundException($"Place with ID {placeid} not found.");
            }

            var activityEntity = await _unitOfWork.RepositoryPlaceActivity.GetAll()
                .Include(p => p.PlaceActivityMedia)
                .Include(p => p.PlaceActivityTranslations.Where(pt => pt.LanguageCode == languageCode))
                .FirstOrDefaultAsync(e => e.Id == activityid && e.PlaceId == placeid);

            if (activityEntity == null)
            {
                throw new KeyNotFoundException($"PlaceActivity with ID {activityid} for Place ID {placeid} not found.");
            }
            activityEntity.PlaceActivityMedia = activityEntity.PlaceActivityMedia
                .OrderByDescending(pm => pm.Type == "Video")
                .ThenBy(pm => pm.Id)
                .ToList();
            return activityEntity;
        }

        public async Task<PlaceActivityRequest> UpdateActivity(int placeid, int activityid, PlaceActivityRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            var place = await _unitOfWork.RepositoryPlace.GetById(placeid);
            if (place == null)
            {
                throw new ArgumentException($"Place with id {placeid} not found.");
            }
            var existingActivity = await _unitOfWork.RepositoryPlaceActivity.GetById(activityid);
            if (existingActivity == null)
            {
                throw new ArgumentException($"Event with id {activityid} not found.");
            }
            if (existingActivity.PlaceId != placeid)
            {
                throw new InvalidOperationException($"Activity with id {activityid} does not belong to place with id {placeid}.");
            }
            if (!string.IsNullOrEmpty(existingActivity.PhotoDisplay))
            {
                await _fileService.DeleteFile(existingActivity.PhotoDisplay);
            }
            var photos = await _fileService.SaveImageFile(request.PhotoDisplay, "PlaceActivityMedia");
            existingActivity.DisplayNumber = request.DisplayNumber;
            existingActivity.PhotoDisplay = photos.Data;
            existingActivity.PlaceId = placeid;

            var existingMedia = await _unitOfWork.RepositoryPlaceActivityMedium.GetAll()
                                                                       .Where(e => e.PlaceActivityId == activityid)
                                                                       .ToListAsync();
            foreach (var media in existingMedia)
            {
                 _unitOfWork.RepositoryPlaceActivityMedium.Delete(media);
            }

            var existingTranslations = await _unitOfWork.RepositoryPlaceActivityTranslation.GetAll()
                                                                       .Where(e => e.PlaceActivityId == activityid)
                                                                       .ToListAsync();
            foreach (var translation in existingTranslations)
            {
                 _unitOfWork.RepositoryPlaceActivityTranslation.Delete(translation);
            }
            await _unitOfWork.CommitAsync();
            var photoSaveResult = await _fileService.SaveStaticFiles(request.PlaceActivityMedium, "PlaceActivityMedia");
            if (!photoSaveResult.Success)
            {
                throw new Exception(photoSaveResult.Message);
            }

            foreach (var photoUrl in photoSaveResult.Data.imageUrls)
            {
                var photo = new PlaceActivityMedium
                {
                    PlaceActivityId = existingActivity.Id,
                    CreateDate = DateTime.Now,
                    Type = "Image",
                    Url = photoUrl
                };
                await _unitOfWork.RepositoryPlaceActivityMedium.Insert(photo);
            }
            foreach (var mediaUrl in photoSaveResult.Data.videoUrls)
            {
                var media = new PlaceActivityMedium
                {
                    PlaceActivityId = existingActivity.Id,
                    CreateDate = DateTime.Now,
                    Type = "Video",
                    Url = mediaUrl
                };
                await _unitOfWork.RepositoryPlaceActivityMedium.Insert(media);
            }
            foreach (var translation in request.PlaceActivityTranslations)
            {
                var translationEntity = new PlaceActivityTranslation
                {
                    PlaceActivityId = existingActivity.Id,
                    LanguageCode = translation.LanguageCode,
                    ActivityName = translation.ActivityName,
                    Price = translation.Price,
                    Description = translation.Description,
                    PriceType = translation.PriceType,
                    Discount = translation.Discount,

                };
                await _unitOfWork.RepositoryPlaceActivityTranslation.Insert(translationEntity);
            }
            _unitOfWork.RepositoryPlaceActivity.Update(existingActivity);
            await _unitOfWork.CommitAsync();
            return request;
        }
        public async Task<bool> DeletePlaceActivity(int placeid, int activityid)
        {
            var places = await _unitOfWork.RepositoryPlace.GetById(placeid);
            if (places == null)
            {
                throw new ArgumentException($"Place with id {placeid} not found.");
            }
            var activity = await _unitOfWork.RepositoryPlaceActivity.GetById(activityid);
            if (activity != null)
            {
                    _unitOfWork.RepositoryPlaceActivity.Delete(activity);
            }
            await _unitOfWork.CommitAsync();
            return true;

        }
    }
}
