using FirebaseAdmin.Auth;
using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;
using LocalTour.WebApi.Helper;
using LocalTour.WebApi.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace LocalTour.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {

            var users = await _userService.GetAll();

            if (users == null)
            {
                return NotFound();
            }
            return Ok(users);
        }
        
        [HttpGet("pageSize")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<User>>> GetPageSize(int pageIndex, int pageSize)
        {
            var users = await _userService.GetAll();
            if (users == null)
            {
                return NotFound();
            }
            return Ok(users.Skip((pageIndex - 1) * pageSize).Take(pageSize));
        }

        [HttpPost("setPassword")]
        [Authorize]
        public async Task<IActionResult> SetPassword(string password)
        {
            string firebasetoken = User.GetFirebaseToken();
            FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(firebasetoken);
            string phoneNumber = decodedToken.Claims.ContainsKey("phoneNumber")
                                ? decodedToken.Claims["phoneNumber"].ToString()
                                : null;
            if (phoneNumber == null)
            {
                return BadRequest("Invalid Token");
            }
            var result = await _userService.SetPassword(phoneNumber,password);

            if (!result)
            {
                return BadRequest("Set Password Fail");
            }
            return Ok();
        }

        [HttpPost("changePassword")]
        [Authorize]
        public async Task<IActionResult> changePassword(string oldPassword, string newPassword)
        {
            string phoneNumber = User.GetPhoneNumber();
            
            if (phoneNumber == null)
            {
                return BadRequest("Invalid Token");
            }

            var result = await _userService.ChangePassword(phoneNumber, oldPassword, newPassword);

            if (!result)
            {
                return BadRequest("Set Password Fail");
            }
            return Ok();
        }


        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromForm]UpdateUserRequest updateUserRequest)
        {
            var requestUrl = $"{Request.Scheme}://{Request.Host}";
            string phonenumber = User.GetPhoneNumber();
            var result = await _userService.UpdateUser(phonenumber, updateUserRequest, requestUrl);
            if (result.Success)
            {
                return Accepted(result.Data);
            }
            return BadRequest();
        }

    }
}
