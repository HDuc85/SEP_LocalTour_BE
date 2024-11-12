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
            var result = await _markPlaceService.GetMarkPlaces(User.GetUserId(), languageCode);
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddMarkPlace(int placeId)
        {
            var result = await _markPlaceService.CreateMarkPlace(User.GetUserId(), placeId);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateMarkPlace(int placeId, bool isVisited)
        {
            var result = await _markPlaceService.UpdateMarkPlace(User.GetUserId(), placeId, isVisited);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }
        
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> RemoveMarkPlace(int placeId)
        {
            var result = await _markPlaceService.DeleteMarkPlace(User.GetUserId(), placeId);

            if (result.Success)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }

    }
}