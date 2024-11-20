using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using LocalTour.Domain;
using LocalTour.Domain.Common;
using LocalTour.Domain.Entities;
using LocalTour.Services.ViewModel;
using LocalTour.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace LocalTour.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenHandler _tokenHandler;

        public AuthenController(IUserService userService, ITokenHandler tokenHandler)
        {
            _userService = userService;
            _tokenHandler = tokenHandler;
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromFile("firebaseServiceAccount.json")
                });
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            
            var result = await _userService.CheckLogin(loginRequest.PhoneNumber,loginRequest.Password);
          
            if (!result.Success)
            {
                return Unauthorized(result.Message);
            }

            (string accessToken, DateTime expiredDateAccessToken) = await _tokenHandler.CreateAccessToken(result.Data);
            (string refreshToken, DateTime expiredDateRefreshToken) = await _tokenHandler.CreateRefreshToken(result.Data);
            
            return Ok(new JwtModel
            {
                accessToken = accessToken,
                refreshToken = refreshToken,
                userId = result.Data.Id.ToString(),
                accessTokenExpiredDate = expiredDateAccessToken,
                refeshTokenExpiredDate = expiredDateRefreshToken
            });
        }

        [HttpPost("refreshToken")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody]TokenRequest request)
        {
            var validate = await _tokenHandler.ValidateRefreshToken(request.Token);
            if (validate.userId == null)
                return Unauthorized("Invalid RefreshToken");
            return Ok(validate);
            
        }

        [HttpPost("verifyTokenFirebase")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyToken([FromBody]TokenRequest request)
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

                bool firstTime = false;
                if (user == null)
                {
                    user = await _userService.CreateUser(new Domain.Entities.User
                    {
                        PhoneNumber = phoneNumber,
                        DateCreated = DateTime.UtcNow,
                        DateUpdated = DateTime.UtcNow,
                        PhoneNumberConfirmed = !phoneNumber.IsNullOrEmpty(),
                        EmailConfirmed = !email.IsNullOrEmpty(),
                        Email = email,
                        UserName = phoneNumber.IsNullOrEmpty()?email:phoneNumber,
                        Id = Guid.NewGuid()
                    });
                    firstTime = true;
                }
                
                (string firebaseAuthToken, DateTime expiredDateToken) = await _tokenHandler.CreateAuthenFirebaseToken(user,request.Token);
                if(user.PasswordHash == null) firstTime = true;
                return Ok(new 
                {
                    firebaseAuthToken = firebaseAuthToken,
                    expiredDateToken = expiredDateToken,
                    firstTime = firstTime,
                    userId = user.Id.ToString()
                });
            }
            catch (FirebaseAuthException ex)
            {
                if (ex.AuthErrorCode == AuthErrorCode.InvalidIdToken)
                {
                    return BadRequest("Token không hợp lệ.");
                }
                else
                {
                    return StatusCode(500, "Lỗi xác thực token.");
                }
            }
        }

        
    }
}
