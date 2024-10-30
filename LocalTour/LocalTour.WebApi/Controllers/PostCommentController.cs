using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LocalTour.Services.Abstract;
using LocalTour.Services.ViewModel;

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

        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] CreatePostCommentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdComment = await _postCommentService.CreateCommentAsync(request);
                return CreatedAtAction(nameof(GetCommentsByPostId), new { postId = createdComment.PostId }, createdComment);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{postId}")]
        public async Task<IActionResult> GetCommentsByPostId(int postId, int parentId, [FromQuery] Guid userId)
        {
            var comments = await _postCommentService.GetCommentsByPostIdAsync(postId,parentId, userId);
            return Ok(comments);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(int id, [FromBody] UpdatePostCommentRequest request)
        {
            var updatedComment = await _postCommentService.UpdateCommentAsync(id, request);
            if (updatedComment == null)
                return NotFound();

            return Ok(updatedComment);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var success = await _postCommentService.DeleteCommentAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}
