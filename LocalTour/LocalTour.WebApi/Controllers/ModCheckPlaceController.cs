using LocalTour.Services.Abstract;
using LocalTour.Services.ViewModel;
using LocalTour.WebApi.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace LocalTour.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class ModCheckPlaceController : ControllerBase
    {
        private readonly IModCheckService _modCheckService;
        public ModCheckPlaceController(IModCheckService modCheckService)
        {
            _modCheckService = modCheckService;
        }
        
        [HttpGet("GetAll")]
        //[Authorize(Roles = "Administrator")]
        public async Task<IActionResult> GetAllModChecks([FromQuery]GetAllModRequest queryParams)
        {
            try
            {
                var result = await _modCheckService.GetAllModChecks(queryParams);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = $"Internal server error: {ex.Message}" });
            }
        }
        
        [HttpPost("CreateModCheck")]
        [Authorize]
        public async Task<IActionResult> CreateModCheck([FromForm]CreateModCheckRequest bannerRequest)
        {
            try
            {
                var userId= User.GetUserId();
                
                var result = await _modCheckService.CreateModCheck(bannerRequest, userId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = $"Internal server error: {ex.Message}" });
            }
        }
        
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                var result = await _modCheckService.Delete(id);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = $"Internal server error: {ex.Message}" });
            }
        }
    }
}