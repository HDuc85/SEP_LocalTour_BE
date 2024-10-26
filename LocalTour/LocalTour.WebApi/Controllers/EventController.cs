using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;
using LocalTour.Services.Model;
using LocalTour.Services.Services;
using LocalTour.Services.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LocalTour.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }
        [HttpGet("getall")]
        public async Task<ActionResult<PaginatedList<EventRequest>>> GetAllEventsByPlaceid([FromQuery] int placeid, [FromQuery] GetEventRequest request)
        {
            var events = await _eventService.GetAllEventByPlaceid(placeid, request);
            return Ok(events);
        }
        [HttpGet("geteventbyid")]
        public async Task<ActionResult<Event>> GetEventById([FromQuery] int placeid, [FromQuery] int eventid)
        {
                var eventEntity = await _eventService.GetEventById(placeid, eventid);
                return Ok(eventEntity);
        }

        [HttpPost("create")]
        public async Task<ActionResult<ServiceResponseModel<EventRequest>>> CreateEvent([FromForm] int placeid, [FromForm] EventRequest request)
        {
            if (request == null)
            {
                return BadRequest(new ServiceResponseModel<EventRequest>(false, "Request cannot be null"));
            }
            try
            {
                var events = await _eventService.CreateEvent(placeid, request);
                return Ok(new ServiceResponseModel<EventRequest>(events)
                {
                    Message = "Place created successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ServiceResponseModel<EventRequest>(false, $"An error occurred: {ex.Message}"));
            }
        }
        [HttpPut("update")]
        public async Task<ActionResult<ServiceResponseModel<EventRequest>>> UpdateEvent([FromForm] int placeid, [FromForm] int eventid, [FromForm] EventRequest request)
        {
            if (request == null)
            {
                return BadRequest(new ServiceResponseModel<EventRequest>(false, "Request cannot be null"));
            }
            try
            {
                var events = await _eventService.UpdateEvent(placeid,eventid, request);
                return Ok(new ServiceResponseModel<EventRequest>(events)
                {
                    Message = "Place updated successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ServiceResponseModel<EventRequest>(false, $"An error occurred: {ex.Message}"));
            }
        }
    }
}
