using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using SocialGoal.Web.Core.Helpers;

namespace SocialGoal.Web.Core.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static UserHtmlHelper User(this IHtmlHelper html)
        {
            var factory = html.ViewContext.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
            return new UserHtmlHelper(factory.GetUrlHelper(html.ViewContext));
        }
    }
}
