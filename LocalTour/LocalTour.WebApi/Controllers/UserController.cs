using FirebaseAdmin.Auth;
using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;
using LocalTour.WebApi.Helper;
using LocalTour.Services.ViewModel;
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
        [Authorize(Roles = "Administrator")]
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
        [Authorize(Roles = "Administrator")]
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
            string email = userRecord.Email;
            if (phoneNumber.IsNullOrEmpty() && email.IsNullOrEmpty())
            {
                return BadRequest("Invalid Token");
            }
            var userId = User.GetUserId();
            var result = await _userService.SetPassword(userId,password);

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
            string userId = User.GetUserId();
            
            if (userId.IsNullOrEmpty())
            {
                return BadRequest("Invalid Token");
            }

            var result = await _userService.ChangePassword(userId, oldPassword, newPassword);

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
            string userId = User.GetUserId();
            var result = await _userService.UpdateUser(userId, updateUserRequest, requestUrl);
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
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> AddRole(string userId, string role)
        {
            var result = await _userService.AddRole(userId, role);
            if (result)
            {
                return Ok("Success");
            }
            return BadRequest();
        } 
        [HttpPost("removeRole")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> RemoveRole(string userId, string role)
        {
            var result = await _userService.RemoveRole(userId, role);
            if (result)
            {
                return Ok("Success");   
            }
            return BadRequest();
        }

        [HttpPut("BanUser")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> BanUser(string userId, DateTime endDate)
        {
            var result = await _userService.BanUser(userId, endDate);
            if (result)
            {
                return Ok("Success");
            }
            return BadRequest();    
        }

        [HttpPost("CreateModerate")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CreateModerate(CreateUserRequest createUserRequest)
        {
            if (createUserRequest.Password != createUserRequest.ConfirmPassword)
            {
                return BadRequest("Passwords do not match");
            }
            var user = await _userService.CreateModerate(createUserRequest);
            if (user != null)
            {
                return Ok("Success");
            }
            return BadRequest();
        }
    }
}
