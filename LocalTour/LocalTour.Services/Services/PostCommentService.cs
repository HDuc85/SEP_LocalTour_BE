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
    public class PostCommentService : IPostCommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PostCommentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PostCommentRequest> CreateCommentAsync(PostCommentRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var commentEntity = _mapper.Map<PostComment>(request);
            commentEntity.CreatedDate = DateTime.UtcNow; // Set created date

            await _unitOfWork.RepositoryPostComment.Insert(commentEntity);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<PostCommentRequest>(commentEntity);
        }

        public async Task<PostCommentRequest?> GetCommentByIdAsync(int id, Guid userId)
        {
            var commentEntity = await _unitOfWork.RepositoryPostComment.GetById(id);
            if (commentEntity == null) return null;

            var commentRequest = _mapper.Map<PostCommentRequest>(commentEntity);
            // Add like status and total likes
            commentRequest.TotalLikes = commentEntity.PostCommentLikes.Count;
            commentRequest.LikedByUser = commentEntity.PostCommentLikes.Any(like => like.UserId == userId);

            return commentRequest;
        }

        public async Task<PostCommentRequest?> UpdateCommentAsync(int id, PostCommentRequest request)
        {
            var commentEntity = await _unitOfWork.RepositoryPostComment.GetById(id);
            if (commentEntity == null) return null;

            // Update only content
            commentEntity.Content = request.Content;
            // No UpdateDate property available, so skip it

            _unitOfWork.RepositoryPostComment.Update(commentEntity);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<PostCommentRequest>(commentEntity);
        }

        public async Task<bool> DeleteCommentAsync(int id)
        {
            var commentEntity = await _unitOfWork.RepositoryPostComment.GetById(id);
            if (commentEntity == null) return false;

            _unitOfWork.RepositoryPostComment.Delete(commentEntity);
            await _unitOfWork.CommitAsync();

            return true;
        }

        public async Task<List<PostCommentRequest>> GetCommentsByPostIdAsync(int postId, Guid userId)
        {
            var comments = await _unitOfWork.RepositoryPostComment
                .GetAll()
                .Where(c => c.PostId == postId)
                .OrderBy(c => c.ParentId) // Order by ParentId for parent-child relationship
                .ToListAsync();

            var commentRequests = _mapper.Map<List<PostCommentRequest>>(comments);

            // Populate likes count and user like status
            foreach (var commentRequest in commentRequests)
            {
                var commentEntity = comments.First(c => c.Id == commentRequest.Id);
                commentRequest.TotalLikes = commentEntity.PostCommentLikes.Count;
                commentRequest.LikedByUser = commentEntity.PostCommentLikes.Any(like => like.UserId == userId);
            }

            return commentRequests;
        }

        //List ra tất cả comment và medium dựa theo comment cha -> con 
        public async Task<List<PostMediumRequest>> GetAllMediaByPostId(int postId, PaginatedQueryParams queryParams)
        {
            // Fetch media related to the specific post ID
            var query = _unitOfWork.RepositoryPostMedium.GetAll()
                .Where(m => m.PostId == postId);

            // Sorting logic
            if (!string.IsNullOrEmpty(queryParams.SortBy))
            {
                query = queryParams.SortOrder?.ToLower() == "desc"
                    ? query.OrderByDescending(e => EF.Property<object>(e, queryParams.SortBy))
                    : query.OrderBy(e => EF.Property<object>(e, queryParams.SortBy));
            }

            // Paging logic
            var page = queryParams.Page ?? 1;
            var size = queryParams.Size ?? 10;
            var mediaEntities = await query
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();

            return _mapper.Map<List<PostMediumRequest>>(mediaEntities);
        }
    }
}
