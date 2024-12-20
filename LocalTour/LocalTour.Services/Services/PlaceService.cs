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
using Net.payOS;
using Net.payOS.Types;
using Org.BouncyCastle.Asn1.Ocsp;

namespace LocalTour.Services.Services
{
    public class PlaceService : IPlaceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly PayOS _payOS;
        private readonly IMailService _mailService;

        public PlaceService(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService, IFileService fileService, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, PayOS payOS, IMailService mailService)
        {
            _userService = userService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileService = fileService;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _payOS = payOS;
            _mailService = mailService;
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
                Status = "Unpaid",
                CreatedDate = DateTime.UtcNow,
                //BRC = brcs.Data,
            };
            if(place.BRC != null)
            {
            var brcs = await _fileService.SaveImageFile(place.BRC);
                placeEntity.BRC=brcs.Data;
            }
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
            foreach (var mediaUrl in place.PlaceMedia)
            {
                string mediaType = "Unknown";
                var fileExtension = Path.GetExtension(mediaUrl).ToLower();
                if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png" || fileExtension == ".gif")
                {
                    mediaType = "Image";
                }
                else if (fileExtension == ".mp4" || fileExtension == ".avi" || fileExtension == ".mkv")
                {
                    mediaType = "Video";
                }

                var placeMedium = new PlaceMedium
                {
                    PlaceId = placeEntity.Id,
                    CreateDate = DateTime.Now,
                    Type = mediaType,
                    Url = mediaUrl
                };

                await _unitOfWork.RepositoryPlaceMedium.Insert(placeMedium);
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
                        .Where(r => r.Status == "Approved")
                        .AsQueryable();

            }
            else
            {
                places = _unitOfWork.RepositoryPlace.GetAll().Include(x => x.PlaceTranslations.Where(pt => pt.LanguageCode == request.LanguageCode))
                           .Include(y => y.PlaceTags)
                           .Include(z => z.PlaceActivities)
                           .Include(r => r.PlaceMedia)
                           .Include(w => w.Ward)
                           .Where(r => r.Status == "Approved")
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
                    request.Distance,
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

        public async Task<PlaceDetailModel> GetPlaceById(string? languageCode, int placeid)
        {
            var placeEntity = await _unitOfWork.RepositoryPlace.GetDataQueryable()
                .Include(p => p.PlaceMedia)
                .Include(p => p.PlaceTranslations)
                .Include(p => p.PlaceActivities)
                .ThenInclude(pa => pa.PlaceActivityTranslations)
                .Include(p => p.PlaceActivities)
                .ThenInclude(pa => pa.PlaceActivityMedia)
                .FirstOrDefaultAsync(e => e.Id == placeid);

            var placeFeedback = await _unitOfWork.RepositoryPlaceFeeedback.GetAll().Where(p => p.PlaceId == placeid).ToListAsync();

            if (languageCode != null)
            {
                if (placeEntity.PlaceTranslations.Count > 0)
                {
                    placeEntity.PlaceTranslations = placeEntity.PlaceTranslations.Where(p => p.LanguageCode.Contains(languageCode)).ToList();
                }

                if (placeEntity.PlaceActivities.Count > 0)
                {
                    placeEntity.PlaceActivities = placeEntity.PlaceActivities
                        .Where(p => p.PlaceActivityTranslations
                            .Any(a => a.LanguageCode.Contains(languageCode))).ToList();
                }

            }

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
                BRC =placeEntity.BRC,
                CreatedDate = placeEntity.CreatedDate,
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
                    TagVi = p.Tag.TagVi,
                    TagPhotoUrl = p.Tag.TagPhotoUrl
                }).ToListAsync();
            return places;
        }
        public async Task<PlaceUpdateRequest> UpdatePlace(int placeid, PlaceUpdateRequest request)
        {
            var existingPlace = await _unitOfWork.RepositoryPlace.GetById(placeid);
            if (existingPlace == null)
            {
                throw new ArgumentException($"Event with id {placeid} not found.");
            }
            existingPlace.PhotoDisplay = request.PhotoDisplay;
            existingPlace.BRC = request.brc;
            existingPlace.WardId = request.WardId;
            existingPlace.TimeOpen = request.TimeOpen;
            existingPlace.TimeClose = request.TimeClose;
            existingPlace.Longitude = request.Longitude;
            existingPlace.Latitude = request.Latitude;
            if (existingPlace.Status != "Unpaid")
            {
                existingPlace.Status = "Pending";
            }
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
            foreach (var mediaUrl in request.PlaceMedia)
            {
                string mediaType = "Unknown";
                var fileExtension = Path.GetExtension(mediaUrl).ToLower();
                if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png" || fileExtension == ".gif")
                {
                    mediaType = "Image";
                }
                else if (fileExtension == ".mp4" || fileExtension == ".avi" || fileExtension == ".mkv")
                {
                    mediaType = "Video";
                }

                var placeMedium = new PlaceMedium
                {
                    PlaceId = existingPlace.Id,
                    CreateDate = DateTime.Now,
                    Type = mediaType,
                    Url = mediaUrl
                };

                await _unitOfWork.RepositoryPlaceMedium.Insert(placeMedium);
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
        public async Task<Place> TransferAuthor(int placeId, Guid userIdTransfer)
        {
            var existingPlace = await _unitOfWork.RepositoryPlace.GetById(placeId);
            if (existingPlace == null)
            {
                throw new ArgumentException($" {placeId} not found.");
            }
            var userid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userid, out var userId))
            {
                throw new InvalidOperationException("User ID is not a valid GUID.");
            }

            if (existingPlace.AuthorId != userId)
            {
                throw new InvalidOperationException("User ID is not the same as existing one.");
            }
            existingPlace.AuthorId = userIdTransfer;
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
                    _unitOfWork.RepositoryPlaceActivityTranslation.Delete(x => x.PlaceActivityId == activityEntity.PlaceId);
                    _unitOfWork.RepositoryPlaceActivityMedium.Delete(x => x.PlaceActivityId == activityEntity.PlaceId);
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

            var feedback = await _unitOfWork.RepositoryPlaceFeeedback.GetData(e => e.PlaceId == placeid);
            if (feedback != null && feedback.Any())
            {
                foreach (var item in feedback)
                {
                    _unitOfWork.RepositoryPlaceFeeedbackHelpful.Delete(x => x.Id == item.PlaceId);
                    _unitOfWork.RepositoryPlaceFeeedbackMedium.Delete(x => x.Id == item.PlaceId);
                }
            }

            _unitOfWork.RepositoryPlaceFeeedback.Delete(e => e.PlaceId == placeid);
            _unitOfWork.RepositoryPlaceMedium.Delete(x => x.PlaceId == placeid);
            _unitOfWork.RepositoryMarkPlace.Delete(x => x.PlaceId == placeid);
            _unitOfWork.RepositoryPlaceReport.Delete(x => x.PlaceId == placeid);
            _unitOfWork.RepositoryTraveledPlace.Delete(x => x.PlaceId == placeid);
            _unitOfWork.RepositoryPlaceTranslation.Delete(x => x.PlaceId == placeid);
            _unitOfWork.RepositoryModCheckPlace.Delete(x => x.PlaceId == placeid);
            _unitOfWork.RepositoryPlace.Delete(places);

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
                .Select(s => s.DistrictNcityId)
                .ToListAsync();

            if (request.DistrictNCityIds != null && request.DistrictNCityIds.Any())
            {
                modTags = modTags.Where(x => request.DistrictNCityIds.Contains(x)).ToList();
            }

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
                           .Include(z => z.PlaceActivities)
                           .Include(r => r.PlaceMedia)
                           .Include(w => w.Ward)
                           .Where(y => modTags.Contains(y.Ward.DistrictNcityId))
                           .AsQueryable();

            }
            else
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

            if (request.Status != null)
            {
                places = places.Where(p => p.Status == request.Status);
            }

            return await places
                .ListPaginateWithSortPlaceAsync<Place, PlaceVM>(
                    request.Page,
                    request.Size,
                    request.SortBy,
                    request.SortOrder,
                    request.Distance,
                    request.CurrentLongitude,
                    request.CurrentLatitude,
                    userTags,
                    userId,
                    _mapper.ConfigurationProvider);
        }

        public async Task<CreatePaymentResult> CreatePaymentPlaceRegister(int placeId, string userId)
        {
            var place = await _unitOfWork.RepositoryPlace.GetById(placeId);
            if (place == null)
            {
                throw new Exception("Place is not exists.");
            }

            if (place.Status != "Unpaid")
            {
                throw new Exception("Place is already paid.");
            }
            var user = await _unitOfWork.RepositoryUser.GetById(Guid.Parse(userId));
            if (place.AuthorId != user.Id)
            {
                throw new Exception("User does not have permission to access this page.");
            }

            long orderCode = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            ItemData item = new ItemData("Register place", 1, int.Parse(_configuration["PayOS:placeRegisterPrice"] ?? "50000"));
            List<ItemData> items = new List<ItemData>();
            items.Add(item);
            PaymentData paymentData = new PaymentData(orderCode,
                //2000,
                int.Parse(_configuration["PayOS:placeRegisterPrice"] ?? "50000"),
                $"Register placeId {place.Id}",
                items,
                $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/api/Place/PlaceCancelPayment",
                $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/api/Place/PlaceSuccessPayment");

            CreatePaymentResult createPayment = await _payOS.createPaymentLink(paymentData);
            _unitOfWork.RepositoryPlacePayment.Insert(new PlacePayment()
            {
                OrderCode = orderCode,
                PlaceId = place.Id,
                Status = "Created",
            });
            await _unitOfWork.CommitAsync();
            return createPayment;
        }

        public async Task<string> PlaceSuccessPayment(long orderCode, string status)
        {
            var placePayment = await _unitOfWork.RepositoryPlacePayment.GetDataQueryable(x => x.OrderCode == orderCode).FirstOrDefaultAsync();
            if (placePayment == null)
            {
                throw new Exception("OrderCode is not exists.");
            }
            var place = await _unitOfWork.RepositoryPlace.GetById(placePayment.PlaceId);
            placePayment.Status = status;
            place.Status = "Pending";
            _unitOfWork.RepositoryPlacePayment.Update(placePayment);
            _unitOfWork.RepositoryPlace.Update(place);
            await _unitOfWork.CommitAsync();

            return _configuration["PayOS:successUrl"];
        }

        public async Task<string> PlaceCancelPayment(long orderCode, string status)
        {
            var placePayment = await _unitOfWork.RepositoryPlacePayment.GetDataQueryable(x => x.OrderCode == orderCode).FirstOrDefaultAsync();
            if (placePayment == null)
            {
                throw new Exception("OrderCode is not exists.");
            }
            placePayment.Status = status;
            _unitOfWork.RepositoryPlacePayment.Update(placePayment);
            await _unitOfWork.CommitAsync();
            return _configuration["PayOS:cancelUrl"];
        }

        public async Task<bool> sendMail(SendMailRequest request)
        {
            var place = await _unitOfWork.RepositoryPlace.GetById(request.PlaceId);
            if (place == null)
            {
                throw new Exception("Place is not exists.");
            }
            var author = await _unitOfWork.RepositoryUser.GetById(place.AuthorId);
            var placemodel = await _unitOfWork.RepositoryPlace.GetDataQueryable(x => x.Id == request.PlaceId)
                .Include(y => y.PlaceTranslations)
                .FirstOrDefaultAsync();

            string placeName = "name is:";
            foreach (var item in placemodel.PlaceTranslations)
            {
                placeName = $"{placeName} / {item.Name}";
            }

            try
            {
                _mailService.SendEmail(new SendEmailModel()
                {
                    ServiceOwnerName = author.UserName,
                    IsApproved = request.IsApproved,
                    To = author.Email,
                    RejectReason = request.RejectReason,
                    PlaceName = placeName,
                    Subject = request.IsApproved ? "Place Approved" : "Place Rejected"
                });
                return true;
            }
            catch (Exception e)
            {
                throw new Exception($"Error in server {e.Message}");
            }
        }
    }

}
