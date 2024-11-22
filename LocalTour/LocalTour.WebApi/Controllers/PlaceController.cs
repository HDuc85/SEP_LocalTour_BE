using Azure.Core;
using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;
using LocalTour.Services.Model;
using LocalTour.Services.Services;
using LocalTour.Services.ViewModel;
using LocalTour.Services.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocalTour.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class PlaceController : ControllerBase
    {
        private readonly IPlaceService _placeService;
        public PlaceController(IPlaceService placeService)
        {
            _placeService = placeService;
        }
        [HttpPost("create")]
        public async Task<ActionResult<ApiReponseModel<PlaceRequest>>> CreatePlace(PlaceRequest request)
        {
            if (request == null)
            {
                return BadRequest(new ApiReponseModel<PlaceRequest>(false, "Request cannot be null"));
            }
            try
            {
                var place = await _placeService.CreatePlace(request);
                return Ok(new ApiReponseModel<PlaceRequest>(place)
                {
                    message = "Place created successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiReponseModel<PlaceRequest>(false, $"An error occurred: {ex.Message}"));
            }
        }
        [HttpGet("getAll")]
        public async Task<ActionResult<PaginatedList<PlaceVM>>> GetAllPlaces([FromQuery] GetPlaceRequest request)
        {
            try
            {
                var places = await _placeService.GetAllPlace(request);
                return Ok(places);
            }
            catch (Exception ex)
            {
                return StatusCode(400, new ApiReponseModel<PlaceVM>(false, $"An error occurred: {ex.Message}"));
            }
        }
        [HttpGet("getPlaceById")]
        public async Task<ActionResult<ApiReponseModel<Place>>> GetPlaceById(string languageCode, int placeid)
        {
            try
            {
                var placeEntity = await _placeService.GetPlaceById(languageCode,placeid);
                return Ok(placeEntity);
            }
            catch (Exception ex)
            {
                return StatusCode(400, new ApiReponseModel<Place>(false, $"An error occurred: {ex.Message}"));
            }
        }
        [HttpPut("update")]
        public async Task<ActionResult<ApiReponseModel<PlaceRequest>>> UpdatePlace(int placeid,PlaceRequest request)
        {
            if (request == null)
            {
                return BadRequest(new ApiReponseModel<PlaceRequest>(false, "Request cannot be null"));
            }
            try
            {
                var place = await _placeService.UpdatePlace(placeid, request);
                return Ok(new ApiReponseModel<PlaceRequest>(place)
                {
                    message = "Place updated successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiReponseModel<PlaceRequest>(false, $"An error occurred: {ex.Message}"));
            }
        }
        [HttpPut("changeStatusPlace")]
        public async Task<ActionResult<Place>> ChangeStatusPlace([FromQuery] int placeid, [FromQuery] string status)
        {
            try
            {
                var placeEntity = await _placeService.ChangeStatusPlace(placeid, status);
                return Ok(placeEntity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiReponseModel<Place>(false, $"An error occurred: {ex.Message}"));
            }
        }
        [HttpDelete("delete")]
        public async Task<ActionResult<Place>> DeletePlace(int placeid)
        {
            if (placeid == null)
            {
                return BadRequest(new ApiReponseModel<Place>(false, "Request cannot be null"));
            }
            try
            {
                var placeEntity = await _placeService.DeletePlace(placeid);
                return Ok(new ApiReponseModel<bool>(true)
                {
                    message = "Successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiReponseModel<Place>(false, $"An error occurred: {ex.Message}"));
            }
        }
        [HttpGet("getAllByRole")]
        public async Task<ActionResult<PaginatedList<PlaceVM>>> GetAllPlacesByRole([FromQuery] GetPlaceRequest request)
        {
            try
            {
                var places = await _placeService.GetAllPlaceByRole(request);
                return Ok(places);
            }
            catch (Exception ex)
            {
                return StatusCode(400, new ApiReponseModel<PlaceVM>(false, $"An error occurred: {ex.Message}"));
            }
        }
    }
}
