using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace SocialGoal.Web.Helpers
{
    /// <summary>
    /// Replacement for MVC 5 Html.Action: runs a controller method inside the current request
    /// and renders its partial view (or plain value). Action filters do not run.
    /// </summary>
    public static class ChildActionExtensions
    {
        public static async Task<IHtmlContent> ChildActionAsync<TController>(this IHtmlHelper html, Expression<Func<TController, object>> action)
            where TController : Controller
        {
            var body = action.Body is UnaryExpression convert ? convert.Operand : action.Body;
            var actionName = ((MethodCallExpression)body).Method.Name;
            var controllerName = typeof(TController).Name.Replace("Controller", "");

            var http = html.ViewContext.HttpContext;
            var routeData = new RouteData();
            routeData.Values["controller"] = controllerName;
            routeData.Values["action"] = actionName;
            var context = new ActionContext(http, routeData, new ControllerActionDescriptor { ControllerName = controllerName, ActionName = actionName });

            var controller = ActivatorUtilities.CreateInstance<TController>(http.RequestServices);
            controller.ControllerContext = new ControllerContext(context);

            // shortcut: compiled on every call; cache by method if page rendering gets slow
            var result = action.Compile()(controller);

            return result switch
            {
                PartialViewResult partial => new HtmlString(await ViewRenderer.RenderAsync(context, partial.ViewName ?? actionName, partial.ViewData, partial.TempData)),
                ViewResult view => new HtmlString(await ViewRenderer.RenderAsync(context, view.ViewName ?? actionName, view.ViewData, view.TempData)),
                null or EmptyResult => HtmlString.Empty,
                IActionResult other => throw new NotSupportedException("Child action returned " + other.GetType().Name),
                _ => new HtmlString(System.Net.WebUtility.HtmlEncode(result.ToString()))
            };
        }
    }
}
