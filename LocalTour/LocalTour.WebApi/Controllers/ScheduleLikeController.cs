using LocalTour.Services.Abstract;
using LocalTour.Services.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocalTour.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleLikeController : ControllerBase
    {
        private readonly IScheduleLikeService _scheduleLikeService;

        public ScheduleLikeController(IScheduleLikeService scheduleLikeService)
        {
            _scheduleLikeService = scheduleLikeService;
        }

        [HttpPost("like")]
        public async Task<IActionResult> LikeSchedule(int scheduleId, Guid userId)
        {
            var result = await _scheduleLikeService.LikeScheduleAsync(scheduleId, userId);
            return result ? Ok("Schedule liked successfully.") : BadRequest("Schedule already liked.");
        }

        [HttpPost("unlike")]
        public async Task<IActionResult> UnlikeSchedule(int scheduleId, Guid userId)
        {
            var result = await _scheduleLikeService.UnlikeScheduleAsync(scheduleId, userId);
            return result ? Ok("Schedule unliked successfully.") : BadRequest("Schedule not liked.");
        }

        [HttpGet("totalLikes/{scheduleId}")]
        public async Task<IActionResult> GetTotalLikes(int scheduleId)
        {
            var totalLikes = await _scheduleLikeService.GetTotalLikesAsync(scheduleId);
            return Ok(totalLikes);
        }

        [HttpGet("usersLiked/{scheduleId}")]
        public async Task<IActionResult> GetUsersLiked(int scheduleId)
        {
            var usersLiked = await _scheduleLikeService.GetUsersLikedAsync(scheduleId);
            return Ok(usersLiked);
        }
    }
}
