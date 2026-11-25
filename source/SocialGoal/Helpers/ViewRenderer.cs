using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace SocialGoal.Web.Helpers
{
    public static class ViewRenderer
    {
        public static async Task<string> RenderAsync(ActionContext context, string viewName, ViewDataDictionary viewData, ITempDataDictionary tempData)
        {
            var engine = context.HttpContext.RequestServices.GetRequiredService<ICompositeViewEngine>();
            var result = viewName.StartsWith("~/")
                ? engine.GetView(null, viewName, false)
                : engine.FindView(context, viewName, false);
            if (!result.Success)
                throw new InvalidOperationException("View not found: " + viewName);
            using var writer = new StringWriter();
            await result.View.RenderAsync(new ViewContext(context, result.View, viewData, tempData, writer, new HtmlHelperOptions()));
            return writer.ToString();
        }
    }
}
