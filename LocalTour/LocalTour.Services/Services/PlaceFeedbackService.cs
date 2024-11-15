﻿using AutoMapper;
using LocalTour.Data.Abstract;
using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;
using LocalTour.Services.Extensions;
using LocalTour.Services.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LocalTour.Services.Services
{
    public class PlaceFeedbackService : IPlaceFeedbackService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFileService _fileService;

        public PlaceFeedbackService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _fileService = fileService;
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
            var lastFeedbacks = await _unitOfWork.RepositoryPlaceFeeedback.GetData(f => f.PlaceId == placeid && f.UserId == userId);
            var lastFeedback = lastFeedbacks.OrderByDescending(f => f.CreatedDate).FirstOrDefault();
            if (lastFeedback != null)
            {
                var daysSinceLastFeedback = (DateTime.UtcNow - lastFeedback.CreatedDate).TotalDays;
                if (daysSinceLastFeedback < 7)
                {
                    throw new InvalidOperationException("You can only provide feedback every 7 days for the same place.");
                }
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
            await _unitOfWork.CommitAsync();
            var mediaSaveResult = await _fileService.SaveStaticFiles(request.PlaceFeedbackMedia);
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
        public async Task<bool> DeleteFeedback(int placeid, int feedbackid)
        {
            var places = await _unitOfWork.RepositoryPlace.GetById(placeid);
            if (places == null)
            {
                throw new ArgumentException($"Place with id {placeid} not found.");
            }
            var existingMedia = await _unitOfWork.RepositoryPlaceFeeedbackMedium.GetAll()
                                         .Where(e => e.FeedbackId == feedbackid)
                                         .ToListAsync();
            foreach (var media in existingMedia)
            {
                _unitOfWork.RepositoryPlaceFeeedbackMedium.Delete(media);
            }
            var feedbackEntity = await _unitOfWork.RepositoryPlaceFeeedback.GetById(feedbackid);
            if (feedbackEntity != null)
            {
                _unitOfWork.RepositoryPlaceFeeedback.Delete(feedbackEntity);
            }
            await _unitOfWork.CommitAsync();
            return true;
        }

        public async Task<PlaceFeedbackRequest> UpdateFeedback(int placeid, int feedbackid, PlaceFeedbackRequest request)
        {
            var places = await _unitOfWork.RepositoryPlace.GetById(placeid);
            if (places == null)
            {
                throw new ArgumentException($"Place with id {placeid} not found.");
            }
            var feedback = await _unitOfWork.RepositoryPlaceFeeedback.GetById(feedbackid);
            if (feedback == null)
            {
                throw new ArgumentException($"Feedback with id {feedbackid} not found.");
            }
            var userid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userid, out var userId))
            {
                throw new InvalidOperationException("User ID is not a valid GUID.");
            }
            var existingMedia = await _unitOfWork.RepositoryPlaceFeeedbackMedium.GetAll()
                                                     .Where(e => e.FeedbackId == feedbackid)
                                                     .ToListAsync();
            foreach (var media in existingMedia)
            {
                _unitOfWork.RepositoryPlaceFeeedbackMedium.Delete(media);
            }
            if (userId == feedback.UserId)
            {
                feedback.Rating = request.Rating;
                feedback.Content = request.Content;
                feedback.CreatedDate = DateTime.UtcNow;
                var mediaSaveResult = await _fileService.SaveStaticFiles(request.PlaceFeedbackMedia);
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
            }
            else
            {
                throw new InvalidOperationException("Wrong user.");
            }
            _unitOfWork.RepositoryPlaceFeeedback.Update(feedback);
            await _unitOfWork.CommitAsync();

            return request;
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
