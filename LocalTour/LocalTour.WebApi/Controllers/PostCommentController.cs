using LocalTour.Services.Abstract;
using LocalTour.Services.Services;
using LocalTour.Services.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocalTour.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostCommentController : ControllerBase
    {
        private readonly IPostCommentService _postCommentService;

        public PostCommentController(IPostCommentService postCommentService)
        {
            _postCommentService = postCommentService;
        }

        [HttpPost("create")]
        public async Task<ActionResult<PostCommentRequest>> CreateComment([FromBody] PostCommentRequest request)
        {
            var createdComment = await _postCommentService.CreateCommentAsync(request);
            return Ok(createdComment);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PostCommentRequest>> GetCommentById(int id, Guid userId)
        {
            var comment = await _postCommentService.GetCommentByIdAsync(id, userId);
            if (comment == null) return NotFound();
            return Ok(comment);
        }

        [HttpGet("by-post/{postId}")]
        public async Task<ActionResult<List<PostCommentRequest>>> GetCommentsByPostId(int postId, Guid userId)
        {
            var comments = await _postCommentService.GetCommentsByPostIdAsync(postId, userId);
            return Ok(comments);
        }

        [HttpPut("update/{id}")]
        public async Task<ActionResult<PostCommentRequest>> UpdateComment(int id, [FromBody] PostCommentRequest request)
        {
            var updatedComment = await _postCommentService.UpdateCommentAsync(id, request);
            if (updatedComment == null) return NotFound();
            return Ok(updatedComment);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var deleted = await _postCommentService.DeleteCommentAsync(id);
            return deleted ? NoContent() : NotFound();
        }

        [HttpGet("media/by-post/{postId}")]
        public async Task<ActionResult<List<PostMediumRequest>>> GetAllMediaByPostId(int postId, [FromQuery] PaginatedQueryParams queryParams)
        {
            var media = await _postCommentService.GetAllMediaByPostId(postId, queryParams);
            return Ok(media);
        }
    }
}
