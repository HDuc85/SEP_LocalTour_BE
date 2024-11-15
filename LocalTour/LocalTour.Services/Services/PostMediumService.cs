﻿using AutoMapper;
using LocalTour.Data.Abstract;
using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;
using LocalTour.Services.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocalTour.Services.Services
{
    public class PostMediumService : IPostMediumService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PostMediumService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PostMediumRequest> CreateMedia(PostMediumRequest mediaRequest)
        {
            if (mediaRequest == null) throw new ArgumentNullException(nameof(mediaRequest));

            var mediaEntity = _mapper.Map<PostMedium>(mediaRequest);
            mediaEntity.CreateDate = DateTime.UtcNow;

            await _unitOfWork.RepositoryPostMedium.Insert(mediaEntity);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<PostMediumRequest>(mediaEntity);
        }

        public async Task<PostMediumRequest?> GetMediaById(int mediaId)
        {
            var mediaEntity = await _unitOfWork.RepositoryPostMedium.GetById(mediaId);
            return mediaEntity == null ? null : _mapper.Map<PostMediumRequest>(mediaEntity);
        }

        public async Task<PostMediumRequest?> UpdateMedia(int mediaId, PostMediumRequest mediaRequest)
        {
            var mediaEntity = await _unitOfWork.RepositoryPostMedium.GetById(mediaId);
            if (mediaEntity == null) return null;

            // Update only the fields you want to change
            mediaEntity.PostId = mediaRequest.PostId ?? mediaEntity.PostId; 
            mediaEntity.Type = mediaRequest.Type;
            mediaEntity.Url = mediaRequest.Url;

            // Ensure CreateDate remains unchanged or update as per your requirement
            // mediaEntity.CreateDate = mediaRequest.CreateDate; // Uncomment if you want to update it

            _unitOfWork.RepositoryPostMedium.Update(mediaEntity);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<PostMediumRequest>(mediaEntity);
        }


        public async Task<bool> DeleteMedia(int mediaId)
        {
            var mediaEntity = await _unitOfWork.RepositoryPostMedium.GetById(mediaId);
            if (mediaEntity == null) return false;

            _unitOfWork.RepositoryPostMedium.Delete(mediaEntity);
            await _unitOfWork.CommitAsync();

            return true;
        }

        public async Task<List<PostMediumRequest>> GetAllMediaByPostId(int postId)
        {
            var mediaEntities = _unitOfWork.RepositoryPostMedium.GetAll().Where(m => m.PostId == postId).ToList();
            return _mapper.Map<List<PostMediumRequest>>(mediaEntities);
        }
    }
}
