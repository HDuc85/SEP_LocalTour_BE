using LocalTour.Domain.Entities;
using LocalTour.Services.ViewModel;
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

        [HttpGet("getAll")]
        [Authorize]
        public async Task<IActionResult> GetAll(int? pageNumber, int? pageSize, string languageCode)
        {
            var placesSearch = await _placeSearchHistoryService.GetAllPlaceSearchHistory(User.GetUserId(),pageNumber, pageSize, languageCode);
            if (!placesSearch.Success)
            {
                return BadRequest(placesSearch.Message);
            }
            var result = placesSearch.Data.Select(x => new PlaceSearchHistoryVM()
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
                return NotFound("Empty Search History");
            }
            
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post(int placeId)
        {
            var result = await _placeSearchHistoryService.AddPlaceSearchHistory(User.GetUserId(), placeId);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> Delete(int placeId)
        {
            var result = await _placeSearchHistoryService.DeletePlaceSearchHistory(User.GetUserId(), placeId);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }

        [HttpDelete("deleteAll")]
        [Authorize]
        public async Task<IActionResult> DeleteAll()
        {
            var result = await _placeSearchHistoryService.DeleteAllPlaceSearchHistory(User.GetUserId());
            if (result.Success)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }
}
}

