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
        private readonly IPostCommentLikeService _postCommentLikeService; 

        public PostCommentService(IUnitOfWork unitOfWork, IMapper mapper, IPostCommentLikeService postCommentLikeService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _postCommentLikeService = postCommentLikeService; // Initialize service
        }

        public async Task<PostComment> CreateCommentAsync(CreatePostCommentRequest request)
        {
            // Treat ParentId as null if it's not provided or is 0, making it a top-level comment
            int? parentId = request.ParentId.HasValue && request.ParentId > 0 ? request.ParentId : null;

            if (parentId.HasValue)
            {
                var parentComment = await _unitOfWork.RepositoryPostComment.GetById(parentId.Value);
                if (parentComment == null)
                {
                    throw new InvalidOperationException("Parent comment does not exist.");
                }
            }

            var newComment = new PostComment
            {
                PostId = request.PostId,
                ParentId = parentId,
                UserId = request.UserId,
                Content = request.Content,
                CreatedDate = DateTime.UtcNow
            };

            _unitOfWork.RepositoryPostComment.Insert(newComment);
            await _unitOfWork.CommitAsync();

            return newComment;
        }

        public async Task<List<PostCommentRequest>> GetCommentsByPostIdAsync(int postId, int? parentId, Guid userId)
        {
            var comments = await _unitOfWork.RepositoryPostComment
                .GetAll()
                .Where(c => c.PostId == postId)
                .Include(c => c.InverseParent)
                .OrderBy(c => c.CreatedDate)
                .ToListAsync();

            // Filter comments based on parentId if provided
            var filteredComments = parentId.HasValue
                ? comments.Where(c => c.ParentId == parentId.Value).ToList()
                : comments.Where(c => c.ParentId == null).ToList(); // No parentId provided, get top-level comments

            var tasks = comments
                .Where(c => c.ParentId == null)
                .Select(parent => MapToRequestWithChildren(parent, comments, userId)); // Await the tasks

            var result = await Task.WhenAll(tasks); // Await the collection of tasks

            return result.ToList();
        }

        private async Task<PostCommentRequest> MapToRequestWithChildren(PostComment parent, List<PostComment> comments, Guid userId)
        {
            var parentRequest = _mapper.Map<PostCommentRequest>(parent);
            parentRequest.ChildComments = comments
                .Where(c => c.ParentId == parent.Id)
                .OrderBy(c => c.CreatedDate)
                .Select(child => MapToRequestWithChildren(child, comments, userId).Result) // Use .Result here or await the next call
                .ToList();

            // Fetch total likes and whether the user has liked this comment
            parentRequest.TotalLikes = await _postCommentLikeService.GetTotalLikesByCommentIdAsync(parent.Id);
            parentRequest.LikedByUser = (await _postCommentLikeService.GetUserLikesByCommentIdAsync(parent.Id)).Contains(userId);

            return parentRequest;
        }

        public async Task<PostCommentRequest?> UpdateCommentAsync(int id, UpdatePostCommentRequest request)
        {
            // Retrieve the existing comment entity
            var commentEntity = await _unitOfWork.RepositoryPostComment.GetById(id);
            if (commentEntity == null) return null;

            // Treat ParentId as null if it's not provided or is 0, making it a top-level comment
            //int? parentId = request.ParentId.HasValue && request.ParentId > 0 ? request.ParentId : null;

            //if (parentId.HasValue)
            //{
            //    var parentComment = await _unitOfWork.RepositoryPostComment.GetById(parentId.Value);
            //    if (parentComment == null)
            //    {
            //        throw new InvalidOperationException("Parent comment does not exist.");
            //    }
            //}

            // Update properties of the existing comment
            commentEntity.Content = request.Content;
            //commentEntity.ParentId = parentId; // Update the ParentId if provided

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
    }
}
