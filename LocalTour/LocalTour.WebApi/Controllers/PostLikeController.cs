using LocalTour.Services.Abstract;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class PostLikeController : ControllerBase
{
    private readonly IPostLikeService _postLikeService;

    public PostLikeController(IPostLikeService postLikeService)
    {
        _postLikeService = postLikeService;
    }

    [HttpPost("like/{postId}")]
    public async Task<IActionResult> LikePost(int postId, Guid userId)
    {
        await _postLikeService.LikePostAsync(postId, userId);
        return Ok("Liked the post successfully.");
    }

    [HttpDelete("unlike/{postId}")]
    public async Task<IActionResult> UnlikePost(int postId, Guid userId)
    {
        await _postLikeService.UnlikePostAsync(postId, userId);
        return Ok("Unliked the post successfully.");
    }

    [HttpGet("users/{postId}")]
    public async Task<IActionResult> GetUsersWhoLikedPost(int postId)
    {
        var users = await _postLikeService.GetUserLikesByPostIdAsync(postId);
        return Ok(users);
    }

    [HttpGet("total-likes/{postId}")]
    public async Task<IActionResult> GetTotalLikes(int postId)
    {
        var totalLikes = await _postLikeService.GetTotalLikesByPostIdAsync(postId);
        return Ok(totalLikes);
    }
}
