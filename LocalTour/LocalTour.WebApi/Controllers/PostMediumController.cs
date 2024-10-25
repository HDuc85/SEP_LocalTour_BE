using LocalTour.Services.Abstract;
using LocalTour.Services.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocalTour.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostMediumController : ControllerBase
    {
        private readonly IPostMediumService _postMediumService;

        public PostMediumController(IPostMediumService postMediumService)
        {
            _postMediumService = postMediumService;
        }

        [HttpGet("{postId}")]
        public async Task<ActionResult<List<PostMediumRequest>>> GetAllMediaByPostId(int postId)
        {
            var media = await _postMediumService.GetAllMediaByPostId(postId);
            return Ok(media);
        }

        [HttpGet("media/{mediaId}")]
        public async Task<ActionResult<PostMediumRequest>> GetMediaById(int mediaId)
        {
            var media = await _postMediumService.GetMediaById(mediaId);
            if (media == null) return NotFound();
            return Ok(media);
        }

        [HttpPost]
        public async Task<ActionResult<PostMediumRequest>> CreateMedia(PostMediumRequest request)
        {

            var createdMedia = await _postMediumService.CreateMedia(request);
            return CreatedAtAction(nameof(GetMediaById), new { mediaId = createdMedia.Id }, createdMedia);
        }

        [HttpPut("{mediaId}")]
        public async Task<ActionResult<PostMediumRequest>> UpdateMedia(int mediaId, PostMediumRequest request)
        {
            var updatedMedia = await _postMediumService.UpdateMedia(mediaId, request);
            if (updatedMedia == null) return NotFound();
            return Ok(updatedMedia);
        }

        [HttpDelete("{mediaId}")]
        public async Task<ActionResult> DeleteMedia(int mediaId)
        {
            var result = await _postMediumService.DeleteMedia(mediaId);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
