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
    public class PlaceFeedbackHelpfulController : ControllerBase
    {
        private readonly IPlaceFeedbackHelpfulService _placeFeedbackHelpfulService;
        public PlaceFeedbackHelpfulController(IPlaceFeedbackHelpfulService placeFeedbackHelpfulService)
        {
            _placeFeedbackHelpfulService = placeFeedbackHelpfulService;
        }
        [HttpPost("like")]
        public async Task<ActionResult<ServiceResponseModel<PlaceFeeedbackHelpful>>> LikeFeedback([FromForm] int placeid, [FromForm] int placefeedbackid)
        {
            if (placefeedbackid == null)
            {
                return BadRequest(new ServiceResponseModel<PlaceFeeedbackHelpful>(false, "Request cannot be null"));
            }
            try
            {
                var feedback = await _placeFeedbackHelpfulService.CreateHelpful(placeid, placefeedbackid);
                return Ok(new ServiceResponseModel<PlaceFeeedbackHelpful>(feedback)
                {
                    Message = "Feedback created successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ServiceResponseModel<PlaceFeeedbackHelpful>(false, $"An error occurred: {ex.Message}"));
            }
        }
        [HttpDelete("unlike")]
        public async Task<ActionResult<bool>> UnlikeFeedback([FromForm] int placeid, [FromForm] int placefeedbackid, int helpfulid)
        {
            if (placefeedbackid == null)
            {
                return BadRequest(new ServiceResponseModel<PlaceFeeedbackHelpful>(false, "Request cannot be null"));
            }
            try
            {
                var feedback = await _placeFeedbackHelpfulService.DeleteHelpful(placeid, placefeedbackid, helpfulid);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ServiceResponseModel<PlaceFeeedbackHelpful>(false, $"An error occurred: {ex.Message}"));
            }
        }
    }
}
