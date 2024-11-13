using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;
using LocalTour.Services.Model;
using LocalTour.Services.Services;
using LocalTour.Services.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace LocalTour.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlaceFeedbackController : ControllerBase
    {
        private readonly IPlaceFeedbackService _placeFeedbackService;
        public PlaceFeedbackController(IPlaceFeedbackService placeFeedbackService)
        {
            _placeFeedbackService = placeFeedbackService;
        }
        [HttpGet("getAllFeedback")]
        public async Task<ActionResult<PaginatedList<PlaceFeedbackRequest>>> GetAllFeedbacksByPlaceid([FromQuery] int placeid, [FromQuery] GetPlaceFeedbackRequest request)
        {
            if (request == null)
            {
                return BadRequest(new ApiReponseModel<PlaceFeedbackRequest>(false, "Request cannot be null"));
            }
            try
            {
                var feedback = await _placeFeedbackService.GetAllFeedbackByPlace(placeid, request);
                return Ok(new ApiReponseModel<PaginatedList<PlaceFeedbackRequest>>(feedback)
                {
                    message = "Successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiReponseModel<PlaceFeedbackRequest>(false, $"An error occurred: {ex.Message}"));
            }
        }
        [HttpPost("create")]
        public async Task<ActionResult<ApiReponseModel<PlaceFeedbackRequest>>> CreateFeedback([FromForm] int placeid, [FromForm] PlaceFeedbackRequest request)
        {
            if (request == null)
            {
                return BadRequest(new ApiReponseModel<PlaceFeedbackRequest>(false, "Request cannot be null"));
            }
            try
            {
                var feedback = await _placeFeedbackService.CreateFeedback(placeid, request);
                return Ok(new ApiReponseModel<PlaceFeedbackRequest>(feedback)
                {
                    message = "Feedback created successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiReponseModel<PlaceFeedbackRequest>(false, $"An error occurred: {ex.Message}"));
            }
        }
        [HttpPut("update")]
        public async Task<ActionResult<ApiReponseModel<PlaceFeedbackRequest>>> UpdateEvent([FromForm] int placeid, [FromForm] int feedbackid, [FromForm] PlaceFeedbackRequest request)
        {
            if (request == null)
            {
                return BadRequest(new ApiReponseModel<PlaceFeedbackRequest>(false, "Request cannot be null"));
            }
            try
            {
                var feedback = await _placeFeedbackService.UpdateFeedback(placeid, feedbackid, request);
                return Ok(new ApiReponseModel<PlaceFeedbackRequest>(feedback)
                {
                    message = "Feedback updated successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiReponseModel<PlaceFeedbackRequest>(false, $"An error occurred: {ex.Message}"));
            }
        }
        [HttpDelete("delete")]
        public async Task<ActionResult<PlaceFeeedback>> DeletePlaceFeedback(int placeid, int feedbackid)
        {
            if (placeid == null && feedbackid == null)
            {
                return BadRequest(new ApiReponseModel<PlaceFeeedback>(false, "Request cannot be null"));
            }
            try
            {
                var feedback = await _placeFeedbackService.DeleteFeedback(placeid, feedbackid);
                return Ok(new ApiReponseModel<bool>(true)
                {
                    message = "Successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiReponseModel<PlaceFeeedback>(false, $"An error occurred: {ex.Message}"));
            }
        }
    }
}
