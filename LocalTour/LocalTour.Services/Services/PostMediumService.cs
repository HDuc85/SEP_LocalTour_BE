using AutoMapper;
using LocalTour.Data.Abstract;
using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;
using LocalTour.Services.ViewModel;
using Microsoft.EntityFrameworkCore;
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

            //mediaEntity.PostId = mediaRequest.PostId ?? mediaEntity.PostId; 
            mediaEntity.Type = mediaRequest.Type;
            mediaEntity.Url = mediaRequest.Url;

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

        public async Task<List<PostMedium>> GetAllMediaByPostId(int postId)
        {
            var mediaEntities = await _unitOfWork.RepositoryPostMedium.GetAll()
                .Where(m => m.PostId == postId)
                .ToListAsync();
            return _mapper.Map<List<PostMedium>>(mediaEntities);
        }
    }
}
