using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SocialGoal.Web.Core.Extensions
{
    public static class SelectExtensions
    {
        /// <summary>Drop-down of an enum, with numeric values and [Description] texts.</summary>
        public static IHtmlContent EnumDropDownListFor<TModel, TEnum>(this IHtmlHelper<TModel> html, Expression<Func<TModel, TEnum>> expression, object htmlAttributes)
        {
            var items = Enum.GetValues(typeof(TEnum)).Cast<object>().Select(value => new SelectListItem
            {
                Value = Convert.ToInt32(value).ToString(),
                Text = typeof(TEnum).GetField(value.ToString()).GetCustomAttribute<DescriptionAttribute>()?.Description ?? value.ToString()
            });
            return html.DropDownListFor(expression, items.ToList(), htmlAttributes);
        }
    }
}
