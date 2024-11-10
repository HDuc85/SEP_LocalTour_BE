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

    [HttpGet("{userId}/{tagId}")]
    public async Task<IActionResult> GetById(Guid userId, int tagId)
    {
        var modTag = await _modTagService.GetByIdAsync(userId, tagId);
        if (modTag == null)
            return NotFound();

        return Ok(modTag);
    }

    // GET: api/ModTag/UserTags/{userId}
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

    // GET: api/ModTag/TagUsers/{tagId}
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
    public async Task<IActionResult> Create(ModTagRequest request)
    {
        var modTag = await _modTagService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { userId = modTag.UserId, tagId = modTag.TagId }, modTag);
    }

    [HttpPut("{userId}/{tagId}")]
    public async Task<IActionResult> Update(Guid userId, int tagId, ModTagRequest request)
    {
        var updatedModTag = await _modTagService.UpdateAsync(userId, tagId, request);
        if (updatedModTag == null)
            return NotFound();

        return Ok(updatedModTag);
    }

    [HttpDelete("{userId}/{tagId}")]
    public async Task<IActionResult> Delete(Guid userId, int tagId)
    {
        var result = await _modTagService.DeleteAsync(userId, tagId);
        if (!result)
            return NotFound();

        return NoContent();
    }
}
