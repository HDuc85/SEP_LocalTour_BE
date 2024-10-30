using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;
using LocalTour.Services.Model;
using LocalTour.Services.Services;
using LocalTour.Services.ViewModel;
using LocalTour.Services.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace LocalTour.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlaceController : ControllerBase
    {
        private readonly IPlaceService _placeService;
        public PlaceController(IPlaceService placeService)
        {
            _placeService = placeService;
        }
        [HttpPost("create")]
        public async Task<ActionResult<ServiceResponseModel<PlaceRequest>>> CreatePlace([FromForm] PlaceRequest request)
        {
            if (request == null)
            {
                return BadRequest(new ServiceResponseModel<PlaceRequest>(false, "Request cannot be null"));
            }
            try
            {
                var place = await _placeService.CreatePlace(request);
                return Ok(new ServiceResponseModel<PlaceRequest>(place)
                {
                    Message = "Place created successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ServiceResponseModel<PlaceRequest>(false, $"An error occurred: {ex.Message}"));
            }
        }
        [HttpGet("all")]
        public async Task<ActionResult<PaginatedList<PlaceRequest>>> GetAllPlaces([FromQuery] GetPlaceRequest request)
        {
            var places = await _placeService.GetAllPlace(request);
            return Ok(places);
        }
        [HttpGet("getplacebyid")]
        public async Task<ActionResult<Place>> GetPlaceById([FromQuery] int placeid)
        {
            var placeEntity = await _placeService.GetPlaceById(placeid);
            return Ok(placeEntity);
        }
        [HttpPut("update")]
        public async Task<ActionResult<ServiceResponseModel<PlaceRequest>>> UpdatePlace([FromForm] int placeid, [FromForm] PlaceRequest request)
        {
            if (request == null)
            {
                return BadRequest(new ServiceResponseModel<PlaceRequest>(false, "Request cannot be null"));
            }
            try
            {
                var place = await _placeService.UpdatePlace(placeid, request);
                return Ok(new ServiceResponseModel<PlaceRequest>(place)
                {
                    Message = "Place updated successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ServiceResponseModel<PlaceRequest>(false, $"An error occurred: {ex.Message}"));
            }
        }

    }
}
