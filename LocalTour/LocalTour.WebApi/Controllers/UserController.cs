using FirebaseAdmin.Auth;
using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;
using LocalTour.WebApi.Helper;
using LocalTour.WebApi.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace LocalTour.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {

            var users = await _userService.GetAll();

            if (users.IsNullOrEmpty())
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
            if (users.IsNullOrEmpty())
            {
                return NotFound();
            }
            return Ok(users.Skip((pageIndex - 1) * pageSize).Take(pageSize));
        }

        [HttpPost("setPassword")]
        [Authorize]
        public async Task<IActionResult> SetPassword(string password)
        {
            string firebaseToken= User.GetFirebaseToken();
            UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(firebaseToken);
            string phoneNumber = userRecord.PhoneNumber;
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
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword)
        {
            string phoneNumber = User.GetPhoneNumber();
            
            if (phoneNumber.IsNullOrEmpty())
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
            string phoneNumber = User.GetPhoneNumber();
            var result = await _userService.UpdateUser(phoneNumber, updateUserRequest, requestUrl);
            if (!result.Success)
            {
                return BadRequest();
            }
            else
            {
                return Accepted(result.Data);

            }
        } 
        [HttpPost("addRole")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AddRole(string phoneNumber, string role)
        {

            var result = await _userService.AddRole(phoneNumber, role);
            if (result)
            {
                return Ok("Success");
            }
            return BadRequest();
        } 
    }
}
