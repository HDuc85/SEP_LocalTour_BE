using System.Security.Claims;
using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;
using LocalTour.WebApi.Helper;

namespace LocalTour.WebApi.Middleware;

public class CheckUserBanMiddleware
{
    private readonly RequestDelegate _next;

    public CheckUserBanMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IUserService userService)
    {
        string userId = context.User.GetUserId();
       
        if (!string.IsNullOrEmpty(userId))
        {
            bool banned = await userService.IsUserBanned(userId);
            if (banned)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("User is banned.");
                return;
            }
        }
        await _next(context);
    }
}