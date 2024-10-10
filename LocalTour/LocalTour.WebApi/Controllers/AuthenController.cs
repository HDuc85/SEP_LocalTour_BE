using LocalTour.Domain;
using LocalTour.Services.Abstract;
using LocalTour.WebApi.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            (string refreshToken, DateTime expiredDateRefreshToken, string codeRefreshToken) = await _tokenHandler.CreateRefreshToken(user);

            return Ok(new JwtModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                AccessTokenExpiredDate = expiredDateAccessToken
            });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody]string refreshToken)
        {
            var validate = await _tokenHandler.ValidateRefreshToken(refreshToken);
            if (validate.PhoneNumber == null)
                return Unauthorized("Invalid RefreshToken");
            return Ok(validate);
        }
    }
}
