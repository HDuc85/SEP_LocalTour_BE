using LocalTour.Services.Abstract;
using LocalTour.Services.Model;
using LocalTour.Services.Services;
using LocalTour.Services.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocalTour.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet]
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
        public async Task<IActionResult> GetPost(int postId)
        {
            var userId = _postService.GetCurrentUserId();
            var postRequest = await _postService.GetPostById(postId, userId);

            if (postRequest == null)
            {
                return Ok(new { statusCode = 404, message = "Post not found." });
            }

            return Ok(new { statusCode = 200, data = postRequest });
        }

        [HttpPost]
        [HttpPost("createPost")]
        public async Task<IActionResult> CreatePost([FromForm] CreatePostRequest createPostRequest)
        {
            if (createPostRequest == null)
            {
                return BadRequest(new { statusCode = 400, message = "Post data is missing." });
            }

            if (string.IsNullOrEmpty(createPostRequest.Title) || string.IsNullOrEmpty(createPostRequest.Content))
            {
                return BadRequest(new { statusCode = 400, message = "Title and Content are required fields." });
            }

            try
            {
                await _postService.CreatePost(createPostRequest);
                return StatusCode(201, new { statusCode = 201, message = "Post created successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpPut("updatePost/{id}")]
        public async Task<IActionResult> UpdatePost(int id, [FromForm] CreatePostRequest createPostRequest)
        {
            if (createPostRequest == null)
            {
                return BadRequest(new { statusCode = 400, message = "Post data is missing." });
            }

            if (string.IsNullOrEmpty(createPostRequest.Title) || string.IsNullOrEmpty(createPostRequest.Content))
            {
                return BadRequest(new { statusCode = 400, message = "Title and Content are required fields." });
            }

            try
            {
                var result = await _postService.UpdatePost(id, createPostRequest);
                if (result == null)
                {
                    return BadRequest(new { statusCode = 400, message = "Failed to update the post. Invalid data or post not found." });
                }

                return Ok(new { statusCode = 200, message = "Post updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = $"Error updating post: {ex.Message}" });
            }
        }

        [HttpDelete("deletePost/{id}")]
        public async Task<ActionResult<ServiceResponseModel<bool>>> DeletePost(int id)
        {
            try
            {
                var deleted = await _postService.DeletePost(id);
                if (!deleted)
                {
                    return Ok(new { statusCode = 404, message = "Post not found or already deleted." });
                }

                return Ok(new { statusCode = 200, message = "Post deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = $"Internal server error: {ex.Message}" });
            }
        }
    }
}
