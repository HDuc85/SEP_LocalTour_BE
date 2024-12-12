using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;
using LocalTour.Services.Model;
using LocalTour.Services.ViewModel;
using LocalTour.WebApi.Helper;
using Microsoft.AspNetCore.Mvc;

namespace LocalTour.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagController : ControllerBase
    {
        private readonly ITagService _tagService;
        public TagController(ITagService tagService)
        {
            _tagService = tagService;
        }
        [HttpGet("getAll")]
        public async Task<ActionResult<PaginatedList<TagViewModel>>> GetAllTag([FromQuery] PaginatedQueryParams request)
        {
            var tags = await _tagService.GetAllTag(request);
            return Ok(tags);
        }

        [HttpGet("TagsTopPlace")]
        public async Task<IActionResult> TagsTopPlace()
        {
            var userId = User.GetUserId();

            var result = await _tagService.GetTagsTopPlace(userId);
            
            return Ok(result);
        }
        
        [HttpGet("getTagById")]
        public async Task<ActionResult<Tag>> GetTagById( int tagid)
        {
            var tagEntity = await _tagService.GetTagById(tagid);
            return Ok(tagEntity);
        }

        [HttpPost("create")]
        public async Task<ActionResult<ServiceResponseModel<TagRequest>>> CreateTag(TagRequest request)
        {
            if (request == null)
            {
                return BadRequest(new ServiceResponseModel<TagRequest>(false, "Request cannot be null"));
            }
            try
            {
                var tags = await _tagService.CreateTag(request);
                return Ok(tags);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ServiceResponseModel<TagRequest>(false, $"An error occurred: {ex.Message}"));
            }
        }
        [HttpPut("update")]
        public async Task<ActionResult<ServiceResponseModel<TagUpdateRequest>>> UpdateTag(int tagid,[FromForm] TagUpdateRequest request)
        {
            if (request == null)
            {
                return BadRequest(new ServiceResponseModel<TagRequest>(false, "Request cannot be null"));
            }
            try
            {
                var tags = await _tagService.UpdateTag(tagid, request);
                return Ok(tags);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ServiceResponseModel<TagRequest>(false, $"An error occurred: {ex.Message}"));
            }
        }
        [HttpDelete("delete")]
        public async Task<ActionResult<Tag>> DeleteTag(int tagid)
        {
            if (tagid == null)
            {
                return BadRequest(new ServiceResponseModel<Tag>(false, "Request cannot be null"));
            }
            try
            {
                var eventEntity = await _tagService.DeleteTag(tagid);
                return Ok(new ApiReponseModel<bool>(true)
                {
                    message = "Successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ServiceResponseModel<Tag>(false, $"An error occurred: {ex.Message}"));
            }
        }
    }
}
