using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;
using LocalTour.Services.ViewModel;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LocalTour.Data.Abstract;
using Microsoft.EntityFrameworkCore;

namespace LocalTour.Services.Services
{
    public class PostLikeService : IPostLikeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PostLikeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> LikePostAsync(int postId, Guid userId)
        {
            // Check if the like already exists
            var existingLike = await _unitOfWork.RepositoryPostLike
                .GetAll()
                .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);

            if (existingLike != null)
            {
                return false; // Already liked
            }

            var like = new PostLike
            {
                PostId = postId,
                UserId = userId,
                CreatedDate = DateTime.UtcNow
            };

            await _unitOfWork.RepositoryPostLike.Insert(like);
            await _unitOfWork.CommitAsync();

            return true;
        }

        public async Task<bool> UnlikePostAsync(int postId, Guid userId)
        {
            var existingLike = await _unitOfWork.RepositoryPostLike
                .GetAll()
                .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);

            if (existingLike == null)
            {
                return false; // Not liked yet
            }

            _unitOfWork.RepositoryPostLike.Delete(existingLike);
            await _unitOfWork.CommitAsync();

            return true;
        }

        public async Task<List<Guid>> GetUserLikesByPostIdAsync(int postId)
        {
            // Get all likes for the post from repository
            var likes = await _unitOfWork.RepositoryPostLike.GetData(l => l.PostId == postId);

            // Return list of UserId from filtered likes
            return likes.Select(l => l.UserId).ToList();
        }

        public async Task<int> GetTotalLikesByPostIdAsync(int postId)
        {
            var likes = await _unitOfWork.RepositoryPostLike.GetData(l => l.PostId == postId);
            return likes.Count(); // Return total number of likes
        }
    }
}
