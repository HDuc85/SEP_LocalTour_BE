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

            // Kiểm tra kết quả trả về từ service
            if (result == 1) // Nếu result = 1, tức là hành động "like" đã được thực hiện thành công
            {
                return StatusCode(200, new { statusCode = 200, message = "Liked the schedule successfully." });
            }
            else if (result == 2) // Nếu result = 2, tức là hành động "unlike" đã được thực hiện thành công
            {
                return StatusCode(200, new { statusCode = 200, message = "Unliked the schedule successfully." });
            }
            else
            {
                // Trả về lỗi nếu có vấn đề khi toggle
                return StatusCode(400, new { statusCode = 400, message = "An error occurred while toggling like status." });
            }
        }


        [HttpPost("unlike")]
        public async Task<IActionResult> UnlikeSchedule(int scheduleId, Guid userId)
        {
            var result = await _scheduleLikeService.UnlikeScheduleAsync(scheduleId, userId);

            if (result)
            {
                return StatusCode(200, new { statusCode = 200, message = "Schedule unliked successfully." });
            }
            else
            {
                return StatusCode(400, new { statusCode = 400, message = "Schedule not liked." });
            }
        }

        [HttpGet("totalLikes/{scheduleId}")]
        public async Task<IActionResult> GetTotalLikes(int scheduleId)
        {
            var totalLikes = await _scheduleLikeService.GetTotalLikesAsync(scheduleId);

            return StatusCode(200, new { statusCode = 200, message = "Total likes retrieved successfully.", data = totalLikes });
        }

        [HttpGet("usersLiked/{scheduleId}")]
        public async Task<IActionResult> GetUsersLiked(int scheduleId)
        {
            var usersLiked = await _scheduleLikeService.GetUsersLikedAsync(scheduleId);

            return StatusCode(200, new { statusCode = 200, message = "Users who liked the schedule retrieved successfully.", data = usersLiked });
        }
    }
}
