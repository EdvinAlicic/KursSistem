using System.Security.Claims;

namespace KursSistemDiplomskiRad.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string? GetUserEmail(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Email)?.Value;
        }
    }
}
