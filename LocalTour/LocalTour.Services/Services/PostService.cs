using AutoMapper;
using LocalTour.Data.Abstract;
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
using System.Security.Claims;

namespace LocalTour.Services.Services
{
    public class PostService : IPostService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<PostService> _logger;
        private readonly IFileService _fileService;
        private readonly IPostMediumService _postMediumService;
        private readonly IUserService _userService;

        public PostService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration, 
                            IHttpContextAccessor httpContextAccessor, ILogger<PostService> logger, 
                            IFileService fileService, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _fileService = fileService;
            _userService = userService;
        }

        public async Task<PaginatedList<PostRequest>> GetAllPosts(GetPostRequest request)
        {
            var postsQuery = _unitOfWork.RepositoryPost.GetAll()
                .Include(p => p.PostMedia)  // Include the media related to the post
                .Include(p => p.Author)  // Include the Author (User) data
                .AsQueryable();

            if (request.UserId != null)
            {
                postsQuery = postsQuery.Where(p => p.AuthorId == request.UserId);
            }

            if (request.PostId != null)
            {
                postsQuery = postsQuery.Where(p => p.Id == request.PostId);
            }

            if (request.SortBy == "like")
            {
                postsQuery = request.SortOrder == "asc"
                    ? postsQuery.OrderBy(p => p.PostLikes.Count)
                    : postsQuery.OrderByDescending(p => p.PostLikes.Count);
            }
            else if (request.SortBy == "date")
            {
                postsQuery = request.SortOrder == "asc"
                    ? postsQuery.OrderBy(p => p.CreatedDate)
                    : postsQuery.OrderByDescending(p => p.CreatedDate);
            }

            var posts = await postsQuery
                .ListPaginateWithSortPostAsync<Post, PostRequest>(request.Page, request.Size, request.SortOrder, _mapper.ConfigurationProvider);

            foreach (var postRequest in posts.Items)
            {
                postRequest.TotalLikes = await GetTotalLikesByPostIdAsync(postRequest.Id);

                postRequest.Media = await GetAllMediaByPostId(postRequest.Id);

                var user = await _userService.GetUserByIdAsync(postRequest.AuthorId);

                if (user != null)
                {
                    postRequest.AuthorFullName = user.FullName;
                    postRequest.AuthorProfilePictureUrl = user.ProfilePictureUrl;
                }
            }

            return posts;
        }

        public async Task<PostRequest> GetPostById(int postId, Guid currentUserId)
        {
            var post = await _unitOfWork.RepositoryPost.GetById(postId);
            if (post == null)
            {
                return null;
            }

            var postRequest = _mapper.Map<PostRequest>(post);

            postRequest.Media = await GetAllMediaByPostId(postId);

            var comments = await GetCommentsByPostIdAsync(postId, null, currentUserId);

            var user = await _userService.GetUserByIdAsync(postRequest.AuthorId);

            if (user != null)
            {
                postRequest.AuthorFullName = user.FullName;
                postRequest.AuthorProfilePictureUrl = user.ProfilePictureUrl;
            }

            postRequest.Comments = comments.Select(comment =>
            {
                comment.UserFullName = user.FullName;
                comment.UserProfilePictureUrl = user.ProfilePictureUrl;
                return comment;
            }).ToList();

            postRequest.TotalLikes = await GetTotalLikesByPostIdAsync(postId);

            return postRequest;
        }

        public async Task<PostRequest> CreatePost(CreatePostRequest createPostRequest)
        {
            var postEntity = new Post
            {
                Title = createPostRequest.Title,
                Content = createPostRequest.Content,
                CreatedDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
                Public = createPostRequest.Public,
                AuthorId = createPostRequest.AuthorId,
                ScheduleId = createPostRequest.ScheduleId,
                PlaceId = createPostRequest.PlaceId,
            };

            await _unitOfWork.RepositoryPost.Insert(postEntity);
            await _unitOfWork.CommitAsync();

            if (createPostRequest.MediaFiles != null && createPostRequest.MediaFiles.Any())
            {
                try
                {
                    var mediaSaveResult = await _fileService.SaveStaticFiles(createPostRequest.MediaFiles, "PostMedia");

                    if (!mediaSaveResult.Success)
                    {
                        throw new Exception(mediaSaveResult.Message);
                    }

                    foreach (var fileUrl in mediaSaveResult.Data.imageUrls)
                    {
                        var postMedium = new PostMedium
                        {
                            PostId = postEntity.Id,
                            Type = "Image",
                            Url = fileUrl,
                            CreateDate = DateTime.UtcNow
                        };
                        await _unitOfWork.RepositoryPostMedium.Insert(postMedium);
                    }

                    foreach (var videoUrl in mediaSaveResult.Data.videoUrls)
                    {
                        var postMedium = new PostMedium
                        {
                            PostId = postEntity.Id,
                            Type = "Video",
                            Url = videoUrl,
                            CreateDate = DateTime.UtcNow
                        };
                        await _unitOfWork.RepositoryPostMedium.Insert(postMedium);
                    }

                    await _unitOfWork.CommitAsync();
                }
                catch (Exception mediaEx)
                {
                    _logger.LogError("Error while saving media files: " + mediaEx.Message);
                    throw new Exception("Error occurred while saving media files.", mediaEx);
                }
            }

            await _unitOfWork.CommitAsync();

            var postRequest = new PostRequest
            {
                Id = postEntity.Id,
                Title = postEntity.Title,
                Content = postEntity.Content,
                CreatedDate = postEntity.CreatedDate,
                UpdateDate = postEntity.UpdateDate,
                Public = postEntity.Public,
                AuthorId = postEntity.AuthorId,
                AuthorFullName = "", 
                Media = new List<PostMediumRequest>(),
                ScheduleId = postEntity.ScheduleId,
                PlaceId = postEntity.PlaceId
            };

            return postRequest;
        }

        public async Task<PostRequest?> UpdatePost(int postId, CreatePostRequest createPostRequest)
        {
            var postEntity = await _unitOfWork.RepositoryPost.GetById(postId);
            if (postEntity == null) return null;  

            postEntity.Title = createPostRequest.Title;
            postEntity.Content = createPostRequest.Content;
            postEntity.Public = createPostRequest.Public;
            postEntity.UpdateDate = DateTime.UtcNow;
            postEntity.PlaceId = createPostRequest.PlaceId;
            postEntity.ScheduleId= createPostRequest.ScheduleId;

            if (createPostRequest.MediaFiles != null && createPostRequest.MediaFiles.Any())
            {
                var existingMedia = await _unitOfWork.RepositoryPostMedium.GetAll()
                    .Where(m => m.PostId == postId)
                    .ToListAsync();

                foreach (var media in existingMedia)
                {
                    _unitOfWork.RepositoryPostMedium.Delete(media);
                }

                var mediaSaveResult = await _fileService.SaveStaticFiles(createPostRequest.MediaFiles, "PostMedia");
                if (!mediaSaveResult.Success)
                {
                    throw new Exception(mediaSaveResult.Message);
                }

                foreach (var fileUrl in mediaSaveResult.Data.imageUrls)
                {
                    var postMedium = new PostMedium
                    {
                        PostId = postEntity.Id,
                        Type = "Image",
                        Url = fileUrl,
                        CreateDate = DateTime.UtcNow
                    };
                    await _unitOfWork.RepositoryPostMedium.Insert(postMedium);
                }

                foreach (var videoUrl in mediaSaveResult.Data.videoUrls)
                {
                    var postMedium = new PostMedium
                    {
                        PostId = postEntity.Id,
                        Type = "Video",
                        Url = videoUrl,
                        CreateDate = DateTime.UtcNow
                    };
                    await _unitOfWork.RepositoryPostMedium.Insert(postMedium);
                }
            }

            _unitOfWork.RepositoryPost.Update(postEntity);
            await _unitOfWork.CommitAsync();

            var updatedPostRequest = _mapper.Map<PostRequest>(postEntity);
            updatedPostRequest.Media = await _postMediumService.GetAllMediaByPostId(postEntity.Id);

            return updatedPostRequest;
        }

        public async Task<bool> DeletePost(int postId)
        {
            var postEntity = await _unitOfWork.RepositoryPost.GetById(postId);
            if (postEntity == null) return false;

            _unitOfWork.RepositoryPost.Delete(postEntity);
            await _unitOfWork.CommitAsync();

            return true;
        }

        public Guid GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }

        public PostRequest MapPostToRequest(Post post)
        {
            if (post == null) throw new ArgumentNullException(nameof(post));
            return _mapper.Map<PostRequest>(post);
        }

        public async Task<List<PostCommentRequest>> GetCommentsByPostIdAsync(int postId, int? parentId, Guid userId)
        {
            var comments = await _unitOfWork.RepositoryPostComment
                .GetAll()
                .Where(c => c.PostId == postId)
                .Include(c => c.InverseParent)
                .OrderBy(c => c.CreatedDate)
                .ToListAsync();

            var filteredComments = parentId.HasValue
                ? comments.Where(c => c.ParentId == parentId.Value).ToList()
                : comments.Where(c => c.ParentId == null).ToList();

            var tasks = filteredComments
                .Where(c => c.ParentId == null)
                .Select(parent => MapToRequestWithChildren(parent, comments, userId));

            var result = await Task.WhenAll(tasks);
            return result.ToList();
        }

        private async Task<PostCommentRequest> MapToRequestWithChildren(PostComment parentComment, List<PostComment> allComments, Guid userId)
        {
            var childComments = allComments.Where(c => c.ParentId == parentComment.Id).ToList();
            var childCommentRequests = new List<PostCommentRequest>();

            // Loop through each child comment
            foreach (var c in childComments)
            {
                var totalLikes = await GetTotalLikesByCommentIdAsync(c.Id);
                var likedByUser = c.PostCommentLikes.Any(l => l.UserId == userId);

                // Fetch the user's full name and profile picture for each child comment
                var user = await _userService.GetUserByIdAsync(c.UserId);

                childCommentRequests.Add(new PostCommentRequest
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    // Assigning user's full name and profile picture
                    UserFullName = user?.FullName ?? "Anonymous", // Default to "Anonymous" if no user found
                    UserProfilePictureUrl = user?.ProfilePictureUrl ?? "default-profile-picture-url", // Default URL if no avatar is set
                    Content = c.Content,
                    CreatedDate = c.CreatedDate,
                    TotalLikes = totalLikes,
                    LikedByUser = likedByUser
                });
            }

            return new PostCommentRequest
            {
                Id = parentComment.Id,
                Content = parentComment.Content,
                CreatedDate = parentComment.CreatedDate,
                UserId = parentComment.UserId,
                TotalLikes = await GetTotalLikesByCommentIdAsync(parentComment.Id),
                LikedByUser = parentComment.PostCommentLikes.Any(l => l.UserId == userId),
                ChildComments = childCommentRequests
            };
        }

        public async Task<int> GetTotalLikesByPostIdAsync(int postId)
        {
            var likes = await _unitOfWork.RepositoryPostLike.GetData(l => l.PostId == postId);
            return likes.Count(); // Return total number of likes
        }

        public async Task<int> GetTotalLikesByCommentIdAsync(int postCommentId)
        {
            var likes = await _unitOfWork.RepositoryPostCommentLike.GetData(like => like.PostCommentId == postCommentId);
            return likes.Count(); // Return total number of likes for a comment
        }

        public async Task<List<PostMediumRequest>> GetAllMediaByPostId(int postId)
        {
            var mediaEntities = await _unitOfWork.RepositoryPostMedium.GetAll()
                .Where(m => m.PostId == postId)
                .ToListAsync();
            return _mapper.Map<List<PostMediumRequest>>(mediaEntities);
        }

    }
}
