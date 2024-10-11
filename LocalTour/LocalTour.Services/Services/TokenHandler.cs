using LocalTour.Domain.Entities;
using LocalTour.Domain.Models;
using LocalTour.Services.Abstract;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.Services
{
    public class TokenHandler : ITokenHandler
    {
        IConfiguration _configuration;
        IUserService _userService;
        UserManager<User> _userManager;

        public TokenHandler(IConfiguration configuration, IUserService userService, UserManager<User> userManager)
        {
            _userManager = userManager;
            _userService = userService;
            _configuration = configuration;
        }

        public async Task<(string, DateTime)> CreateAccessToken(User user)
        {
            DateTime expiredToken = DateTime.Now.AddMinutes(double.Parse(_configuration["JWT:AccessTokenExiredByMinutes"]));

            var roles = await _userManager.GetRolesAsync(user);

            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString(),ClaimValueTypes.String,_configuration["JWT:Issuer"]),
                new Claim(JwtRegisteredClaimNames.Iss, _configuration["JWT:Issuer"],ClaimValueTypes.String,_configuration["JWT:Issuer"]),
                new Claim(JwtRegisteredClaimNames.Iat, EpochTime.GetIntDate(DateTime.Now).ToString(CultureInfo.InvariantCulture), ClaimValueTypes.Integer64),
                new Claim(JwtRegisteredClaimNames.Aud, _configuration["JWT:Audience"],ClaimValueTypes.String,_configuration["JWT:Issuer"]),
                new Claim(JwtRegisteredClaimNames.Exp, DateTime.Now.AddMinutes(double.Parse(_configuration["JWT:AccessTokenExiredByMinutes"])).ToString("dd/MM/yyyy hh:mm:ss"),ClaimValueTypes.String,_configuration["JWT:Issuer"]),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(),ClaimValueTypes.String,_configuration["JWT:Issuer"]),
                new Claim(ClaimTypes.MobilePhone,user.PhoneNumber, ClaimValueTypes.String, _configuration["JWT:Issuer"])

            }.Union(roles.Select(x => new Claim(ClaimTypes.Role, x)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenInfo = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddMinutes(double.Parse(_configuration["JWT:AccessTokenExiredByMinutes"])),
                signingCredentials: credential
                );

            string accessToken = new JwtSecurityTokenHandler().WriteToken(tokenInfo);

            await _userManager.SetAuthenticationTokenAsync(user, "AccessTokenProvider", "AccessToken", accessToken);

            return await Task.FromResult((accessToken, expiredToken));
        }

        public async Task<(string, DateTime, string)> CreateRefreshToken(User user)
        {
            DateTime expiredToken = DateTime.Now.AddDays(double.Parse(_configuration["JWT:RefreshTokenExiredByDay"]));
            string code = Guid.NewGuid().ToString();
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString(),ClaimValueTypes.String,_configuration["JWT:Issuer"]),
                new Claim(JwtRegisteredClaimNames.Iss, _configuration["JWT:Issuer"],ClaimValueTypes.String,_configuration["JWT:Issuer"]),
                new Claim(JwtRegisteredClaimNames.Iat, EpochTime.GetIntDate(DateTime.Now).ToString(CultureInfo.InvariantCulture), ClaimValueTypes.Integer64),
                new Claim(JwtRegisteredClaimNames.Aud, _configuration["JWT:Audience"],ClaimValueTypes.String,_configuration["JWT:Issuer"]),
                new Claim(JwtRegisteredClaimNames.Exp, DateTime.Now.AddDays(double.Parse(_configuration["JWT:RefreshTokenExiredByDay"])).ToString("dd/MM/yyyy hh:mm:ss"),ClaimValueTypes.String,_configuration["JWT:Issuer"]),
                new Claim(ClaimTypes.SerialNumber,code,ClaimValueTypes.String, _configuration["JWT:Issuer"]),
                new Claim(ClaimTypes.MobilePhone,user.PhoneNumber, ClaimValueTypes.String, _configuration["JWT:Issuer"])
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenInfo = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddDays(double.Parse(_configuration["JWT:RefreshTokenExiredByDay"])),
                signingCredentials: credential
                );


            string refreshToken = new JwtSecurityTokenHandler().WriteToken(tokenInfo);

            await _userManager.SetAuthenticationTokenAsync(user, "RefreshTokenProvider", "RefreshToken", refreshToken);

            return await Task.FromResult((refreshToken, expiredToken, code));
        }

        public async Task ValidateToken(TokenValidatedContext context)
        {
            var claims = context.Principal.Claims.ToList();

            if (claims.Count == 0)
            {
                context.Fail("Token has no info");
                return;
            }

            var identity = context.Principal.Identity as ClaimsIdentity;

            if (identity.FindFirst(JwtRegisteredClaimNames.Iss) == null)
            {
                context.Fail("Token is not issued by point entry");
                return;
            }

            if (identity.FindFirst(ClaimTypes.MobilePhone) == null)
            {
                string userid = identity.FindFirst(ClaimTypes.NameIdentifier).Value;

                var user = await _userService.FindById(userid);
                if (user == null)
                {
                    context.Fail("Token is invalid for user");
                    return;

                }

            }

            if (identity.FindFirst(JwtRegisteredClaimNames.Exp) == null)
            {
                var dateExp = identity.FindFirst(JwtRegisteredClaimNames.Exp).Value;

                long exp = long.Parse(dateExp);
                var date = DateTimeOffset.FromUnixTimeSeconds(exp).DateTime;

                var minutes = date.Subtract(DateTime.Now).TotalMinutes;

                if (minutes < 0)
                {
                    context.Fail("Token is expired");

                    throw new Exception("Token is expired");


                }

            }


        }

        public async Task<JwtModel> ValidateRefreshToken(string refreshToken)
        {
            JwtModel jwtModel = new();

            var claimPriciple = new ClaimsPrincipal();
            try
            {
                claimPriciple = new JwtSecurityTokenHandler().ValidateToken(refreshToken, new TokenValidationParameters
                {
                    RequireExpirationTime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"])),
                    ValidateIssuerSigningKey = true,

                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidIssuer = _configuration["JWT:Issuer"],
                    ValidAudience = _configuration["JWT:Audience"],
                    ClockSkew = TimeSpan.Zero,
                }, out _
            );
            }
            catch
            {
                return new();
            }

            if (claimPriciple == null) return new();
            string userid = claimPriciple?.Claims?.FirstOrDefault(s => s.Type == ClaimTypes.NameIdentifier)?.Value;

            var user = await _userManager.FindByIdAsync(userid);

            var token = await _userManager.GetAuthenticationTokenAsync(user, "AccessTokenProvider", "AccessToken");

            if (string.IsNullOrEmpty(token)) return new();



            (string newAccessToken, DateTime createdDateAccessToken) = await CreateAccessToken(user);
            (string newRefreshToken, DateTime createdDateRefreshToken, string newCode) = await CreateRefreshToken(user);

            return new JwtModel
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                PhoneNumber = user.PhoneNumber,
                FullName = user.FullName
            };



        }

    }
}
