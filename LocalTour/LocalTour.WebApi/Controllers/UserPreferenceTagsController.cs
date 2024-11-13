using LocalTour.Services.Abstract;
using LocalTour.Services.Services;
using LocalTour.Services.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocalTour.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserPreferenceTagsController : ControllerBase
    {
        private readonly IUserPreferenceTagsService _service;

        public UserPreferenceTagsController(IUserPreferenceTagsService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetUserPreferenceTagsRequest>>> GetAllUserPreferenceTagsGroupedByUserAsync()
        {
            var result = await _service.GetAllUserPreferenceTagsGroupedByUserAsync();

            if (result == null || !result.Any())
            {
                return NotFound();
            }

            return Ok(result);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<UserPreferenceTagsRequest>> GetById(int id)
        {
            var tag = await _service.GetUserPreferenceTagsById(id);
            if (tag == null)
                return NotFound();

            return Ok(tag);
        }

        [HttpPost]
        public async Task<ActionResult<UserPreferenceTagsRequest>> Create([FromForm] UserPreferenceTagsRequest request)
        {
            var createdTag = await _service.CreateUserPreferenceTags(request);
            return CreatedAtAction(nameof(GetById), new { id = createdTag.TagIds }, createdTag);
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> Update(Guid userId, [FromForm] UpdateTagRequest request)
        {
            // Call the service to update the user tags
            var result = await _service.UpdateUserTagsAsync(userId, request.TagIds);

            if (!result)
                return NotFound($"No record found for UserId {userId}.");

            return Ok("User tags updated successfully.");
        }


        [HttpDelete("{userId}")]
        public async Task<IActionResult> Delete(Guid userId, [FromForm] List<int> tagIds)
        {
            var deleted = await _service.DeleteUserPreferenceTags(userId, tagIds);
            if (!deleted)
                return NotFound();

            return Ok(deleted);
        }
    }
}
