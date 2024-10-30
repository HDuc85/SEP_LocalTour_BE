using System.Security.Claims;

namespace LocalTour.WebApi.Helper
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal user)
        {
            return user?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }
        public static string GetPhoneNumber(this ClaimsPrincipal user)
        {
            return user?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.MobilePhone)?.Value;
        }
        public static string GetFirebaseToken(this ClaimsPrincipal user)
        {
            return user?.Claims.FirstOrDefault(c => c.Type == "FirebaseToken")?.Value;
        }
        public static string GetRoleName(this ClaimsPrincipal user)
        {
            return user?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        }
    }
}
