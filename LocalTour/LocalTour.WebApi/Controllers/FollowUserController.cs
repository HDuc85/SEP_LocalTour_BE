using LocalTour.Domain.ViewModel;
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

        public FollowUserController(IFollowUserService followUserService)
        {
            _followUserService = followUserService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserFollow(Guid userId)
        {
            var usersFollow = await _followUserService.GetListUserFollow(userId);
            if (usersFollow.Any())
            {
                var result = new List<UserFollowVM>();
                foreach (var user in usersFollow)
                {
                    result.Add(new UserFollowVM()
                    {
                        UserId = user.Id,
                        UserName = user.UserName,
                        UserProfileUrl = user.ProfilePictureUrl
                    });
                }
                return Ok(result);
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddUserFollow(Guid userFollowedId)
        {
            string phoneNumber = User.GetPhoneNumber();
            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                return BadRequest();
            }
            var result = await _followUserService.AddFollowUser(userFollowedId, phoneNumber);
            if (result)
            {
                return Ok("Success");
            }
            return NotFound();
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> RemoveUserFollow(Guid userFollowedId)
        {
            string phoneNumber = User.GetPhoneNumber();
            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                return BadRequest();    
            }
            var result = await _followUserService.RemoveFollowUser(userFollowedId, phoneNumber);

            if (result)
            {
                return Ok("Success");
            }
            return NotFound();
        }
        
    }
}