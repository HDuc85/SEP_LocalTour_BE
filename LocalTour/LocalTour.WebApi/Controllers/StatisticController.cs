﻿using LocalTour.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;

namespace LocalTour.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticController : ControllerBase
    {
        private readonly IStatisticService _statistics;
        public StatisticController(IStatisticService statistics)
        {
            _statistics = statistics;
        }
        
        [HttpGet("GetUserRegistrationByMonthAsync")]
        //[Authorize(Roles = "Administrator")]
        public async Task<IActionResult> GetUserRegistrationByMonthAsync(int year)
        {
            try
            {
                var result = await _statistics.GetUserRegistrationByMonthAsync(year);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = $"Internal server error: {ex.Message}" });
            }
        }
        [HttpGet("GetTotalSuccessfulTravelsAsync")]
        //[Authorize(Roles = "Administrator")]
        public async Task<IActionResult> GetTotalSuccessfulTravelsAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var result = await _statistics.GetTotalSuccessfulTravelsAsync(startDate, endDate);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = $"Internal server error: {ex.Message}" });
            }
        }
        [HttpGet("GetTotalSchedulesCreatedAsync")]
        //[Authorize(Roles = "Administrator")]
        public async Task<IActionResult> GetTotalSchedulesCreatedAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var result = await _statistics.GetTotalSchedulesCreatedAsync(startDate, endDate);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = $"Internal server error: {ex.Message}" });
            }
        }
        [HttpGet("GetTotalPostsCreatedAsync")]
       // [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> GetTotalPostsCreatedAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var result = await _statistics.GetTotalPostsCreatedAsync(startDate, endDate);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = $"Internal server error: {ex.Message}" });
            }
        }
        
    }

}