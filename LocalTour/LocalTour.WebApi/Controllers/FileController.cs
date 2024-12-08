using LocalTour.Services.Abstract;
using LocalTour.Services.Model;
using LocalTour.Services.Services;
using LocalTour.Services.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace LocalTour.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;
        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }
        [HttpPost("createlink")]
        public async Task<ActionResult<ServiceResponseModel<MediaFileStaticVM>>> SaveStaticFiles(List<IFormFile> files)
        {
            if (files == null)
            {
                return BadRequest(new ServiceResponseModel<MediaFileStaticVM>(false, "Request cannot be null"));
            }
            try
            {
                var fileEntity = await _fileService.SaveStaticFiles(files);
                var result = new
                {
                    ImageUrls = fileEntity.Data?.imageUrls,
                    VideoUrls = fileEntity.Data?.videoUrls
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ServiceResponseModel<MediaFileStaticVM>(false, $"An error occurred: {ex.Message}"));
            }
        }
        [HttpPost("link")]
        public async Task<ActionResult<MediaFileStaticVM>> SaveImageFiles(IFormFile file)
        {
            if (file == null)
            {
                return BadRequest(new ServiceResponseModel<MediaFileStaticVM>(false, "Request cannot be null"));
            }
            try
            {
                var fileEntity = await _fileService.SaveImageFile(file);
                return Ok(fileEntity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ServiceResponseModel<MediaFileStaticVM>(false, $"An error occurred: {ex.Message}"));
            }
        }
    }
}
