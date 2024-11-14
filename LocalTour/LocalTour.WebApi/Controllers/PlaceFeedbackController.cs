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
                return Ok(feedback);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiReponseModel<PlaceFeedbackRequest>(false, $"An error occurred: {ex.Message}"));
            }
        }
        [HttpPost("create")]
        public async Task<ActionResult<ApiReponseModel<PlaceFeedbackRequest>>> CreateFeedback( int placeid, PlaceFeedbackRequest request)
        {
            if (request == null)
            {
                return BadRequest(new ApiReponseModel<PlaceFeedbackRequest>(false, "Request cannot be null"));
            }
            try
            {
                var feedback = await _placeFeedbackService.CreateFeedback(placeid, request);
                return Ok(feedback);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiReponseModel<PlaceFeedbackRequest>(false, $"An error occurred: {ex.Message}"));
            }
        }
        [HttpPut("update")]
        public async Task<ActionResult<ApiReponseModel<PlaceFeedbackRequest>>> UpdateEvent( int placeid, int feedbackid, PlaceFeedbackRequest request)
        {
            if (request == null)
            {
                return BadRequest(new ApiReponseModel<PlaceFeedbackRequest>(false, "Request cannot be null"));
            }
            try
            {
                var feedback = await _placeFeedbackService.UpdateFeedback(placeid, feedbackid, request);
                return Ok(feedback);
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
