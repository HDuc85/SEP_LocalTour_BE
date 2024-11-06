using LocalTour.Services.Abstract;
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
        public async Task<ActionResult<List<UserPreferenceTagsRequest>>> GetAll()
        {
            var tags = await _service.GetAllUserPreferenceTags();
            return Ok(tags);
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
        public async Task<ActionResult<UserPreferenceTagsRequest>> Create(UserPreferenceTagsRequest request)
        {
            var createdTag = await _service.CreateUserPreferenceTags(request);
            return CreatedAtAction(nameof(GetById), new { id = createdTag.Id }, createdTag);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserPreferenceTagsRequest>> Update(int id, UserPreferenceTagsRequest request)
        {
            var updatedTag = await _service.UpdateUserPreferenceTags(id, request);
            if (updatedTag == null)
                return NotFound();

            return Ok(updatedTag);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var deleted = await _service.DeleteUserPreferenceTags(id);
            if (!deleted)
                return NotFound();

            return Ok(deleted);
        }
    }
}
