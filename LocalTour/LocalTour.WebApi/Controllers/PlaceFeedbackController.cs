using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;
using LocalTour.Services.Model;
using LocalTour.Services.Services;
using LocalTour.Services.ViewModel;
using Microsoft.AspNetCore.Authorization;
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
        public async Task<ActionResult<PaginatedList<PlaceFeedbackRequest>>> GetAllFeedbacksByPlaceid([FromQuery]GetPlaceFeedbackRequest request)
        {
            if (request == null)
            {
                return BadRequest(new ApiReponseModel<PlaceFeedbackRequest>(false, "Request cannot be null"));
            }
            try
            {
                var feedback = await _placeFeedbackService.GetAllFeedbackByPlace(request);
                return Ok(feedback);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiReponseModel<PlaceFeedbackRequest>(false, $"An error occurred: {ex.Message}"));
            }
        }
        [HttpPost("create")]
        [Authorize]
        public async Task<ActionResult<ApiReponseModel<FeedbackRequest>>> CreateFeedback([FromForm] FeedbackRequest request)
        {
            if (request == null)
            {
                return BadRequest(new ApiReponseModel<FeedbackRequest>(false, "Request cannot be null"));
            }
            try
            {
                var feedback = await _placeFeedbackService.CreateFeedback(request);
                return Ok(feedback);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("update")]
        [Authorize]
        public async Task<ActionResult<ApiReponseModel<FeedbackRequest>>> UpdateFeedback(int feedbackid, FeedbackRequest request)
        {
            if (request == null)
            {
                return BadRequest(new ApiReponseModel<FeedbackRequest>(false, "Request cannot be null"));
            }
            try
            {
                var feedback = await _placeFeedbackService.UpdateFeedback(request.placeid, feedbackid, request);
                return Ok(feedback);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiReponseModel<FeedbackRequest>(false, $"An error occurred: {ex.Message}"));
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
