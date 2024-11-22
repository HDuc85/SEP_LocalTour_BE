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
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace LocalTour.Services.Services
{
    public class PlaceService : IPlaceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PlaceService(IUnitOfWork unitOfWork, IMapper mapper,IUserService userService, IFileService fileService, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileService = fileService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<PlaceRequest> CreatePlace(PlaceRequest place)
        {
            if (place == null)
            {
                throw new ArgumentNullException(nameof(place));
            }
            var user = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(user) || !Guid.TryParse(user, out var userId))
            {
                throw new UnauthorizedAccessException("User not found or invalid User ID.");
            }
            var photos = await _fileService.SaveImageFile(place.PhotoDisplay);
            var placeEntity = new Place
            {
                WardId = place.WardId,
                TimeOpen = place.TimeOpen,
                TimeClose = place.TimeClose,
                Longitude = place.Longitude,
                Latitude = place.Latitude,
                PhotoDisplay = photos.Data,
                ContactLink = place.ContactLink,
                AuthorId = userId,
                Status = "0",
            };
            await _unitOfWork.RepositoryPlace.Insert(placeEntity);
            await _unitOfWork.CommitAsync();
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
                    PlaceId = placeEntity.Id,
                    LanguageCode = translation.LanguageCode,
                    Name = translation.Name,
                    Description = translation.Description,
                    Address = translation.Address,
                    Contact = translation.Contact
                };
                await _unitOfWork.RepositoryPlaceTranslation.Insert(translationEntity);
            }
            var photoSaveResult = await _fileService.SaveStaticFiles(place.PlaceMedia);
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

        public async Task<PaginatedList<PlaceVM>> GetAllPlace(GetPlaceRequest request)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            User user = new User();
            if (userId != null)
            {
                user = await _userService.FindById(userId);
            }
            List<int> userTags = new List<int>();
            if (user != null)
            {  
                userTags = await _unitOfWork.RepositoryUserPreferenceTags.GetAll()
                    .Where(mt => mt.UserId == user.Id)
                    .Select(mt => mt.TagId)
                    .ToListAsync();
            }
            
            var roles = _httpContextAccessor.HttpContext.User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();
            
            IQueryable<Place> places;
            if (roles.Contains("Visitor") || roles.IsNullOrEmpty())
            {
                places = _unitOfWork.RepositoryPlace.GetAll().Include(x => x.PlaceTranslations.Where(pt => pt.LanguageCode == request.LanguageCode))
                        .Include(y => y.PlaceTags)
                        .Include(z => z.PlaceActivities)
                        .Include(r => r.PlaceMedia)
                        .Include(w => w.Ward)
                        .Where(r => r.Status == "1")
                        .AsQueryable();

            }
            else
            {
                places = _unitOfWork.RepositoryPlace.GetAll().Include(x => x.PlaceTranslations.Where(pt => pt.LanguageCode == request.LanguageCode))
                           .Include(y => y.PlaceTags)
                           .Include(z => z.PlaceActivities)
                           .Include(r => r.PlaceMedia)
                           .Include(w => w.Ward)
                           .AsQueryable();

            }
            if (request.Tags != null && request.Tags.Any())
            {
                places = places.Where(p => p.PlaceTags.Any(pt => request.Tags.Contains(pt.TagId)));
            }
            if (request.SearchTerm is not null)
            {
                places = places.Where(x => x.PlaceTranslations.Any(pt => pt.Name.Contains(request.SearchTerm)) ||
                                           x.PlaceTranslations.Any(pt => pt.Address.Contains(request.SearchTerm)));
            }
            return await places
                .ListPaginateWithSortPlaceAsync<Place, PlaceVM>(
                    request.Page,
                    request.Size,
                    request.SortBy,
                    request.SortOrder,
                    request.CurrentLongitude,
                    request.CurrentLatitude,
                    userTags,
                    user.Id,
                    _mapper.ConfigurationProvider);
        }
        private double CalculateDistance(double longitude1, double latitude1, double longitude2, double latitude2)
        {
            var R = 6371; // Radius of the Earth in kilometers
            var dLat = ToRadians(latitude2 - latitude1);
            var dLon = ToRadians(longitude2 - longitude1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(latitude1)) * Math.Cos(ToRadians(latitude2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var distance = R * c; // Distance in kilometers
            return distance;
        }

        private double ToRadians(double degree)
        {
            return degree * Math.PI / 180.0;
        }

        public async Task<PlaceDetailModel> GetPlaceById(string languageCode, int placeid)
        {
            var placeEntity = await _unitOfWork.RepositoryPlace.GetAll()
                .Include(p => p.PlaceMedia)
                .Include(p => p.PlaceTranslations.Where(pt => pt.LanguageCode == languageCode))
                .Include(p => p.PlaceActivities)
                .ThenInclude(pa => pa.PlaceActivityTranslations.Where(pat => pat.LanguageCode == languageCode) )
                .Include(p => p.PlaceActivities)
                .ThenInclude(pa => pa.PlaceActivityMedia)
                .FirstOrDefaultAsync(e => e.Id == placeid);
            
            var placeFeedback = await _unitOfWork.RepositoryPlaceFeeedback.GetAll().Where(p => p.PlaceId == placeid).ToListAsync();
            
            double AverageRating = placeFeedback.Any() ? placeFeedback.Average(feedback => feedback.Rating) : 0;
            

            if (placeEntity == null)
            {
                throw new KeyNotFoundException($"Place with ID {placeid} not found.");
            }
            
            
            
            placeEntity.PlaceMedia = placeEntity.PlaceMedia
            .OrderByDescending(pm => pm.Type == "Video")
            .ThenBy(pm => pm.Id)
            .ToList();
         
            
            placeEntity.PlaceActivities = placeEntity.PlaceActivities.OrderBy(x => x.DisplayNumber).ToList();
            PlaceDetailModel result = new PlaceDetailModel
            {
                Id = placeEntity.Id,
                Latitude = placeEntity.Latitude,
                Longitude = placeEntity.Longitude,
                Status = placeEntity.Status,
                ContactLink = placeEntity.ContactLink,
                AuthorId = placeEntity.AuthorId,
                PhotoDisplay = placeEntity.PhotoDisplay,
                TimeClose = placeEntity.TimeClose,
                TimeOpen = placeEntity.TimeOpen,
                ApprovedTime = placeEntity.ApprovedTime,
                Approver = placeEntity.Approver,
                Destinations = placeEntity.Destinations,
                Events = placeEntity.Events,
                Posts = placeEntity.Posts,
                Ward = placeEntity.Ward,
                ApproverId = placeEntity.ApproverId,
                PlaceMedia = placeEntity.PlaceMedia,
                PlaceTranslations = placeEntity.PlaceTranslations,
                PlaceActivities = placeEntity.PlaceActivities,
                MarkPlaces = placeEntity.MarkPlaces,
                PlaceFeeedbacks = placeEntity.PlaceFeeedbacks,
                PlaceTags = placeEntity.PlaceTags,
                PlaceReports = placeEntity.PlaceReports,
                WardId = placeEntity.WardId,
                TraveledPlaces = placeEntity.TraveledPlaces,
                PlaceSearchHistories = placeEntity.PlaceSearchHistories,
                Rating = AverageRating,
            };

            return result;
        }

        public async Task<List<TagViewModel>> GetTagsByPlaceId(int placeid)
        {
            var places = await _unitOfWork.RepositoryPlaceTag.GetAll().Where(p => p.PlaceId == placeid)
                .Include(pt => pt.Tag)
                .Select(p => new TagViewModel
                {
                    Id = p.TagId,
                    TagName = p.Tag.TagName,
                    TagPhotoUrl = p.Tag.TagPhotoUrl
                }).ToListAsync();
            return places;
        }
        public async Task<PlaceRequest> UpdatePlace(int placeid, PlaceRequest request)
        {
            var existingPlace = await _unitOfWork.RepositoryPlace.GetById(placeid);
            if (existingPlace == null)
            {
                throw new ArgumentException($"Event with id {placeid} not found.");
            }
            if (!string.IsNullOrEmpty(existingPlace.PhotoDisplay))
            {
                await _fileService.DeleteFile(existingPlace.PhotoDisplay);
            }
            var photos = await _fileService.SaveImageFile(request.PhotoDisplay);
            existingPlace.WardId = request.WardId;
            existingPlace.TimeOpen = request.TimeOpen;
            existingPlace.TimeClose = request.TimeClose;
            existingPlace.Longitude = request.Longitude;
            existingPlace.Latitude = request.Latitude;
            existingPlace.PhotoDisplay = photos.Data;
            existingPlace.Status = "0";
            existingPlace.ContactLink = request.ContactLink;
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
            await _unitOfWork.CommitAsync();
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
                    PlaceId = existingPlace.Id,
                    LanguageCode = translation.LanguageCode,
                    Name = translation.Name,
                    Description = translation.Description,
                    Address = translation.Address,
                    Contact = translation.Contact
                };
                await _unitOfWork.RepositoryPlaceTranslation.Insert(translationEntity);
            }
            var photoSaveResult = await _fileService.SaveStaticFiles(request.PlaceMedia);
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

        public async Task<Place> ChangeStatusPlace(int placeid, string status)
        {
            var existingPlace = await _unitOfWork.RepositoryPlace.GetById(placeid);
            if (existingPlace == null)
            {
                throw new ArgumentException($" {placeid} not found.");
            }
            var userid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userid, out var userId))
            {
                throw new InvalidOperationException("User ID is not a valid GUID.");
            }
            existingPlace.Status = status;
            existingPlace.ApprovedTime = DateTime.Now;
            existingPlace.ApproverId = userId;
            _unitOfWork.RepositoryPlace.Update(existingPlace);
            await _unitOfWork.CommitAsync();
            return existingPlace;
        }
        public async Task<bool> DeletePlace(int placeid)
        {
            var places = await _unitOfWork.RepositoryPlace.GetById(placeid);
            if (places == null)
            {
                throw new ArgumentException($"Place with id {placeid} not found.");
            }
            _unitOfWork.RepositoryPlace.Delete(places);
            var events = await _unitOfWork.RepositoryEvent.GetData(e => e.PlaceId == placeid);
            if (events != null && events.Any())
            {
                foreach (var eventEntity in events)
                {
                    _unitOfWork.RepositoryEvent.Delete(eventEntity);
                }
            }
            var activity = await _unitOfWork.RepositoryPlaceActivity.GetData(e => e.PlaceId == placeid);
            if (activity != null && activity.Any())
            {
                foreach (var activityEntity in activity)
                {
                    _unitOfWork.RepositoryPlaceActivity.Delete(activityEntity);
                }
            }
            var tag = await _unitOfWork.RepositoryPlaceTag.GetData(e => e.PlaceId == placeid);
            if (tag != null && tag.Any())
            {
                foreach (var tagEntity in tag)
                {
                    _unitOfWork.RepositoryPlaceTag.Delete(tagEntity);
                }
            }
            await _unitOfWork.CommitAsync();
            return true;

        }
        public async Task<PaginatedList<PlaceVM>> GetAllPlaceByRole(GetPlaceRequest request)
        {
            var user = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(user) || !Guid.TryParse(user, out var userId))
            {
                throw new UnauthorizedAccessException("User not found or invalid User ID.");
            }
            var userTags = await _unitOfWork.RepositoryUserPreferenceTags.GetAll()
                .Where(mt => mt.UserId == userId)
                .Select(mt => mt.TagId)
                .ToListAsync();
            var modTags = await _unitOfWork.RepositoryModTag.GetAll()
                .Where(mt => mt.UserId == userId)
                .Select(mt => mt.TagId)
                .ToListAsync();
            var roles = _httpContextAccessor.HttpContext.User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();
            if (!roles.Any())
            {
                throw new UnauthorizedAccessException("User has no assigned roles.");
            }
            IQueryable<Place> places;
            if (roles.Contains("Service Owner"))
            {
                places = _unitOfWork.RepositoryPlace.GetAll().Include(x => x.PlaceTranslations.Where(pt => pt.LanguageCode == request.LanguageCode))
                        .Include(y => y.PlaceTags)
                        .Include(z => z.PlaceActivities)
                        .Include(r => r.PlaceMedia)
                        .Include(w => w.Ward)
                        .Where(r => r.AuthorId == userId)
                        .AsQueryable();

            }
            else if (roles.Contains("Moderator"))
            {
                places = _unitOfWork.RepositoryPlace.GetAll().Include(x => x.PlaceTranslations.Where(pt => pt.LanguageCode == request.LanguageCode))
                           .Where(y => y.PlaceTags.Any(pt => modTags.Contains(pt.TagId)))
                           .Include(z => z.PlaceActivities)
                           .Include(r => r.PlaceMedia)
                           .Include(w => w.Ward)
                           .AsQueryable();

            } else
            {
                throw new UnauthorizedAccessException("You do not have permission to access this page");
            }
            if (request.Tags != null && request.Tags.Any())
            {
                places = places.Where(p => p.PlaceTags.Any(pt => request.Tags.Contains(pt.TagId)));
            }
            if (request.SearchTerm is not null)
            {
                places = places.Where(x => x.PlaceTranslations.Any(pt => pt.Name.Contains(request.SearchTerm)) ||
                                           x.PlaceTranslations.Any(pt => pt.Address.Contains(request.SearchTerm)));
            }
            return await places
                .ListPaginateWithSortPlaceAsync<Place, PlaceVM>(
                    request.Page,
                    request.Size,
                    request.SortBy,
                    request.SortOrder,
                    request.CurrentLongitude,
                    request.CurrentLatitude,
                    userTags,
                    userId,
                    _mapper.ConfigurationProvider);
        }
    }

}
