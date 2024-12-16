using LocalTour.Services.Abstract;
using LocalTour.Services.Model;
using LocalTour.Services.Services;
using LocalTour.Services.ViewModel;
using LocalTour.WebApi.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocalTour.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlaceReportController : ControllerBase
    {
        private readonly IPlaceReportService _placeReportService;

        public PlaceReportController(IPlaceReportService placeReportService)
        {
            _placeReportService = placeReportService;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<PlaceReportRequest>> CreateReport([FromBody] PlaceReportRequest request)
        {
            try
            {
                if (request == null) return BadRequest();

                var createdReport = await _placeReportService.CreateReport(request, User.GetUserId());
                return Ok(createdReport);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlaceReportRequest>>> GetAllReports()
        {
            var reports = await _placeReportService.GetAllReports();
            return Ok(reports);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponseModel<PlaceReportRequest>>> GetPlaceReportById(int id)
        {
            var report = await _placeReportService.GetPlaceReportById(id);
            if (report == null)
            {
                return NotFound(new ServiceResponseModel<PlaceReportRequest>(false, "Place report not found"));
            }

            return Ok(new ServiceResponseModel<PlaceReportRequest>(report));
        }

        // Endpoint để lấy báo cáo theo tag
        [HttpGet("tag/{tagId}")]
        public async Task<ActionResult<IEnumerable<PlaceReportRequest>>> GetReportsByTag(int tagId)
        {
            var reports = await _placeReportService.GetReportsByTag(tagId);
            return Ok(reports);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PlaceReportRequest>> UpdateReport(int id, [FromBody] PlaceReportRequest request)
        {
            if (request == null) return BadRequest();

            var updatedReport = await _placeReportService.UpdateReport(id, request);
            if (updatedReport == null) return NotFound();

            return Ok(updatedReport);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteReport(int id)
        {
            var result = await _placeReportService.DeleteReport(id);
            if (!result) return NotFound();

            return NoContent();
        }
        [HttpGet("getAllByMod")]
        //[Authorize]
        public async Task<ActionResult<PaginatedList<PlaceReportVM>>> GetAllPlaceReportByMod([FromQuery] PlaceReportViewModel request)
        {
            try
            {
                var reports = await _placeReportService.GetAllPlaceReportByMod(request, User.GetUserId());
                return Ok(reports);
            }
            catch (Exception ex)
            {
                return StatusCode(400, new ApiReponseModel<PlaceReportVM>(false, $"An error occurred: {ex.Message}"));
            }
        }
    }
}