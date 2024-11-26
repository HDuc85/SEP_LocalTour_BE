

using LocalTour.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocalTour.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
           _addressService = addressService;
        }
        
        [HttpGet("Province")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllProvinces()
        {
            try
            {
               var result = await _addressService.GetAllProvincesAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = $"Internal server error: {ex.Message}" });
            }
        }
        [HttpGet("District")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllDistrictsAsync(int provinceI)
        {
            try
            {
                var result = await _addressService.GetAllDistrictsAsync(provinceI);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = $"Internal server error: {ex.Message}" });
            }
        }
        [HttpGet("Ward")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllWardsAsync(int cityId)
        {
            try
            {
                var result = await _addressService.GetAllWardAsync(cityId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = $"Internal server error: {ex.Message}" });
            }
        }
    }
}