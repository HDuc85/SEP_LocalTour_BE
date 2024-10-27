using LocalTour.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LocalTour.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostCommentLikeController : ControllerBase
    {
        private readonly IPostCommentLikeService _postCommentLikeService;

        public PostCommentLikeController(IPostCommentLikeService postCommentLikeService)
        {
            _postCommentLikeService = postCommentLikeService;
        }

        [HttpPost("{commentId}/like")]
        public async Task<IActionResult> LikeComment(int commentId, Guid userId)
        {
            var liked = await _postCommentLikeService.LikeCommentAsync(commentId, userId);
            if (!liked) return BadRequest("Comment already liked.");
            return NoContent();
        }

        [HttpPost("{commentId}/unlike")]
        public async Task<IActionResult> UnlikeComment(int commentId, Guid userId)
        {
            var unliked = await _postCommentLikeService.UnlikeCommentAsync(commentId, userId);
            if (!unliked) return BadRequest("Comment not liked yet.");
            return NoContent();
        }

        [HttpGet("likes/{commentId}")]
        public async Task<ActionResult<List<Guid>>> GetUserLikes(int commentId)
        {
            var userIds = await _postCommentLikeService.GetUserLikesByCommentIdAsync(commentId);
            return Ok(userIds);
        }

        [HttpGet("total-likes/{postCommentId}")]
        public async Task<IActionResult> GetTotalLikes(int postCommentId)
        {
            var totalLikes = await _postCommentLikeService.GetTotalLikesByCommentIdAsync(postCommentId);
            return Ok(totalLikes); // Trả về tổng số lượt like
        }
    }
}