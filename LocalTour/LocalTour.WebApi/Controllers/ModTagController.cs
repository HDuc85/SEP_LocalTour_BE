using LocalTour.Services.Abstract;
using LocalTour.Services.ViewModel;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class ModTagController : ControllerBase
{
    private readonly IModTagService _modTagService;

    public ModTagController(IModTagService modTagService)
    {
        _modTagService = modTagService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var modTags = await _modTagService.GetAllAsync();
        if (modTags == null)
        {
            return NotFound();
        }
        
        return Ok(modTags);
    }

    [HttpGet("UserTags/{userId}")]
    public async Task<ActionResult<IEnumerable<ModTagRequest>>> GetTagsByUser(Guid userId)
    {
        var tags = await _modTagService.GetTagsByUserAsync(userId);
        if (tags == null)
        {
            return NotFound($"No tags found for user with ID {userId}.");
        }
        return Ok(tags);
    }

    [HttpGet("TagUsers/{tagId}")]
    public async Task<ActionResult<IEnumerable<ModTagRequest>>> GetUsersByTag(int tagId)
    {
        var users = await _modTagService.GetUsersByTagAsync(tagId);
        if (users == null)
        {
            return NotFound($"No users found for tag with ID {tagId}.");
        }
        return Ok(users);
    }

    [HttpPost]
    public async Task<IActionResult> CreateMultiple([FromForm] ModTagRequest request)
    {
        var modTags = await _modTagService.CreateMultipleAsync(request);
        return Ok(modTags);
    }

    [HttpPut("{userId}")]
    public async Task<IActionResult> Update(Guid userId, [FromForm] UpdateTagRequest request)
    {
        var result = await _modTagService.UpdateUserTagsAsync(userId, request.TagIds);
        if (!result)
            return NotFound($"No record found for UserId {userId}.");

        return Ok("User tags updated successfully.");
    }


    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteMultiple(Guid userId, [FromForm] List<int> tagIds)
    {
        var result = await _modTagService.DeleteMultipleAsync(userId, tagIds);
        if (!result)
            return NotFound();

        return NoContent();
    }
}
