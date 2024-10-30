using LocalTour.Domain.Entities;
using LocalTour.Domain.ViewModel;
using LocalTour.Services.Abstract;
using LocalTour.WebApi.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocalTour.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaceSearchHistoryController : ControllerBase
    {
        private readonly IPlaceSearchHistoryService _placeSearchHistoryService;

        public PlaceSearchHistoryController(IPlaceSearchHistoryService placeSearchHistoryService)
        {
            _placeSearchHistoryService = placeSearchHistoryService;
        }

        [HttpGet("GetAll")]
        [Authorize]
        public async Task<IActionResult> GetAll(int? pageNumber, int? pageSize, string languageCode)
        {
            var userId = User.GetUserId();
            
            var placesSearch = await _placeSearchHistoryService.GetAllPlaceSearchHistory(userId,pageNumber, pageSize, languageCode);
            if (!placesSearch.Any())
            {
                return BadRequest();
            }
            var result = placesSearch.Select(x => new PlaceSearchHistoryVM()
            {
                PlaceId = x.PlaceId,
                PlaceName = x.Place.PlaceTranslations.FirstOrDefault(y => y.LanguageCode == languageCode).Name,
                LastSearchDate = x.LastSearch,
                PlacePhotoDisplayUrl = x.Place.PhotoDisplay
            });
            if (result.Any())
            {
                return Ok(result);
            }
            else
            {
                return NoContent();
            }
            
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post(int placeId)
        {
            var userId = User.GetUserId();
            var result = await _placeSearchHistoryService.AddPlaceSearchHistory(userId, placeId);
            if (result)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Put(int placeId)
        {
            var userId = User.GetUserId();
            var result = await _placeSearchHistoryService.UpdatePlaceSearchHistory(userId, placeId);
            if (result)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> Delete(int placeId)
        {
            var userId = User.GetUserId();
            var result = await _placeSearchHistoryService.DeletePlaceSearchHistory(userId, placeId);
            if (result)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpDelete("DeleteAll")]
        [Authorize]
        public async Task<IActionResult> DeleteAll()
        {
            var userId = User.GetUserId();
            var result = await _placeSearchHistoryService.DeleteAllPlaceSearchHistory(userId);
            if (result)
            {
                return Ok();
            }
            return BadRequest();
        }
}
}

