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
        public async Task<ActionResult> GetAllUsers()
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
        public async Task<ActionResult> GetPageSize(int pageIndex, int pageSize)
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
        public async Task<IActionResult> SetPassword(SetPasswordRequest request)
        {
            if (request.Password != request.ConfirmPassword)
            {
                return BadRequest("Passwords do not match");
            }
            string firebaseToken= User.GetFirebaseToken();
            UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(firebaseToken);
            string phoneNumber = userRecord.PhoneNumber;
            string email = userRecord.Email;
            if (phoneNumber.IsNullOrEmpty() && email.IsNullOrEmpty())
            {
                return BadRequest("Invalid Token");
            }
            var userId = User.GetUserId();
            var result = await _userService.SetPassword(userId,request.Password);

            if (!result)
            {
                return BadRequest("Set Password Fail");
            }
            return Ok();
        }

        [HttpPost("changePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            var result = await _userService.ChangePassword(User.GetUserId(),request);

            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }


        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromForm]UpdateUserRequest updateUserRequest)
        {
            string userId = User.GetUserId();
            var result = await _userService.UpdateUser(userId, updateUserRequest);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            else
            {
                return Ok(new UserProfileVM()
                {
                    phoneNumber = result.Data.PhoneNumber,
                    email = result.Data.Email,
                    address = result.Data.Address,
                    gender = result.Data.Gender,
                    userName = result.Data.UserName,
                    dateOfBirth = result.Data.DateOfBirth,
                    userProfileImage = result.Data.ProfilePictureUrl
                });

            }
        }

        [HttpGet("getProfile")]
        [Authorize]
        public async Task<ActionResult> getProfile(string userId)
        {
            var currentUserId = User.GetUserId();
            var result = await _userService.GetProfile(userId, currentUserId);
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
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
