using LocalTour.Services.Abstract;
using LocalTour.Services.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LocalTour.WebApi.Helper;
using Microsoft.AspNetCore.Authorization;

namespace LocalTour.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostCommentLikeController : ControllerBase
    {
        private readonly IPostCommentLikeService _postCommentLikeService;
        private readonly IUserService _userService;

        public PostCommentLikeController(IPostCommentLikeService postCommentLikeService, IUserService userService)
        {
            _postCommentLikeService = postCommentLikeService;
            _userService = userService;
        }

        [HttpPost("{commentId}/like")]
        [Authorize]
        public async Task<IActionResult> LikeOrUnlikeComment(int commentId)
        {

            try
            {
                // Kiểm tra và xử lý like/unlike
                var isLiked = await _postCommentLikeService.LikeCommentAsync(commentId, Guid.Parse(User.GetUserId()));

                if (isLiked)
                {
                    return StatusCode(200, new { statusCode = 200, message = "Comment has been liked." });
                }
                else
                {
                    return StatusCode(200, new { statusCode = 200, message = "Comment has been unliked." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpPost("{commentId}/unlike")]
        public async Task<IActionResult> UnlikeComment(int commentId, Guid userId)
        {
            var userExists = await _userService.FindById(userId.ToString());
            if (userExists == null)
            {
                return StatusCode(404, new { statusCode = 404, message = "User not found." });
            }

            try
            {
                var unliked = await _postCommentLikeService.UnlikeCommentAsync(commentId, userId);
                if (!unliked)
                {
                    return StatusCode(404, new { statusCode = 404, message = "Comment not liked yet." });
                }

                return StatusCode(200, new { statusCode = 200, message = "Comment has been unliked." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpGet("likes/{commentId}")]
        public async Task<ActionResult<List<Guid>>> GetUserLikes(int commentId)
        {
            try
            {
                var userIds = await _postCommentLikeService.GetUserLikesByCommentIdAsync(commentId);
                return Ok(new { statusCode = 200, data = userIds });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpGet("total-likes/{commentId}")]
        public async Task<IActionResult> GetTotalLikes(int commentId)
        {
            try
            {
                var totalLikes = await _postCommentLikeService.GetTotalLikesByCommentIdAsync(commentId);
                return Ok(new { statusCode = 200, message = "Total likes retrieved successfully.", data = totalLikes });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "An unexpected error occurred.", details = ex.Message });
            }
        }
    }
}
