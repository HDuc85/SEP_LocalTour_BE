using FirebaseAdmin.Auth;
using LocalTour.Domain.Common;
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
        private readonly ITokenHandler _tokenHandler;

        public UserController(IUserService userService, ITokenHandler tokenHandler)
        {
            _tokenHandler = tokenHandler;
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
            var user = await _userService.FindById(userId);
            var result = await _userService.SetPassword(user.Id.ToString(),request.Password);

            if (!result)
            {
                return BadRequest("Set Password Fail");
            }
            
            (string accessToken, DateTime expiredDateAccessToken) = await _tokenHandler.CreateAccessToken(user);
            (string refreshToken, DateTime expiredDateRefreshToken) = await _tokenHandler.CreateRefreshToken(user);
            
            return Ok(new JwtModel
            {
                accessToken = accessToken,
                refreshToken = refreshToken,
                userId = user.Id.ToString(),
                accessTokenExpiredDate = expiredDateAccessToken,
                refeshTokenExpiredDate = expiredDateRefreshToken
            });
            return Ok();
        }

        [HttpPost("changePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            var result = await _userService.ChangePassword(User.GetUserId(),request);
                return Ok(result.Data);
        }


        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromForm]UpdateUserRequest updateUserRequest)
        {
            string userId = User.GetUserId();
            try
            {
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
            catch (Exception e)
            {
                return BadRequest(e.Message);
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

        [HttpPut("UpdatePhoneOrEmail")]
        [Authorize]
        public async Task<IActionResult> UpdatePhoneOrEmail([FromBody] TokenRequest request)
        {
            try
            {
                UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(request.Token);
                string phoneNumber = userRecord.PhoneNumber;
                string email = userRecord.Email;
                User user = null;

                if (phoneNumber != null)
                {
                    phoneNumber = "0" + phoneNumber.Substring(3);
                    user = await _userService.FindByPhoneNumber(phoneNumber);
                }
                if (user == null && !string.IsNullOrEmpty(email))
                {
                    user = await _userService.FindByEmail(email);
                }

                if (user == null)
                {
                    var CurrenUser = await _userService.FindById(User.GetUserId());
                    if (phoneNumber != null && (CurrenUser.PhoneNumber == null || CurrenUser.PhoneNumber == ""))
                    {
                        CurrenUser.PhoneNumber = phoneNumber;
                        await _userService.UpdatePhoneOrEmail(CurrenUser);
                        return Ok("Success");
                    }

                    if (email != null && (CurrenUser.Email == null || CurrenUser.Email == ""))
                    {
                        CurrenUser.Email = email;
                        await _userService.UpdatePhoneOrEmail(CurrenUser);
                        return Ok("Success");
                    }
                    return BadRequest("Nothing to be updated");
                }
                return BadRequest("Email or Phone Number have exist in another account");

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpDelete("RemovePhone")]
        [Authorize]
        public async Task<IActionResult> RemovePhone()
        {
            var result = await _userService.RemovePhone(User.GetUserId());
            if (result)
            {
                return Ok("Success");
            }
            return BadRequest();
        }
        [HttpDelete("RemoveEmail")]
        [Authorize]
        public async Task<IActionResult> RemoveEmail()
        {
            var result = await _userService.RemoveEmail(User.GetUserId());
            if (result)
            {
                return Ok("Success");
            }
            return BadRequest();
        }

        [HttpPost("UnBan/{userId}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UnbanUser(String userId)
        {
            var result = await _userService.UnbanUser(userId);

            if (result)
            {
                return Ok(new { message = "User has been unbanned successfully." });
            }
            return BadRequest(new { message = "Failed to unban user. User may not exist in the ban list." });
        }

        [HttpGet("getlist")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<List<GetUserRequest>>> GetListUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
