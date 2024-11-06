using LocalTour.Services.Abstract;
using LocalTour.WebApi.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace LocalTour.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("getAll")]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            string userId = User.GetUserId();
            var result = await _notificationService.GetAll(userId);
            if (!result.IsNullOrEmpty())
            {
                return Ok(result);
            }
            return BadRequest();
        }

        [HttpPut("IsReaded")]
        [Authorize]
        public async Task<IActionResult> IsReaded(int notificationId)
        {
            string userId = User.GetUserId();
            var result = await _notificationService.ReadedNotification(userId, notificationId);
            if (result)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpDelete]
        [Authorize(Roles = "Administrator,Moderator")]
        public async Task<IActionResult> Delete(int notificationId)
        {
            string userId = User.GetUserId();
            var result = await _notificationService.DeleteNotification(notificationId, userId);
            if (result)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost("notificationEvent")]
        [Authorize(Roles = "Administrator,Moderator")]
        public async Task<IActionResult> NotificationEvent([FromForm] int eventId, string title, string body, DateTime timeSend)
        {
            string userId = User.GetUserId();
            var result = await _notificationService.SetNotificationForEvent(userId, eventId, title, body, timeSend);
            if (!result.IsNullOrEmpty())
            {
                return Ok(result);
            }
            return BadRequest();
        }
        [HttpPost("notificationSystem")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> NotificationSystem([FromForm] string title, string body, DateTime timeSend)
        {
            string userId = User.GetUserId();
            var result = await _notificationService.SetNotificationForSystem(userId,title, body, timeSend);
            if (!result.IsNullOrEmpty())
            {
                return Ok(result);
            }
            return BadRequest();
        }

        [HttpPost("SendNotificationNow")]
        [Authorize(Roles = "Administrator,Moderator")]
        public async Task<IActionResult> SendNotificationNow([FromForm] string deviceToken, string title, string body,
            string notificationType)
        {
            string userId = User.GetUserId();
            var result = await _notificationService.SendNotificationNow(userId, deviceToken, title, body, notificationType);
            if (!result.IsNullOrEmpty())
            {
                return Ok(result);
            }
            return BadRequest();
        }
    }
}

