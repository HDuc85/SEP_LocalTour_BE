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
           string userId = User.GetUserId();
            
            var result = await _markPlaceService.GetMarkPlaces(userId, languageCode);
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
           string userId = User.GetUserId();
            if (!string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest();
            }
            var result = await _markPlaceService.CreateMarkPlace(userId, placeId);
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
           string userId = User.GetUserId();
            var result = await _markPlaceService.UpdateMarkPlace(userId, placeId, isVisited);
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
           string userId = User.GetUserId();
            if (!string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest();    
            }
            var result = await _markPlaceService.DeleteMarkPlace(userId, placeId);

            if (result)
            {
                return Ok("Success");
            }
            return BadRequest();
        }

    }
}