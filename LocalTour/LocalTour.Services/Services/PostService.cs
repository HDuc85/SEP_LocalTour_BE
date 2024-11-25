using AutoMapper;
using LocalTour.Data;
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
using Microsoft.Extensions.DependencyInjection;
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
        private readonly IPostCommentService _postCommentService;
        private readonly IServiceProvider _serviceProvider;


        public PostService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration, 
                            IHttpContextAccessor httpContextAccessor, ILogger<PostService> logger, 
                            IFileService fileService, IUserService userService, IPostCommentService postCommentService, IServiceProvider serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _fileService = fileService;
            _userService = userService;
            _postCommentService = postCommentService;
            _serviceProvider = serviceProvider;
        }

        public async Task<List<PostRequest>> GetAllPosts(GetPostRequest request,String userId)
        {
            var postsQuery = _unitOfWork.RepositoryPost.GetAll()
                .Where(x => x.AuthorId == request.UserId)
                .Include(p => p.Author)
                .Include(p => p.PostLikes)
                .Include(p => p.Place)
                .ThenInclude(pp => pp.PlaceTranslations)
                .Include(z => z.Schedule)
                .Include(t => t.PostComments)
                .ToList();
            var listPostMedium = await _unitOfWork.RepositoryPostMedium.GetData();
            User currentUser = null;
            if (userId != null)
            {
                currentUser = await _userService.FindById(userId);
            }
            var author = await _userService.FindById(request.UserId.ToString());
            if (currentUser != null)
            {
                if (currentUser.Id != request.UserId)
                {
                    postsQuery = postsQuery.Where(x => x.Public == true).ToList();
                }
            }
            
            
            List<PostRequest> postRequests = new List<PostRequest>();
            foreach (var post in postsQuery)
            {
                postRequests.Add(new PostRequest()
                {
                    Id = post.Id,
                    Title = post.Title,
                    Content = post.Content,
                    AuthorId = post.AuthorId,
                    CreatedDate = post.CreatedDate,
                    ScheduleId = post.ScheduleId,
                    PlaceId = post.PlaceId,
                    Public = post.Public,   
                    UpdateDate = post.UpdateDate,
                    TotalLikes = post.PostLikes.Count,
                    TotalComments = post.PostComments.Count,
                    AuthorFullName = author.UserName,
                    Media = listPostMedium.Where(x => x.PostId == post.Id).Select(x =>
                    {
                            return new PostMedium
                            {
                                Id = x.Id,
                                Type = x.Type,
                                CreateDate = x.CreateDate,
                                Url = x.Url,
                                PostId = x.PostId,
                            };
                    }).ToList(),
                    Latitude = post.Latitude,
                    Longitude = post.Longitude,
                    isLiked = currentUser != null? post.PostLikes.Any(x => x.UserId == currentUser.Id): false,
                    AuthorProfilePictureUrl = author.ProfilePictureUrl,
                    PlaceName = post.PlaceId != null ? post.Place.PlaceTranslations.FirstOrDefault(x => x.LanguageCode == request.languageCode).Name : "" ,
                    ScheduleName = post.ScheduleId != null ? post.Schedule.ScheduleName : "",
                    PlacePhotoDisplay = post.PlaceId != null ?post.Place.PhotoDisplay : ""
                });
            }
            

            // Sorting by likes count or date
            if (request.SortBy == "liked")
            {
                postRequests = request.SortOrder == "asc"
                    ? postRequests.OrderBy(p => p.TotalLikes).ToList()
                    : postRequests.OrderByDescending(p => p.TotalLikes).ToList();
            }
            else if (request.SortBy == "created_by")
            {
                postRequests = request.SortOrder == "asc"
                    ? postRequests.OrderBy(p => p.CreatedDate).ToList()
                    : postRequests.OrderByDescending(p => p.CreatedDate).ToList();
            }


            if (request.SearchTerm != null)
            {
                postRequests = postRequests.Where(x => 
                    x.Title.ToLower().Contains(request.SearchTerm.ToLower())||
                    x.Content.ToLower().Contains(request.SearchTerm.ToLower()) || 
                    x.PlaceName.ToLower().Contains(request.SearchTerm.ToLower())||
                    x.ScheduleName.ToLower().Contains(request.SearchTerm.ToLower()))
                    .ToList();
            }


            /*
            var posts = await postsQuery
                .ListPaginateWithSortPostAsync<Post, PostRequest>(request.Page, request.Size, request.SortOrder, _mapper.ConfigurationProvider);

            foreach (var postRequest in posts.Items)
            {

                postRequest.Media = await GetAllMediaByPostId(postRequest.Id);

                // Fetch user details for the post's author
                var user = await _userService.FindById(postRequest.AuthorId.ToString());
                if (user != null)
                {
                    postRequest.AuthorFullName = user.FullName;
                    postRequest.AuthorProfilePictureUrl = user.ProfilePictureUrl;
                }

            }*/



            return postRequests;
        }
        

        public async Task<ServiceResponseModel<PostRequest>> GetPostById(int postId, String userId)
        {
            // Fetch the post by ID
            var post = await _unitOfWork.RepositoryPost.GetById(postId);
            if (post == null)
            {
                return new ServiceResponseModel<PostRequest>(false, "Post not found");
            }

            var postRequest = _mapper.Map<PostRequest>(post);

            var listMedia = await _unitOfWork.RepositoryPostMedium.GetData(x => x.PostId == postId);
            // Fetch media associated with the post
            postRequest.Media = listMedia.Select(x => new PostMedium
            {
                PostId = x.PostId,
                Type = x.Type,
                CreateDate = x.CreateDate,
                Url = x.Url,
            }).ToList();  // Sequential fetching
            
            // Fetch comments associated with the post
          //  var comments = await GetCommentsByPostIdAsync(postId, userId);  // Ensure this is awaited

            // Fetch user details associated with the post
            var user = await _userService.FindById(postRequest.AuthorId.ToString());
            if (userId != null)
            {
                var islikePost = await _unitOfWork.RepositoryPostLike.GetData(x => x.PostId == postId);
                postRequest.isLiked = islikePost.Any( x => x.UserId == Guid.Parse(userId));
            }
            // Map the user to UserFollowVM if the user exists
            UserFollowVM userFollowVM = null;
            if (user != null)
            {
                postRequest.AuthorFullName = user.FullName;
                postRequest.AuthorProfilePictureUrl = user.ProfilePictureUrl;
            }

            // Ensure the comments list is not null and assign user information
            /*
            if (comments != null)
            {
                // Assign parent-child relationships for comments
                postRequest.Comments = BuildCommentHierarchy(comments, userFollowVM); // Pass the mapped UserFollowVM
            }
            else
            {
                postRequest.Comments = new List<PostCommentRequest>(); // Ensure it's an empty list if no comments
            }
            */

            // Fetch total likes for the post
            postRequest.TotalLikes = await GetTotalLikesByPostIdAsync(postId);

            return new ServiceResponseModel<PostRequest>(postRequest)
            {
                Success = true,
                Message = "Post retrieved successfully"
            };
        }

        private List<PostCommentRequest> BuildCommentHierarchy(List<PostCommentRequest> comments, UserFollowVM userFollowVM)
        {
            // Create a dictionary to hold comments by their parent ID
            var commentDictionary = comments.ToLookup(c => c.ParentId);

            // Recursive function to build the comment tree
            List<PostCommentRequest> BuildComments(List<PostCommentRequest> parentComments)
            {
                foreach (var comment in parentComments)
                {
                    // Set user information for the comment (default values if the user info is null)
                    comment.UserFullName = userFollowVM?.UserName ?? "Unknown User";
                    comment.UserProfilePictureUrl = userFollowVM?.UserProfileUrl ?? "default-url"; // Replace with a default URL if user is null

                    // Recursively build child comments
                    comment.ChildComments = BuildComments(commentDictionary[comment.Id].ToList());
                }
                return parentComments;
            }

            // Start building the comment tree from the root comments (ParentId == null)
            return BuildComments(commentDictionary[null].ToList());
        }

        public async Task<PostRequest> CreatePost(CreatePostRequest createPostRequest, String parsedUserId)
        {
            // Lấy UserId từ HttpContext.User hoặc User.Identity.Name
            //var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var user = await _userService.FindById(parsedUserId);
            if (user == null)
            {
                throw new ApplicationException("User not found");
            }
            

            // Tạo entity bài viết
            var postEntity = new Post
            {
                Title = createPostRequest.Title,
                AuthorId = user.Id,
                Content = createPostRequest.Content,
                CreatedDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
                Public = createPostRequest.Public,
                ScheduleId = createPostRequest.ScheduleId,
                PlaceId = createPostRequest.PlaceId,
            };

            // Lưu bài viết vào cơ sở dữ liệu
            await _unitOfWork.RepositoryPost.Insert(postEntity);
            await _unitOfWork.CommitAsync();

            // Lưu các tệp phương tiện nếu có
            if (createPostRequest.MediaFiles != null && createPostRequest.MediaFiles.Any())
            {
                try
                {
                    var mediaSaveResult = await _fileService.SaveStaticFiles(createPostRequest.MediaFiles);

                    if (!mediaSaveResult.Success)
                    {
                        throw new Exception(mediaSaveResult.Message);
                    }

                    // Lưu các tệp hình ảnh
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

                    // Lưu các tệp video
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

            // Map từ Post entity sang PostRequest
            var postRequest = _mapper.Map<PostRequest>(postEntity);

            return postRequest;
        }

        public async Task<bool> UpdatePost(int postId, CreatePostRequest updatePostRequest, String parsedUserId)
        {
            // Lấy UserId từ HttpContext
            var user = await _userService.FindById(parsedUserId);

            var postEntity = await _unitOfWork.RepositoryPost.GetById(postId);
            if (postEntity == null) return true;

            // Kiểm tra xem người dùng có phải là tác giả của bài đăng không
            if (postEntity.AuthorId != user.Id)
            {
                throw new UnauthorizedAccessException("You do not have permission to update this post.");
            }
            
            postEntity.Title = updatePostRequest.Title;
            postEntity.Content = updatePostRequest.Content;
            postEntity.Public = updatePostRequest.Public;
            postEntity.UpdateDate = DateTime.UtcNow;
            postEntity.PlaceId = updatePostRequest.PlaceId;
            postEntity.ScheduleId = updatePostRequest.ScheduleId;

            // Xử lý các tệp phương tiện mới nếu có
            if (updatePostRequest.MediaFiles != null && updatePostRequest.MediaFiles.Any())
            {
                var existingMedia = await _unitOfWork.RepositoryPostMedium.GetAll()
                    .Where(m => m.PostId == postId)
                    .ToListAsync();

                // Xóa media cũ
                foreach (var media in existingMedia)
                {
                    _unitOfWork.RepositoryPostMedium.Delete(media);
                }

                // Lưu các tệp phương tiện mới
                var mediaSaveResult = await _fileService.SaveStaticFiles(updatePostRequest.MediaFiles);
                if (!mediaSaveResult.Success)
                {
                    throw new Exception(mediaSaveResult.Message);
                }

                // Lưu các tệp hình ảnh và video
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

            // Cập nhật bài viết
            _unitOfWork.RepositoryPost.Update(postEntity);
            await _unitOfWork.CommitAsync();

            var updatedPostRequest = _mapper.Map<PostRequest>(postEntity);
            updatedPostRequest.Media = await _postMediumService.GetAllMediaByPostId(postEntity.Id);

            return true;
        }

        public async Task<bool> DeletePost(int postId, Guid userGuid)
        {
            var postEntity = await _unitOfWork.RepositoryPost.GetById(postId);
            if (postEntity == null) return false;

            // Kiểm tra xem người dùng có phải là tác giả của bài đăng không
            if (postEntity.AuthorId != userGuid)
            {
                throw new UnauthorizedAccessException("You do not have permission to delete this post.");
            }

            // Xóa tất cả media liên quan đến bài viết
            var existingMedia = await _unitOfWork.RepositoryPostMedium.GetAll()
                .Where(m => m.PostId == postEntity.Id)
                .ToListAsync();

            foreach (var media in existingMedia)
            {
                _unitOfWork.RepositoryPostMedium.Delete(media);
            }

            // Xóa tất cả comment liên quan đến bài viết
            var existingComments = await _unitOfWork.RepositoryPostComment.GetAll()
                .Where(c => c.PostId == postEntity.Id)
                .ToListAsync();

            foreach (var comment in existingComments)
            {
                var childComments = await _unitOfWork.RepositoryPostComment.GetAll()
                    .Where(c => c.ParentId == comment.Id)
                    .ToListAsync();

                foreach (var childComment in childComments)
                {
                    _unitOfWork.RepositoryPostComment.Delete(childComment);
                }

                _unitOfWork.RepositoryPostComment.Delete(comment);
            }

            // Xóa bài viết
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

        public async Task<List<PostCommentRequest>> GetCommentsByPostIdAsync(int postId, String userId)
        {

            // Fetch all comments for the post, including their child comments (InverseParent)
            var comments = await _unitOfWork.RepositoryPostComment
                .GetAll()
                .Where(c => c.PostId == postId)
                .Include(c => c.InverseParent)
                .ThenInclude(x => x.User)
                .OrderBy(c => c.CreatedDate)
                .ToListAsync();

            // Filter for top-level comments (ParentId is null)
            var topLevelComments = comments.Where(c => c.ParentId == null).ToList();

            // Map each top-level comment to a PostCommentRequest with nested child comments
            var tasks = topLevelComments.Select(parent => MapToRequestWithChildren(parent, comments,userId));

            // Wait for all mapping tasks to complete
            var result = await Task.WhenAll(tasks);

            // Convert the array to a list and return
            return result.ToList();
        }

        private async Task<PostCommentRequest> MapToRequestWithChildren(PostComment parent, List<PostComment> comments, String userId)
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
                parentRequest.LikedByUser = userId != null ? await scopedContext.PostCommentLikes.AnyAsync(pc => pc.UserId == Guid.Parse(userId) && pc.PostCommentId == parent.Id) : false;
                parentRequest.UserFullName = parent.User.FullName;
                parentRequest.UserProfilePictureUrl = parent.User.ProfilePictureUrl;
                return parentRequest;
            }
        }

        public async Task<int> GetTotalLikesByPostIdAsync(int postId)
        {
            var likes = await _unitOfWork.RepositoryPostLike.GetData(l => l.PostId == postId);
            return likes.Count(); // Return total number of likes
        }

        public async Task<int> GetTotalLikesByCommentIdAsync(int postCommentId)
        {
            // Use _unitOfWork to access the repository responsible for PostCommentLikes
            var totalLikes = await _unitOfWork.RepositoryPostCommentLike
                .CountAsync(like => like.PostCommentId == postCommentId);

            return totalLikes;
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
