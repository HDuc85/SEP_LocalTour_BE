using LocalTour.Services.Abstract;
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
        [HttpGet("getallfeedback")]
        public async Task<ActionResult<PaginatedList<PlaceFeedbackRequest>>> GetAllFeedbacksByPlaceid([FromQuery] int placeid, [FromQuery] GetPlaceFeedbackRequest request)
        {
            var events = await _placeFeedbackService.GetAllFeedbackByPlace(placeid, request);
            return Ok(events);
        }
    }
}
