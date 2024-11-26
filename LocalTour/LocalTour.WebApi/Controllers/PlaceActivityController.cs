using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;
using LocalTour.Services.Model;
using LocalTour.Services.Services;
using LocalTour.Services.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace LocalTour.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlaceActivityController : ControllerBase
    {
        private readonly IPlaceActivityService _placeActivityService;
        public PlaceActivityController(IPlaceActivityService placeActivityService)
        {
            _placeActivityService = placeActivityService;
        }
        [HttpGet("getall")]
        public async Task<ActionResult<PaginatedList<PlaceActivityRequest>>> GetAllActivityByPlaceid([FromQuery] int placeid,[FromQuery] GetPlaceActivityRequest request)
        {
            var events = await _placeActivityService.GetAllActivityByPlaceid(placeid, request);
            return Ok(events);
        }
        [HttpPost("create")]
        public async Task<ActionResult<ServiceResponseModel<PlaceActivityRequest>>> CreatePlaceActivity(int placeid, PlaceActivityRequest request)
        {
            if (request == null)
            {
                return BadRequest(new ServiceResponseModel<PlaceActivityRequest>(false, "Request cannot be null"));
            }
            try
            {
                var activity = await _placeActivityService.CreatePlaceActivity(placeid, request);
                return Ok(activity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ServiceResponseModel<PlaceActivityRequest>(false, $"An error occurred: {ex.Message}"));
            }
        }
        [HttpPut("update")]
        public async Task<ActionResult<ServiceResponseModel<PlaceActivityRequest>>> UpdateActivity( int placeid, int activityid, PlaceActivityRequest request)
        {
            if (request == null)
            {
                return BadRequest(new ServiceResponseModel<PlaceActivityRequest>(false, "Request cannot be null"));
            }
            try
            {
                var activity = await _placeActivityService.UpdateActivity(placeid, activityid, request);
                return Ok(activity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ServiceResponseModel<PlaceActivityRequest>(false, $"An error occurred: {ex.Message}"));
            }
        }
        [HttpGet("getactivitybyid")]
        public async Task<ActionResult<PlaceActivity>> GetActivityById( int placeid, int activityid, string languageCode)
        {
            var activityEntity = await _placeActivityService.GetActivityById(placeid, activityid, languageCode);
            return Ok(activityEntity);
        }
        [HttpDelete("delete")]
        public async Task<ActionResult<PlaceActivity>> DeletePlaceActivity(int placeid, int activityid)
        {
            if (placeid == null)
            {
                return BadRequest(new ServiceResponseModel<PlaceActivity>(false, "Request cannot be null"));
            }
            try
            {
                var activityEntity = await _placeActivityService.DeletePlaceActivity(placeid, activityid);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ServiceResponseModel<PlaceActivity>(false, $"An error occurred: {ex.Message}"));
            }
        }
    }
}
