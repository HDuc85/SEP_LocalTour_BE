using LocalTour.Services.ViewModel;
using LocalTour.Services.Abstract;
using LocalTour.WebApi.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocalTour.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowUserController : ControllerBase
    {
        private readonly IFollowUserService _followUserService;
        private readonly IUserService _userService;

        public FollowUserController(IFollowUserService followUserService, IUserService userService)
        {
            _followUserService = followUserService;
            _userService = userService;
        }

        [HttpGet("followed")]
        public async Task<IActionResult> GetUserFollowed(Guid userId)
        {
            var user = await _userService.FindById(userId.ToString());
            if (user == null)
            {
                return BadRequest("User not found");
            }
            var usersFollow = await _followUserService.GetListUserFollowed(userId);
            if (usersFollow.Any())
            {
                var result = new List<UserFollowVM>();
                foreach (var item in usersFollow)
                {
                    result.Add(new UserFollowVM()
                    {
                        UserId = item.Id,
                        UserName = item.UserName,
                        UserProfileUrl = item.ProfilePictureUrl
                    });
                }
                return Ok(result);
            }
            return NotFound("User does not have any following");
        }

        [HttpGet("follow")]
        public async Task<IActionResult> GetUserFollow(Guid userId)
        {
            var user = await _userService.FindById(userId.ToString());
            if (user == null)
            {
                return BadRequest("User not found");
            }
            var usersFollow = await _followUserService.GetListUserFollow(userId);
            if (usersFollow.Any())
            {
                var result = new List<UserFollowVM>();
                foreach (var item in usersFollow)
                {
                    result.Add(new UserFollowVM()
                    {
                        UserId = item.Id,
                        UserName = item.UserName,
                        UserProfileUrl = item.ProfilePictureUrl
                    });
                }
                return Ok(result);
            }
            return NotFound("User does not follow anyone");
        }

        
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddUserFollow(Guid userFollowedId)
        {
            var result = await _followUserService.AddFollowUser(userFollowedId, User.GetUserId());

            if (result.Success)
            {
                return Ok(result.Message);
            }
            
            return BadRequest(result.Message);
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> RemoveUserFollow(Guid userFollowedId)
        {
            var result = await _followUserService.RemoveFollowUser(userFollowedId, User.GetUserId());

            if (result.Success)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }
        
    }
}