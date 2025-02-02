﻿using Microsoft.AspNetCore.Mvc;
using LocalTour.Services.Abstract;
using LocalTour.Services.ViewModel;
using LocalTour.Services.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LocalTour.Domain.Entities;
using LocalTour.WebApi.Helper;
using Microsoft.AspNetCore.Authorization;

namespace LocalTour.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;

        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        [HttpGet("getAllSchedule")]
        [AllowAnonymous]
        public async Task<ActionResult<List<ScheduleRequest>>> GetAllSchedules([FromQuery] GetScheduleRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { statusCode = 400, message = "Validation error." });
                }

                var schedules = await _scheduleService.GetAllSchedulesAsync(request, User.GetUserId());

                if (schedules == null || schedules.TotalCount == 0)
                {
                    return NotFound(new { statusCode = 404, message = "No schedules found." });
                }

                return Ok(new { statusCode = 200, data = schedules });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { statusCode = 400, message = $"Invalid query parameter: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpGet("getById/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetScheduleById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { statusCode = 400, message = "Invalid schedule ID." });
                }

                var schedule = await _scheduleService.GetScheduleByIdAsync(id);
                if (schedule == null)
                {
                    return NotFound(new { statusCode = 404, message = "Schedule not found." });
                }

                return Ok(new { statusCode = 200, data = schedule });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpGet("getByUserId/{userId}")]
        public async Task<IActionResult> GetSchedulesByUserId(Guid userId,string languageCode)
        {
            try
            {
                if (userId == Guid.Empty)
                {
                    return BadRequest(new { statusCode = 400, message = "Invalid user ID." });
                }

                var schedules = await _scheduleService.GetSchedulesByUserIdAsync(userId, languageCode);

                if (schedules == null || !schedules.Any())
                {
                    return Ok(new { statusCode = 404, message = "No schedules found for the specified user." });
                }

                return Ok(new { statusCode = 200, data = schedules });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpPost("createSchedule")]
        [Authorize]
        public async Task<IActionResult> CreateSchedule([FromBody] CreateScheduleRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { statusCode = 400, message = "Schedule data is missing." });
                }

                var createdSchedule = await _scheduleService.CreateScheduleAsync(request, User.GetUserId());
                if (createdSchedule == null)
                {
                    return StatusCode(500, new { statusCode = 500, message = "An error occurred while creating the schedule." });
                }

                return Ok(new { statusCode = 201, message = "Schedule created successfully."});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpPut("updateSchedule")]
        [Authorize]
        public async Task<IActionResult> UpdateSchedule([FromBody] UpdateScheduleRequest request)
        {
            try
            {
                var result = await _scheduleService.UpdateScheduleAsync(request, User.GetUserId());
                if (!result)
                {
                    return NotFound(new { statusCode = 404, message = "Schedule not found or failed to update." });
                }

                return Ok(new { statusCode = 200, message = "Schedule updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = $"Error updating schedule: {ex.Message}" });
            }
        }

        [HttpDelete("deleteSchedule/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { statusCode = 400, message = "Invalid schedule ID." });
                }

                var result = await _scheduleService.DeleteScheduleAsync(id, User.GetUserId());
                if (!result)
                {
                    return NotFound(new { statusCode = 404, message = "Schedule not found or already deleted." });
                }

                return Ok(new { statusCode = 200, message = "Schedule deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpPost("cloneSchedule")]
        [Authorize]
        public async Task<IActionResult> CloneSchedule(int scheduleId)
        {
            try
            {
                if (scheduleId <= 0 )
                {
                    return BadRequest(new { statusCode = 400, message = "Invalid schedule ." });
                }

                var clonedSchedule = await _scheduleService.CloneScheduleFromOtherUserAsync(scheduleId, Guid.Parse(User.GetUserId()));

                if (clonedSchedule == null)
                {
                    return NotFound(new { statusCode = 404, message = "Schedule to clone not found." });
                }

                return CreatedAtAction(nameof(GetScheduleById), new { id = clonedSchedule.Id }, new { statusCode = 201, message = "Schedule cloned successfully.", data = clonedSchedule });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = $"Error cloning schedule: {ex.Message}" });
            }
        }

        [HttpPost("saveSuggestedSchedule")]
        [Authorize]
        public async Task<IActionResult> SaveSuggestedSchedule([FromBody] ScheduleWithDestinationsRequest request, Guid userId)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { statusCode = 400, message = "Suggested schedule data is missing." });
                }

                var savedSchedule = await _scheduleService.SaveSuggestedSchedule(request, userId);

                if (savedSchedule == null)
                {
                    return StatusCode(500, new { statusCode = 500, message = "An error occurred while saving the suggested schedule." });
                }

                return Ok(new { statusCode = 200, message = "Suggested schedule saved successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = $"Internal server error: {ex.Message}" });
            }
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
