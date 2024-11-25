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
using LocalTour.WebApi.Helper;

namespace LocalTour.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<PostController> _logger;

        public PostController(
            IPostService postService,
            IFileService fileService,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<PostController> logger)
        {
            _postService = postService;
            _fileService = fileService;
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

                var userId = User.GetUserId();
                var posts = await _postService.GetAllPosts(request,userId);

                if (posts == null || !posts.Any())
                {
                    return NotFound(new { message = "No posts found." });
                }

                return Ok(posts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpGet("getPost/{postId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPost(int postId)
        {
            try
            {
                var userId = User.GetUserId();
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

                var postRequest = await _postService.CreatePost(createPostRequest, User.GetUserId());

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
                var result = await _postService.UpdatePost(postId, updatePostRequest, User.GetUserId());

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
                // Thực hiện xóa bài viết
                var result = await _postService.DeletePost(postId, Guid.Parse(User.GetUserId()));
                if (!result)
                {
                    return BadRequest(new { StatusCode = 400, Message = "Failed to delete the post" });
                }


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
