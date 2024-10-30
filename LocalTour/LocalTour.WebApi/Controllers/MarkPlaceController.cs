using LocalTour.Services.ViewModel;
using LocalTour.Services.Abstract;
using LocalTour.WebApi.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocalTour.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarkPlaceController : ControllerBase
    {
        private readonly IMarkPlaceService _markPlaceService;

        public MarkPlaceController(IMarkPlaceService markPlaceService)
        {
            _markPlaceService = markPlaceService;
        }
        
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetMarkPlace(string languageCode)
        {
            string phoneNumber = User.GetPhoneNumber();
            var result = await _markPlaceService.GetMarkPlaces(phoneNumber, languageCode);
            if (result.Any())
            {
                return Ok(result);
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddMarkPlace(int placeId)
        {
            string phoneNumber = User.GetPhoneNumber();
            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                return BadRequest();
            }
            var result = await _markPlaceService.CreateMarkPlace(phoneNumber, placeId);
            if (result)
            {
                return Ok("Success");
            }
            return BadRequest();
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateMarkPlace(int placeId, bool isVisited)
        {
            string phoneNumber = User.GetPhoneNumber();
            var result = await _markPlaceService.UpdateMarkPlace(phoneNumber, placeId, isVisited);
            if (result)
            {
                return Ok("Success");
            }
            return BadRequest();
        }
        
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> RemoveMarkPlace(int placeId)
        {
            string phoneNumber = User.GetPhoneNumber();
            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                return BadRequest();    
            }
            var result = await _markPlaceService.DeleteMarkPlace(phoneNumber, placeId);

            if (result)
            {
                return Ok("Success");
            }
            return BadRequest();
        }

    }
}