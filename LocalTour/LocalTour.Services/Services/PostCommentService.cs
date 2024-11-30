using AutoMapper;
using LocalTour.Data;
using LocalTour.Data.Abstract;
using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;
using LocalTour.Services.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LocalTour.Services.Services
{
    public class PostCommentService : IPostCommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPostCommentLikeService _postCommentLikeService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IServiceProvider _serviceProvider;

        public PostCommentService(IUnitOfWork unitOfWork, IMapper mapper, IPostCommentLikeService postCommentLikeService, IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _postCommentLikeService = postCommentLikeService; // Initialize service
            _httpContextAccessor = httpContextAccessor;
            _serviceProvider = serviceProvider;
        }

        public async Task<PostComment> CreateCommentAsync(CreatePostCommentRequest request, Guid parsedUserId)
        {
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
                UserId = parsedUserId, 
                Content = request.Content,
                CreatedDate = DateTime.UtcNow
            };

            await _unitOfWork.RepositoryPostComment.Insert(newComment);
            await _unitOfWork.CommitAsync();

            return new PostComment();
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

            var tasks = filteredComments
                .Where(c => c.ParentId == null)
                .Select(parent => MapToRequestWithChildren(parent, comments, userId)); // Await the tasks

            var result = await Task.WhenAll(tasks); // This returns an array of PostCommentRequest

            return result.ToList(); // Convert the array to a list
        }

        private async Task<PostCommentRequest> MapToRequestWithChildren(PostComment parent, List<PostComment> comments, Guid userId)
        {
            // Use IServiceProvider to create a new scope and get a fresh DbContext instance
            using (var scope = _serviceProvider.CreateScope()) // Create a new scope
            {
                var scopedContext = scope.ServiceProvider.GetRequiredService<LocalTourDbContext>(); // Get a fresh DbContext

                var parentRequest = _mapper.Map<PostCommentRequest>(parent);

                var childComments = comments
                    .Where(c => c.ParentId == parent.Id)
                    .OrderBy(c => c.CreatedDate)
                    .ToList();

                var childRequests = new List<PostCommentRequest>();

                foreach (var child in childComments)
                {
                    var childRequest = await MapToRequestWithChildren(child, comments, userId); // Await each child sequentially
                    childRequests.Add(childRequest);
                }

                parentRequest.ChildComments = childRequests;

                // Fetch the like-related information sequentially
                parentRequest.TotalLikes = await scopedContext.PostCommentLikes.CountAsync(pc => pc.PostCommentId == parent.Id);
                parentRequest.LikedByUser = await scopedContext.PostCommentLikes.AnyAsync(pc => pc.UserId == userId && pc.PostCommentId == parent.Id);

                return parentRequest;
            }
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

            // Lấy danh sách tất cả các comment con của comment cha
            var allCommentsToDelete = await GetAllChildCommentsAsync(id);

            // Xóa tất cả các comment con
            foreach (var childComment in allCommentsToDelete)
            {
                _unitOfWork.RepositoryPostComment.Delete(childComment);
            }

            // Xóa comment cha
            _unitOfWork.RepositoryPostComment.Delete(commentEntity);
            _unitOfWork.RepositoryPostCommentLike.Delete(x => x.PostCommentId == id);

            // Lưu thay đổi vào database
            await _unitOfWork.CommitAsync();

            return true;
        }

        private async Task<List<PostComment>> GetAllChildCommentsAsync(int parentId)
        {
            // Lấy tất cả các comment con trực tiếp của comment cha
            var childComments = await _unitOfWork.RepositoryPostComment
                .GetAll()
                .Where(c => c.ParentId == parentId)
                .ToListAsync();

            // Tạo danh sách tổng hợp tất cả comment con
            var allChildComments = new List<PostComment>(childComments);

            // Đệ quy để lấy các comment con của từng comment con
            foreach (var child in childComments)
            {
                var subChildComments = await GetAllChildCommentsAsync(child.Id); // Đệ quy lấy các comment con
                allChildComments.AddRange(subChildComments); // Thêm các comment con vào danh sách
            }

            return allChildComments; // Trả về danh sách tổng hợp
        }
    }
}
