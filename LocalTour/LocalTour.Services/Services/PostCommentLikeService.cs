using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;
using LocalTour.Services.ViewModel;
using AutoMapper;
using System;
using System.Threading.Tasks;
using LocalTour.Data.Abstract;
using Microsoft.EntityFrameworkCore;

namespace LocalTour.Services.Services
{
    public class PostCommentLikeService : IPostCommentLikeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PostCommentLikeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> LikeCommentAsync(int commentId, Guid userId)
        {
            var existingLike = await _unitOfWork.RepositoryPostCommentLike
                .GetAll()
                .FirstOrDefaultAsync(l => l.PostCommentId == commentId && l.UserId == userId);

            if (existingLike != null)
            {
                _unitOfWork.RepositoryPostCommentLike.Delete(existingLike);
                await _unitOfWork.CommitAsync();
                return false; 
            }

            // Nếu chưa like thì thêm like
            var like = new PostCommentLike
            {
                PostCommentId = commentId,
                UserId = userId,
                CreatedDate = DateTime.UtcNow
            };

            await _unitOfWork.RepositoryPostCommentLike.Insert(like);
            await _unitOfWork.CommitAsync();
            return true;
        }
        public async Task<bool> UnlikeCommentAsync(int commentId, Guid userId)
        {
            var existingLike = await _unitOfWork.RepositoryPostCommentLike
                .GetAll()
                .FirstOrDefaultAsync(l => l.PostCommentId == commentId && l.UserId == userId);

            if (existingLike == null)
            {
                return false; // Not liked yet
            }

            _unitOfWork.RepositoryPostCommentLike.Delete(existingLike);
            await _unitOfWork.CommitAsync();

            return true;
        }

        public async Task<List<Guid>> GetUserLikesByCommentIdAsync(int postCommentId)
        {
            // Lấy tất cả likes cho comment từ repository
            var likes = await _unitOfWork.RepositoryPostCommentLike.GetData(like => like.PostCommentId == postCommentId);

            // Trả về danh sách UserId từ likes đã lọc
            return likes.Select(l => l.UserId).ToList();
        }

        public async Task<int> GetTotalLikesByCommentIdAsync(int postCommentId)
        {
            var likes = await _unitOfWork.RepositoryPostCommentLike.GetData(like => like.PostCommentId == postCommentId);
            return likes.Count(); // Trả về tổng số lượt like
        }

    }
}
