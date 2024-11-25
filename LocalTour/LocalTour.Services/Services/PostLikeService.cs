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

        public async Task<bool> ToggleLikePostAsync(int postId, Guid userId)
        {
            // Kiểm tra nếu người dùng đã "Like" bài viết
            var existingLike = await _unitOfWork.RepositoryPostLike
                .GetAll()
                .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);

            if (existingLike != null)
            {
                // Nếu đã "Like", xóa "Like" (Unlike)
                _unitOfWork.RepositoryPostLike.Delete(existingLike);
                await _unitOfWork.CommitAsync();
                return false; // Trả về false để chỉ ra hành động là "Unlike"
            }

            // Nếu chưa "Like", thêm "Like"
            var like = new PostLike
            {
                PostId = postId,
                UserId = userId,
                CreatedDate = DateTime.UtcNow
            };

            await _unitOfWork.RepositoryPostLike.Insert(like);
            await _unitOfWork.CommitAsync();
            return true; // Trả về true để chỉ ra hành động là "Like"
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

        public async Task<List<UserViewModel>> GetUserLikesByPostIdAsync(int postId)
        {
            var post = await _unitOfWork.RepositoryPost.GetById(postId);
            if (post == null)
            {
                throw new NullReferenceException("Post not found");
            }
            // Get all likes for the post from repository
            var likes =  _unitOfWork.RepositoryPostLike.GetDataQueryable(l => l.PostId == postId).Include(x => x.User);

            // Return list of UserId from filtered likes
            return likes.Select(l => new UserViewModel
            {
                userId = l.UserId,
                userName = l.User.UserName,
                userProfileImage = l.User.ProfilePictureUrl
            }).ToList();
        }

        public async Task<int> GetTotalLikesByPostIdAsync(int postId)
        {
            var likes = await _unitOfWork.RepositoryPostLike.GetData(l => l.PostId == postId);
            return likes.Count(); // Return total number of likes
        }
    }
}
