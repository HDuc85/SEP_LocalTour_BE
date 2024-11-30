using Microsoft.AspNetCore.Mvc;
using LocalTour.Services.Abstract;
using LocalTour.Services.ViewModel;
using LocalTour.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using LocalTour.Services.Services;
using Microsoft.AspNetCore.Authorization;

namespace LocalTour.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DestinationController : ControllerBase
    {
        private readonly IDestinationService _destinationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IScheduleService _scheduleService;

        public DestinationController(IDestinationService destinationService, IScheduleService scheduleService, IHttpContextAccessor httpContextAccessor)
        {
            _destinationService = destinationService;
            _scheduleService = scheduleService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("createDestination")]
        public async Task<ActionResult<Destination>> CreateDestination([FromBody] CreateDestinationRequest request)
        {
            try
            {
                var createdDestination = await _destinationService.CreateDestinationAsync(request);
                return Ok(new { StatusCode = 201, Message = "Destination created successfully", });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { StatusCode = 400, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = "An error occurred while creating the destination." });
            }
        }

        [HttpGet("getAll")]
        public async Task<ActionResult<List<DestinationRequest>>> GetAllDestinations(string? languageCode)
        {
            try
            {
                var destinations = await _destinationService.GetAllDestinations(languageCode);

                if (destinations == null || destinations.Count == 0)
                {
                    return NotFound(new { StatusCode = 404, Message = "No destinations found" });
                }

                return Ok(new { StatusCode = 200, Message = "Destinations fetched successfully", Data = destinations });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = "An error occurred while fetching the destinations." });
            }
        }

        [HttpGet("getByScheduleId/{scheduleId}")]
        public async Task<ActionResult<List<DestinationRequest>>> GetDestinationsByScheduleId(int scheduleId, string? languageCode)
        {
            try
            {
                var destinations = await _destinationService.GetAllDestinationsByScheduleId(scheduleId, languageCode);

                if (destinations == null || destinations.Count == 0)
                {
                    return NotFound(new { StatusCode = 404, Message = "No destinations found for the given schedule ID" });
                }

                return Ok(new { StatusCode = 200, Message = "Destinations fetched successfully", Data = destinations });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = "An error occurred while fetching destinations by schedule ID." });
            }
        }

        [HttpGet("getByDestinationId/{id}")]
        public async Task<ActionResult<DestinationRequest>> GetDestinationById(int id, [FromQuery] string? languageCode)
        {
            try
            {
                var destination = await _destinationService.GetDestinationById(id, languageCode);
                if (destination == null)
                {
                    return NotFound(new { StatusCode = 404, Message = "Destination not found" });
                }

                return Ok(new { StatusCode = 200, Message = "Destination fetched successfully", Data = destination });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = "An error occurred while fetching the destination." });
            }
        }

        [Authorize]
        [HttpPut("updateDestination/{id}")]
        public async Task<IActionResult> UpdateDestination(int id, [FromBody] CreateDestinationRequest request)
        {
            try
            {
                // Fetch the destination to be updated
                var destination = await _destinationService.GetDestinationById(id, null);
                if (destination == null)
                {
                    return NotFound(new { StatusCode = 404, Message = "Destination not found" });
                }

                // Fetch the current schedule of the destination
                var schedule = await _scheduleService.GetScheduleByIdAsync(destination.ScheduleId);
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Check if the logged-in user is the owner of the current schedule
                if (schedule.UserId.ToString() != userId)
                {
                    return Unauthorized(new { StatusCode = 401, Message = "You do not have permission to update this destination" });
                }

                // If the schedule is being changed, ensure the new schedule belongs to the same user
                if (request.ScheduleId != destination.ScheduleId)
                {
                    var newSchedule = await _scheduleService.GetScheduleByIdAsync(request.ScheduleId);
                    if (newSchedule == null)
                    {
                        return NotFound(new { StatusCode = 404, Message = "New schedule not found" });
                    }

                    // Check if the new schedule belongs to the same user
                    if (newSchedule.UserId.ToString() != userId)
                    {
                        return Unauthorized(new { StatusCode = 401, Message = "You do not have permission to assign this schedule to the destination" });
                    }
                }

                // Proceed to update the destination
                var result = await _destinationService.UpdateDestinationAsync(id, request);
                if (!result)
                {
                    return NotFound(new { StatusCode = 404, Message = "Destination update failed" });
                }

                // Return success response with status and message
                return Ok(new { StatusCode = 200, Message = "Destination updated successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred during update: {ex.Message}");
                Console.WriteLine($"Updating destination with ID: {id}, Schedule ID: {request.ScheduleId}");
                return StatusCode(500, new { StatusCode = 500, Message = "An error occurred while updating the destination." });
            }
        }

        [Authorize]
        [HttpDelete("deleteDestination/{id}")]
        public async Task<IActionResult> DeleteDestination(int id)
        {
            try
            {
                // Lấy UserId từ token (nếu có)
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Kiểm tra nếu không có token, trả về lỗi 401
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { StatusCode = 401, Message = "Authentication required" });
                }

                // Kiểm tra xem destination có tồn tại không
                var destination = await _destinationService.GetDestinationById(id, null);
                if (destination == null)
                {
                    return NotFound(new { StatusCode = 404, Message = "Destination not found" });
                }

                // Lấy Schedule từ Destination để kiểm tra quyền sở hữu
                var schedule = await _scheduleService.GetScheduleByIdAsync(destination.ScheduleId);

                // Kiểm tra nếu UserId không khớp với UserId trong Schedule
                if (schedule.UserId.ToString() != userId)
                {
                    return Unauthorized(new { StatusCode = 401, Message = "You do not have permission to delete this destination" });
                }

                // Thực hiện xoá Destination
                var result = await _destinationService.DeleteDestinationAsync(id);
                if (!result)
                {
                    return NotFound(new { StatusCode = 404, Message = "Destination not found" });
                }

                return Ok(new { StatusCode = 200, Message = "Destination deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = "An error occurred while deleting the destination." });
            }
        }
    }
}