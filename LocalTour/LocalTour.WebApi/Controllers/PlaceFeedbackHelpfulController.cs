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
    public class PlaceFeedbackHelpfulController : ControllerBase
    {
        private readonly IPlaceFeedbackHelpfulService _placeFeedbackHelpfulService;
        public PlaceFeedbackHelpfulController(IPlaceFeedbackHelpfulService placeFeedbackHelpfulService)
        {
            _placeFeedbackHelpfulService = placeFeedbackHelpfulService;
        }
        [HttpPost("likeOrUnlike")]
        [Authorize]
        public async Task<ActionResult<ServiceResponseModel<PlaceFeeedbackHelpful>>> LikeorUnlikeFeedback(int placeid, int placefeedbackid)
        {
            if (placefeedbackid == null)
            {
                return BadRequest(new ServiceResponseModel<PlaceFeeedbackHelpful>(false, "Request cannot be null"));
            }
            try
            {
                var feedback = await _placeFeedbackHelpfulService.CreateorDeleteHelpful(placeid, placefeedbackid);
                return Ok(new ServiceResponseModel<PlaceFeeedbackHelpful>(feedback)
                {
                    Message = "Successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ServiceResponseModel<PlaceFeeedbackHelpful>(false, $"An error occurred: {ex.Message}"));
            }
        }
    }
}
