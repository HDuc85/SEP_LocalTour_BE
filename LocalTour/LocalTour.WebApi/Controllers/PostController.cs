using LocalTour.Services.Abstract;
using LocalTour.Services.Model;
using LocalTour.Services.Services;
using LocalTour.Services.ViewModel;
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

        // Retrieve all posts, with pagination, filtering, and optional schedule/place associations
        [HttpGet("getall")]
        public async Task<ActionResult<PaginatedList<PostRequest>>> GetAllPosts([FromQuery] GetPostRequest request)
        {
            var posts = await _postService.GetAllPosts(request);
            return Ok(posts);
        }

        // Retrieve a specific post by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponseModel<PostRequest>>> GetPostById(int id)
        {
            var post = await _postService.GetPostById(id);
            if (post == null)
            {
                return NotFound(new ServiceResponseModel<PostRequest>(false, "Post not found"));
            }
            return Ok(new ServiceResponseModel<PostRequest>(post));
        }

        // Create a new post with optional PlaceId or ScheduleId associations
        [HttpPost("create")]
        public async Task<ActionResult<ServiceResponseModel<PostRequest>>> CreatePost([FromForm] PostRequest request)
        {
            if (request == null)
            {
                return BadRequest(new ServiceResponseModel<PostRequest>(false, "Request cannot be null"));
            }

            try
            {
                var createdPost = await _postService.CreatePost(request);
                return Ok(new ServiceResponseModel<PostRequest>(createdPost)
                {
                    Message = "Post created successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ServiceResponseModel<PostRequest>(false, $"An error occurred: {ex.Message}"));
            }
        }

        // Update an existing post by ID
        [HttpPut("update/{id}")]
        public async Task<ActionResult<ServiceResponseModel<PostRequest>>> UpdatePost(int id, [FromForm] PostRequest request)
        {
            try
            {
                var updatedPost = await _postService.UpdatePost(id, request);
                if (updatedPost == null)
                {
                    return NotFound(new ServiceResponseModel<PostRequest>(false, "Post not found"));
                }

                return Ok(new ServiceResponseModel<PostRequest>(updatedPost)
                {
                    Message = "Post updated successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ServiceResponseModel<PostRequest>(false, $"An error occurred: {ex.Message}"));
            }
        }

        // Delete a post by ID
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<ServiceResponseModel<bool>>> DeletePost(int id)
        {
            try
            {
                var deleted = await _postService.DeletePost(id);
                if (!deleted)
                {
                    return NotFound(new ServiceResponseModel<bool>(false, "Post not found"));
                }

                return Ok(new ServiceResponseModel<bool>(true)
                {
                    Message = "Post deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ServiceResponseModel<bool>(false, $"An error occurred: {ex.Message}"));
            }
        }
    }
}
