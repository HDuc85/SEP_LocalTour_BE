using LocalTour.Domain.Common;
using LocalTour.Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.Abstract
{
    public interface ITokenHandler
    {
        Task<(string, DateTime)> CreateRefreshToken(User user);
        Task<(string, DateTime)> CreateAccessToken(User user);
        Task ValidateToken(TokenValidatedContext context);
        Task<JwtModel> ValidateRefreshToken(string refreshToken);
        Task<(string, DateTime)> CreateAuthenFirebaseToken(User user, string firebaseToken);
    }
}
