using LocalTour.Services.Abstract;
using LocalTour.Services.ViewModel;
using LocalTour.WebApi.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace LocalTour.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class BannerController : ControllerBase
    {
        private readonly IBannerService _bannerService;
        public BannerController(IBannerService bannerService)
        {
            _bannerService = bannerService;
        }
        
        [HttpGet("GetBanner")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBanner()
        {
            try
            {
                var result = await _bannerService.GetPublicBannerActive();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = $"Internal server error: {ex.Message}" });
            }
        }
        [HttpGet("GetAll")]
        [Authorize]
        public async Task<IActionResult> GetAllBanner(GetAllBannerRequest request)
        {
            try
            {
                var result = await _bannerService.GetAllAsync(request);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = $"Internal server error: {ex.Message}" });
            }
        }
        
        [HttpGet("GetByAuthor")]
        [Authorize]
        public async Task<IActionResult> GetById(GetAllBannerRequest request)
        {
            try
            {
                request.UserId = User.GetUserId();
                
                var result = await _bannerService.GetAllAsync(request);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = $"Internal server error: {ex.Message}" });
            }
        }
        [HttpPost("createBanner")]
        [Authorize]
        public async Task<IActionResult> Create([FromForm]BannerRequest bannerRequest)
        {
            try
            {
                var userId= User.GetUserId();
                
                var result = await _bannerService.CreateAsync(bannerRequest, userId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = $"Internal server error: {ex.Message}" });
            }
        }
        
        [HttpPost("CreateHistory")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CreateHistory([FromBody]BannerHistoryRequest historyRequest)
        {
            try
            {
                var userId= User.GetUserId();
                
                var result = await _bannerService.CreateHistoryBanner(historyRequest, userId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = $"Internal server error: {ex.Message}" });
            }
        }
        
        [HttpPut("Update")]
        [Authorize]
        public async Task<IActionResult> Update(string bannerId,[FromForm]BannerRequest bannerRequest)
        {
            try
            {
                var userId= User.GetUserId();
                
                var result = await _bannerService.UpdateAsync(bannerRequest, userId,bannerId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = $"Internal server error: {ex.Message}" });
            }
        }
        
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            try
            {
                var userId= User.GetUserId();
                
                var result = await _bannerService.DeleteAsync(id, userId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = $"Internal server error: {ex.Message}" });
            }
        }
    }
}