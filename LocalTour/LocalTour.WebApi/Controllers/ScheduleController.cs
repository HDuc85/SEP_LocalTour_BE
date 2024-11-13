using Microsoft.AspNetCore.Mvc;
using LocalTour.Services.Abstract;
using LocalTour.Services.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LocalTour.WebApi.Helper;
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

        [HttpGet("{id}")]
        public async Task<ActionResult<ScheduleRequest>> GetScheduleById(int id)
        {
            var schedule = await _scheduleService.GetScheduleByIdAsync(id);
            if (schedule == null)
            {
                return NotFound();
            }
            return Ok(schedule);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<ScheduleRequest>>> GetSchedulesByUserId(Guid userId)
        {
            var schedules = await _scheduleService.GetSchedulesByUserIdAsync(userId);
            return Ok(schedules);
        }

        [HttpPost]
        public async Task<ActionResult<ScheduleRequest>> CreateSchedule(ScheduleRequest request)
        {
            var createdSchedule = await _scheduleService.CreateScheduleAsync(request);
            return CreatedAtAction(nameof(GetScheduleById), new { id = createdSchedule.Id }, createdSchedule);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSchedule(int id, ScheduleRequest request)
        {
            var result = await _scheduleService.UpdateScheduleAsync(id, request);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            var result = await _scheduleService.DeleteScheduleAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPost("{id}/clone")]
        [Authorize] // Ensure that only authenticated users can clone schedules
        public async Task<IActionResult> CloneSchedule(int id)
        {
            var userId = Guid.Parse(User.FindFirst("sub").Value); // Get the authenticated user's ID from the JWT token

            var clonedSchedule = await _scheduleService.CloneScheduleAsync(id, userId);
            if (clonedSchedule == null)
            {
                return NotFound(new { message = "Schedule not found." }); // Return 404 if the schedule does not exist
            }

            return CreatedAtAction(nameof(GetScheduleById), new { id = clonedSchedule.Id }, clonedSchedule); // Return the created schedule
        }

        [HttpPost("suggestSchedule")]
        [Authorize]
        public async Task<IActionResult> SuggestSchedule(SuggestScheduleRequest request )
        {
            var result = await _scheduleService.GenerateSchedule(request, User.GetUserId());

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }
    }
}
