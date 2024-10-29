using LocalTour.Services.Abstract;
using LocalTour.WebApi.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocalTour.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TraveledPlaceController : ControllerBase
    {
        private readonly ITraveledPlaceService _traveledPlaceService;

        public TraveledPlaceController(ITraveledPlaceService traveledPlaceService)
        {
            _traveledPlaceService = traveledPlaceService;
        }

        [HttpGet("getSingle")]
        [Authorize]
        public async Task<IActionResult> GetSingleTraveledPlace(int placeId)
        {
            string phoneNumber = User.GetPhoneNumber();
            var result = await _traveledPlaceService.CountTraveledPlace(phoneNumber, placeId);
            if (result < 0)
            {
                return BadRequest();
            }
            return Ok(result);
        }

        [HttpGet("getAll")]
        [Authorize]
        public async Task<IActionResult> GetAllTraveledPlaces()
        {
            string phoneNumber = User.GetPhoneNumber();
            var result = await _traveledPlaceService.GetAllTraveledPlaces(phoneNumber);
            if (result.Any())
            {
                return Ok(result);
            }
            return BadRequest();
        }

        [HttpGet("getWithPlaceIds")]
        [Authorize]
        public async Task<IActionResult> GetWithPlaceIds(List<int> placeIds)
        {
            string phoneNumber = User.GetPhoneNumber();
            var result = await _traveledPlaceService.CountTraveledPlaces(phoneNumber, placeIds);

            if (result.Any())
            {
                return Ok(result);
            }

            return BadRequest();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddTraveledPlace(int placeId)
        {
            string phoneNumber = User.GetPhoneNumber();
            var result = await _traveledPlaceService.AddTraveledPlace(phoneNumber, placeId);
            if (result)
            {
                return Ok("Success");
            }
            return BadRequest();
        }
    }
}