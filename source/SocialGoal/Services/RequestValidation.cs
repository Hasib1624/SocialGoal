using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace SocialGoal.Web.Services
{
    /// <summary>
    /// Replacement for ASP.NET request validation, which the original relied on: input that looks
    /// like markup is rejected before it can be stored and reach the page scripts.
    /// </summary>
    public static class RequestValidation
    {
        public static bool IsDangerous(string value)
        {
            for (var i = 0; value != null && i < value.Length - 1; i++)
            {
                var next = value[i + 1];
                if (value[i] == '<' && (char.IsAsciiLetter(next) || next == '!' || next == '/' || next == '?'))
                    return true;
                if (value[i] == '&' && next == '#')
                    return true;
            }
            return false;
        }

        public static void UseRequestValidation(this IApplicationBuilder app) => app.Use(async (context, next) =>
        {
            var values = context.Request.Query.AsEnumerable();
            if (context.Request.HasFormContentType)
                values = values.Concat(await context.Request.ReadFormAsync());
            if (values.Any(pair => pair.Value.Any(IsDangerous)))
                throw new InvalidOperationException("A potentially dangerous value was detected in the request.");
            await next(context);
        });
    }
}
