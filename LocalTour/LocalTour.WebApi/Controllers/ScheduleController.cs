using Microsoft.AspNetCore.Mvc;
using LocalTour.Services.Abstract;
using LocalTour.Services.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace LocalTour.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;

        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        [HttpGet("getAllSchedule")]
        public async Task<ActionResult<List<ScheduleRequest>>> GetAllSchedules([FromQuery] GetScheduleRequest request)
        {
            try
            {
                var schedules = await _scheduleService.GetAllSchedulesAsync(request);

                if (schedules == null || schedules.TotalCount == 0)
                {
                    return NotFound("No schedules found.");
                }

                return Ok(schedules);
            }
            catch (Exception ex)
            {
                // Log the exception (if any logging mechanism is available)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("getById/{id}")]
        public async Task<ActionResult<ScheduleRequest>> GetScheduleById(int id)
        {
            var schedule = await _scheduleService.GetScheduleByIdAsync(id);
            if (schedule == null)
                return NotFound();

            return Ok(schedule);
        }

        [HttpGet("getByUserId/{userId}")]
        public async Task<ActionResult<List<ScheduleRequest>>> GetSchedulesByUserId(Guid userId)
        {
            var schedules = await _scheduleService.GetSchedulesByUserIdAsync(userId);
            return Ok(schedules);
        }

        [HttpPost("createSchedule")]
        public async Task<ActionResult<ScheduleRequest>> CreateSchedule([FromForm] CreateScheduleRequest request)
        {
            var createdSchedule = await _scheduleService.CreateScheduleAsync(request);
            return CreatedAtAction(nameof(GetScheduleById), new { id = createdSchedule.Id }, createdSchedule);
        }

        [HttpPut("updateSchedule/{id}")]
        public async Task<IActionResult> UpdateSchedule(int id, [FromForm] CreateScheduleRequest request)
        {
            var result = await _scheduleService.UpdateScheduleAsync(id, request);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("deleteSchedule/{id}")]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            var result = await _scheduleService.DeleteScheduleAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPost("cloneSchedule")]
        public async Task<IActionResult> CloneSchedule(int scheduleId, [FromQuery] Guid userId)
        {
            var clonedSchedule = await _scheduleService.CloneScheduleFromOtherUserAsync(scheduleId, userId);

            if (clonedSchedule == null)
            {
                return NotFound("Schedule không tồn tại");
            }

            return CreatedAtAction(nameof(GetScheduleById), new { id = clonedSchedule.Id }, clonedSchedule);

        }
    }
}
