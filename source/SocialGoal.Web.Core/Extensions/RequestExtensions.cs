using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;

namespace SocialGoal.Web.Core.Extensions
{
    public static class RequestExtensions
    {
        public static string GetUserId(this IIdentity identity) =>
            (identity as ClaimsIdentity)?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public static bool IsAjaxRequest(this HttpRequest request) =>
            request.Headers["X-Requested-With"] == "XMLHttpRequest" || request.Query["X-Requested-With"] == "XMLHttpRequest";
    }
}
