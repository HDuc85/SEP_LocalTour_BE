using AutoMapper;
using Azure;
using LocalTour.Data.Abstract;
using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;
using LocalTour.Services.Model;
using LocalTour.Services.Services;
using LocalTour.Services.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LocalTour.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly IFileService _fileService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<PostController> _logger;

        public PostController(
            IPostService postService,
            IFileService fileService,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<PostController> logger)
        {
            _postService = postService;
            _fileService = fileService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllPosts([FromQuery] GetPostRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { statusCode = 400, message = "Request data is missing." });
                }

                var posts = await _postService.GetAllPosts(request);

                if (posts == null || !posts.Items.Any())
                {
                    return Ok(new { statusCode = 404, message = "No posts found." });
                }

                return Ok(new { statusCode = 200, data = posts });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpGet("getPost/{postId}")]
        [Authorize]
        public async Task<IActionResult> GetPost(int postId)
        {
            try
            {
                var userId = _postService.GetCurrentUserId();
                var postRequest = await _postService.GetPostById(postId, userId);

                if (postRequest == null)
                {
                    return NotFound(new { statusCode = 404, message = "Post not found." });
                }

                var comments = await _postService.GetCommentsByPostIdAsync(postId, userId); 

                return Ok(new { statusCode = 200, data = postRequest, comments });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpPost("createPost")]
        [Authorize]
        public async Task<IActionResult> CreatePost([FromForm] CreatePostRequest createPostRequest)
        {
            if (createPostRequest == null)
            {
                return BadRequest(new { statusCode = 400, message = "Missing post data." });
            }

            if (string.IsNullOrEmpty(createPostRequest.Title) || string.IsNullOrEmpty(createPostRequest.Content))
            {
                return BadRequest(new { statusCode = 400, message = "Title and Content are required fields." });
            }

            try
            {
                var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { statusCode = 401, message = "User not authenticated." });
                }

                if (!Guid.TryParse(userId, out var parsedUserId))
                {
                    return Unauthorized(new { statusCode = 401, message = "Invalid user ID." });
                }

                var postRequest = await _postService.CreatePost(createPostRequest, parsedUserId);

                return StatusCode(201, new { statusCode = 201, message = "Post created successfully.", data = postRequest });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = $"Internal server error: {ex.Message}" });
            }
        }

        [Authorize]
        [HttpPut("updatePost/{postId}")]
        public async Task<IActionResult> UpdatePost(int postId, [FromForm] CreatePostRequest updatePostRequest)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var parsedUserId))
                {
                    return Unauthorized(new { statusCode = 401, message = "User not authenticated." });
                }

                var postEntity = await _unitOfWork.RepositoryPost.GetById(postId);
                if (postEntity == null)
                {
                    return NotFound(new { statusCode = 404, message = "Post not found." });
                }

                if (postEntity.AuthorId != parsedUserId)
                {
                    return Unauthorized(new { statusCode = 401, message = "You do not have permission to update this post." });
                }

                postEntity.Title = updatePostRequest.Title;
                postEntity.Content = updatePostRequest.Content;
                postEntity.Public = updatePostRequest.Public;
                postEntity.UpdateDate = DateTime.UtcNow;
                postEntity.PlaceId = updatePostRequest.PlaceId;
                postEntity.ScheduleId = updatePostRequest.ScheduleId;

                if (updatePostRequest.MediaFiles != null && updatePostRequest.MediaFiles.Any())
                {
                    var existingMedia = await _unitOfWork.RepositoryPostMedium.GetAll()
                        .Where(m => m.PostId == postId)
                        .ToListAsync();

                    foreach (var media in existingMedia)
                    {
                        _unitOfWork.RepositoryPostMedium.Delete(media);
                    }

                    var mediaSaveResult = await _fileService.SaveStaticFiles(updatePostRequest.MediaFiles, "PostMedia");
                    if (!mediaSaveResult.Success)
                    {
                        return BadRequest(new { statusCode = 400, message = mediaSaveResult.Message });
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

                return Ok(new { statusCode = 200, message = "Post updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = $"Internal server error: {ex.Message}" });
            }
        }

        [Authorize]
        [HttpDelete("deletePost/{postId}")]
        public async Task<IActionResult> DeletePost(int postId)
        {
            try
            {
                // Lấy UserId từ token (nếu có)
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Kiểm tra nếu không có token, trả về lỗi 401
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { StatusCode = 401, Message = "Authentication required" });
                }

                // Convert UserId từ string sang Guid
                var userGuid = Guid.Parse(userId);

                // Lấy Post theo ID và kiểm tra quyền của user
                var postResponse = await _postService.GetPostById(postId, userGuid);
                if (postResponse?.Data == null)
                {
                    return NotFound(new { StatusCode = 404, Message = "Post not found" });
                }

                var post = postResponse.Data;

                // Kiểm tra xem UserId có khớp với AuthorId của Post không
                if (post.AuthorId != userGuid)
                {
                    return Unauthorized(new { StatusCode = 401, Message = "You do not have permission to delete this post" });
                }

                // Thực hiện xóa tất cả media liên quan đến Post
                var existingMedia = await _unitOfWork.RepositoryPostMedium.GetAll()
                    .Where(m => m.PostId == post.Id)
                    .ToListAsync();

                foreach (var media in existingMedia)
                {
                    _unitOfWork.RepositoryPostMedium.Delete(media);
                }

                // Thực hiện xóa tất cả comment liên quan đến Post (bao gồm comment con)
                var existingComments = await _unitOfWork.RepositoryPostComment.GetAll()
                    .Where(c => c.PostId == post.Id)
                    .ToListAsync();

                foreach (var comment in existingComments)
                {
                    // Xóa tất cả comment con liên quan
                    var childComments = await _unitOfWork.RepositoryPostComment.GetAll()
                        .Where(c => c.ParentId == comment.Id)
                        .ToListAsync();

                    foreach (var childComment in childComments)
                    {
                        _unitOfWork.RepositoryPostComment.Delete(childComment);
                    }

                    _unitOfWork.RepositoryPostComment.Delete(comment);
                }

                // Thực hiện xóa bài viết
                var result = await _postService.DeletePost(postId, userGuid);
                if (!result)
                {
                    return BadRequest(new { StatusCode = 400, Message = "Failed to delete the post" });
                }

                // Lưu các thay đổi vào cơ sở dữ liệu
                await _unitOfWork.CommitAsync();

                return Ok(new { StatusCode = 200, Message = "Post deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the post.");

                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while deleting the post.",
                    Details = ex.Message
                });
            }
        }

    }
}
