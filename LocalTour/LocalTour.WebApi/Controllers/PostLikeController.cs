using LocalTour.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using LocalTour.WebApi.Helper;
using Microsoft.AspNetCore.Authorization;

namespace LocalTour.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostLikeController : ControllerBase
    {
        private readonly IPostLikeService _postLikeService;
        private readonly IUserService _userService;
        private readonly IPostService _postService;

        public PostLikeController(
            IPostLikeService postLikeService,
            IUserService userService,
            IPostService postService)
        {
            _postLikeService = postLikeService;
            _userService = userService;
            _postService = postService;
        }

        [HttpPost("toggle/{postId}")]
        [Authorize]
        public async Task<IActionResult> ToggleLike(int postId)
        {
            try
            {
                var isLiked = await _postLikeService.ToggleLikePostAsync(postId, Guid.Parse(User.GetUserId()));
                return StatusCode(200, new { message = isLiked ? "Liked the post successfully." : "Unliked the post successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpDelete("unlike/{postId}")]
        public async Task<IActionResult> UnlikePost(int postId, [FromQuery] Guid userId)
        {
            // Kiểm tra User
            var userExists = await _userService.FindById(userId.ToString());
            if (userExists == null)
                return StatusCode(404, new { message = "User not found." });

            // Kiểm tra Post
            var postExists = await _postService.GetPostById(postId, User.GetUserId());
            if (postExists == null)
                return StatusCode(404, new { message = "Post not found." });

            try
            {
                var unliked = await _postLikeService.UnlikePostAsync(postId, userId);
                if (!unliked)
                    return StatusCode(400, new { message = "Post not liked yet." });

                return StatusCode(200, new { message = "Unliked the post successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpGet("users/{postId}")]
        public async Task<IActionResult> GetUsersWhoLikedPost(int postId)
        {
            try
            {
                var users = await _postLikeService.GetUserLikesByPostIdAsync(postId);
                return StatusCode(200, new { message = "User likes retrieved successfully.", data = users });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpGet("total-likes/{postId}")]
        public async Task<IActionResult> GetTotalLikes(int postId)
        {
            try
            {
                var totalLikes = await _postLikeService.GetTotalLikesByPostIdAsync(postId);
                return StatusCode(200, new { message = "Total likes retrieved successfully.", data = totalLikes });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }
    }
}
