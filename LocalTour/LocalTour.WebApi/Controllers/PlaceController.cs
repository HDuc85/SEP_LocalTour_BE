using Azure;
using Azure.Core;
using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;
using LocalTour.Services.Model;
using LocalTour.Services.Services;
using LocalTour.Services.ViewModel;
using LocalTour.Services.ViewModel;
using LocalTour.WebApi.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Net.payOS;
using Net.payOS.Types;
using Response = LocalTour.Services.ViewModel.Response;

namespace LocalTour.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class PlaceController : ControllerBase
    {
        private readonly IPlaceService _placeService;
        private readonly PayOS _payOS;
        public PlaceController(IPlaceService placeService, PayOS payOS)
        {
            _placeService = placeService;
            _payOS = payOS;
        }
        [HttpPost("create")]
        [Authorize(Roles = "Service Owner")]
        public async Task<ActionResult<ApiReponseModel<PlaceRequest>>> CreatePlace(PlaceRequest request)
        {
            if (request == null)
            {
                return BadRequest(new ApiReponseModel<PlaceRequest>(false, "Request cannot be null"));
            }
            try
            {
                var place = await _placeService.CreatePlace(request);
                return Ok(place);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiReponseModel<PlaceRequest>(false, $"An error occurred: {ex.Message}"));
            }
        }
        [HttpGet("getAll")]
        public async Task<ActionResult<PaginatedList<PlaceVM>>> GetAllPlaces([FromQuery] GetPlaceRequest request)
        {
            try
            {
                var places = await _placeService.GetAllPlace(request);
                return Ok(places);
            }
            catch (Exception ex)
            {
                return StatusCode(400, new ApiReponseModel<PlaceVM>(false, $"An error occurred: {ex.Message}"));
            }
        }
        [HttpGet("getPlaceById")]
        public async Task<ActionResult<ApiReponseModel<Place>>> GetPlaceById(string? languageCode, int placeid)
        {
            try
            {
                var placeEntity = await _placeService.GetPlaceById(languageCode,placeid);
                return Ok(placeEntity);
            }
            catch (Exception ex)
            {
                return StatusCode(400, new ApiReponseModel<Place>(false, $"An error occurred: {ex.Message}"));
            }
        }
        [HttpGet("getTagsInPlace")]
        public async Task<ActionResult> getTagsInPlace([FromQuery] int placeId)
        {
            try
            {
                var places = await _placeService.GetTagsByPlaceId(placeId);
                return Ok(places);
            }
            catch (Exception ex)
            {
                return StatusCode(400, new ApiReponseModel<PlaceVM>(false, $"An error occurred: {ex.Message}"));
            }
        }
        [HttpPut("update")]
        [Authorize]
        public async Task<ActionResult<ApiReponseModel<PlaceUpdateRequest>>> UpdatePlace(int placeid,PlaceUpdateRequest request)
        {
            if (request == null)
            {
                return BadRequest(new ApiReponseModel<PlaceRequest>(false, "Request cannot be null"));
            }
            try
            {
                var place = await _placeService.UpdatePlace(placeid, request);
                return Ok(place);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiReponseModel<PlaceRequest>(false, $"An error occurred: {ex.Message}"));
            }
        }
        [HttpPut("changeStatusPlace")]
        [Authorize(Roles = "Moderator")]
        public async Task<ActionResult<Place>> ChangeStatusPlace([FromQuery] int placeid, [FromQuery] string status)
        {
            try
            {
                var placeEntity = await _placeService.ChangeStatusPlace(placeid, status);
                return Ok(placeEntity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiReponseModel<Place>(false, $"An error occurred: {ex.Message}"));
            }
        }
        
        [HttpPut("transferAuthor")]
        [Authorize(Roles = "Service Owner")]
        public async Task<ActionResult<Place>> TransferAuthor([FromQuery] int placeId, [FromQuery] Guid userIdTransfer)
        {
            try
            {
                var placeEntity = await _placeService.TransferAuthor(placeId, userIdTransfer);
                return Ok("Success");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiReponseModel<Place>(false, $"An error occurred: {ex.Message}"));
            }
        }
        [HttpDelete("delete")]
        [Authorize(Roles = "Service Owner")]
        public async Task<ActionResult<Place>> DeletePlace(int placeid)
        {
            if (placeid == null)
            {
                return BadRequest(new ApiReponseModel<Place>(false, "Request cannot be null"));
            }
            try
            {
                var placeEntity = await _placeService.DeletePlace(placeid);
                return Ok(new ApiReponseModel<bool>(true)
                {
                    message = "Successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiReponseModel<Place>(false, $"An error occurred: {ex.Message}"));
            }
        }
        [HttpGet("getAllByRole")]
        [Authorize]
        public async Task<ActionResult<PaginatedList<PlaceVM>>> GetAllPlacesByRole([FromQuery] GetPlaceRequest request)
        {
            try
            {
                var places = await _placeService.GetAllPlaceByRole(request);
                return Ok(places);
            }
            catch (Exception ex)
            {
                return StatusCode(400, new ApiReponseModel<PlaceVM>(false, $"An error occurred: {ex.Message}"));
            }
        }
        
        [HttpGet("GetUrlPlaceRegister")]
        [Authorize]
        public async Task<ActionResult> CreatePaymentPlaceRegister(int placeId)
        {
            try
            {
                var userId = User.GetUserId();
               // var userId = "a487545c-c732-4b58-b1bf-830258328184";
                var places = await _placeService.CreatePaymentPlaceRegister(placeId,userId);
                return Ok(places.checkoutUrl);
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }
        [HttpGet("PlaceSuccessPayment")]
        public async Task<ActionResult> ComfirmPaymentRegister([FromQuery]ReturnUrlPayOS returnUrlPayOS)
        {
            try
            {
                var places = await _placeService.PlaceSuccessPayment(returnUrlPayOS.orderCode,returnUrlPayOS.status);
               
                return RedirectPermanent(places);
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }
        [HttpGet("PlaceCancelPayment")]
        public async Task<IActionResult> CancelOrder([FromQuery]ReturnUrlPayOS returnUrlPayOS)
        {
            try
            {
                var places = await _placeService.PlaceCancelPayment(returnUrlPayOS.orderCode,returnUrlPayOS.status);
               
                return RedirectPermanent(places);
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }
        [HttpPost("sendMail")]
        [Authorize(Roles = "Moderator")]
        public async Task<IActionResult> SendMail(SendMailRequest request)
        {
            try
            {
                var isSend = await _placeService.sendMail(request);
                if (isSend)
                {
                    return Ok();
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }
    }
}
