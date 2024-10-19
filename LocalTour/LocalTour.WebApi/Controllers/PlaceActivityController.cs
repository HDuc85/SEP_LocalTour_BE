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
        public async Task<ActionResult<PaginatedList<PlaceActivityRequest>>> GetAllActivityByPlaceid([FromQuery] int placeid, GetPlaceActivityRequest request)
        {
            var events = await _placeActivityService.GetAllActivityByPlaceid(placeid, request);
            return Ok(events);
        }
        [HttpPost("create")]
        public async Task<ActionResult<ServiceResponseModel<PlaceActivityRequest>>> CreatePlaceActivity([FromForm] int placeid, PlaceActivityRequest request)
        {
            if (request == null)
            {
                return BadRequest(new ServiceResponseModel<PlaceActivityRequest>(false, "Request cannot be null"));
            }
            try
            {
                var events = await _placeActivityService.CreatePlaceActivity(placeid, request);
                return Ok(new ServiceResponseModel<PlaceActivityRequest>(events)
                {
                    Message = "Place created successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ServiceResponseModel<PlaceActivityRequest>(false, $"An error occurred: {ex.Message}"));
            }
        }
    }
}
