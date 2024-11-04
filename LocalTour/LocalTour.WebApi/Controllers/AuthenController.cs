using FirebaseAdmin.Auth;
using LocalTour.Domain;
using LocalTour.Domain.Common;
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
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            
            var user = await _userService.CheckLogin(loginRequest.PhoneNumber,loginRequest.Password);
          
            if (user == null)
            {
                return Unauthorized();
            }

            (string accessToken, DateTime expiredDateAccessToken) = await _tokenHandler.CreateAccessToken(user);
            (string refreshToken, DateTime expiredDateRefreshToken) = await _tokenHandler.CreateRefreshToken(user);

            return Ok(new JwtModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                AccessTokenExpiredDate = expiredDateAccessToken,
                RefeshTokenExpiredDate = expiredDateRefreshToken
            });
        }

        [HttpPost("refreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody]string refreshToken)
        {
            var validate = await _tokenHandler.ValidateRefreshToken(refreshToken);
            if (validate.PhoneNumber == null)
                return Unauthorized("Invalid RefreshToken");
            return Ok(validate);
        }

        [HttpPost("verifyTokenFirebase")]
        public async Task<IActionResult> VerifyToken(string idToken)
        {
            try
            {
                UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(idToken);
                string phoneNumber = userRecord.PhoneNumber;
                string email = userRecord.Email;
                
                var user = await _userService.FindByPhoneNumber(phoneNumber);
                if (user == null)
                {
                user = await _userService.FindByEmail(email);
                }
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
                }

                (string firebaseAuthToken, DateTime expiredDateToken) = await _tokenHandler.CreateAuthenFirebaseToken(user,idToken);

                return Ok(new 
                {
                    firebaseAuthToken = firebaseAuthToken,
                    expiredDateToken = expiredDateToken,
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
