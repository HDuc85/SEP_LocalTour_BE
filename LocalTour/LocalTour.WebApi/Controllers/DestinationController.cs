using Microsoft.AspNetCore.Mvc;
using LocalTour.Services.Abstract;
using LocalTour.Services.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocalTour.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DestinationController : ControllerBase
    {
        private readonly IDestinationService _destinationService;

        public DestinationController(IDestinationService destinationService)
        {
            _destinationService = destinationService;
        }

        [HttpPost]
        public async Task<ActionResult<DestinationRequest>> CreateDestination(DestinationRequest request)
        {
            var createdDestination = await _destinationService.CreateDestinationAsync(request);
            return CreatedAtAction(nameof(GetDestinationById), new { id = createdDestination.Id }, createdDestination);
        }

        [HttpGet("schedule/{scheduleId}")]
        public async Task<ActionResult<List<DestinationRequest>>> GetDestinationsByScheduleId(int scheduleId)
        {
            var destinations = await _destinationService.GetDestinationsByScheduleIdAsync(scheduleId);
            return Ok(destinations);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DestinationRequest>> GetDestinationById(int id)
        {
            var destination = await _destinationService.GetDestinationByIdAsync(id);
            if (destination == null)
            {
                return NotFound();
            }
            return Ok(destination);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDestination(int id, DestinationRequest request)
        {
            var result = await _destinationService.UpdateDestinationAsync(id, request);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDestination(int id)
        {
            var result = await _destinationService.DeleteDestinationAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
