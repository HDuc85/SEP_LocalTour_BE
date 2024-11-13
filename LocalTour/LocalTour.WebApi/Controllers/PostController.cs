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
                var posts = await _postService.GetAllPosts(request);
                return Ok(posts);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("getPost/{postId}")]
        public async Task<IActionResult> GetPost(int postId)
        {
            var userId = _postService.GetCurrentUserId();
            var postRequest = await _postService.GetPostById(postId, userId);

            if (postRequest == null)
            {
                return NotFound();
            }

            return Ok(postRequest);
        }

        [HttpPost]
        [HttpPost("createPost")]
        public async Task<IActionResult> CreatePost([FromForm] CreatePostRequest createPostRequest)
        {
            if (createPostRequest == null)
            {
                return BadRequest("Invalid post data.");
            }
            try
            {
                await _postService.CreatePost(createPostRequest);
                return Ok("Create success");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        //[Authorize]
        [HttpPut("updatePost/{id}")]
        public async Task<IActionResult> UpdatePost(int id, [FromForm] CreatePostRequest createPostRequest)
        {
            try
            {
                var result = await _postService.UpdatePost(id, createPostRequest);
                if (result == null)
                {
                    return BadRequest("Invalid post data.");
                }
                return Ok("Update success");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error updating post: {ex.Message}" });
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
                    return NotFound(new ServiceResponseModel<bool>(false));
                }

                return Ok(new ServiceResponseModel<bool>(true)
                {
                    Message = "Post deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ServiceResponseModel<bool>(false));
            }
        }
    }
}
