using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LocalTour.Services.Abstract;
using LocalTour.Services.ViewModel;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace LocalTour.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostCommentController : ControllerBase
    {
        private readonly IPostCommentService _postCommentService;

        public PostCommentController(IPostCommentService postCommentService)
        {
            _postCommentService = postCommentService;
        }

        [Authorize]
        [HttpPost("createPostComment")]
        public async Task<IActionResult> CreateComment([FromBody] CreatePostCommentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier); // Extract user ID from HttpContext

                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var parsedUserId))
                {
                    return Unauthorized(new { error = "User not authenticated or invalid user ID." });
                }

                var createdComment = await _postCommentService.CreateCommentAsync(request, parsedUserId); // Pass user ID to service
                return CreatedAtAction(nameof(GetCommentsByPostId), new { postId = createdComment.PostId }, createdComment);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("getCommentsByPost/{postId}")]
        public async Task<IActionResult> GetCommentsByPostId(int postId, [FromQuery] int? parentId)
        {
            try
            {
                // Extract user ID from the HttpContext (authenticated user)
                var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var parsedUserId))
                {
                    return Unauthorized(new { error = "User not authenticated or invalid user ID." });
                }

                // Get the comments for the specified post ID
                var comments = await _postCommentService.GetCommentsByPostIdAsync(postId, parentId, parsedUserId);
                return Ok(comments); // Return the list of comments
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("updateComments/{id}")]
        public async Task<IActionResult> UpdateComment(int id, [FromForm] UpdatePostCommentRequest request)
        {
            var updatedComment = await _postCommentService.UpdateCommentAsync(id, request);
            if (updatedComment == null)
                return NotFound();

            return Ok(updatedComment);
        }

        [HttpDelete("deleteComments/{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var success = await _postCommentService.DeleteCommentAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}
