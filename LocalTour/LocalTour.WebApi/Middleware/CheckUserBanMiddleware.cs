using System.Security.Claims;
using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;

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
        string phoneNumber = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.MobilePhone)?.Value;
        if (!string.IsNullOrEmpty(phoneNumber))
        {
            bool banned = await userService.IsUserBanned(phoneNumber);
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