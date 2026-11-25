using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace SocialGoal.Web.Core.Helpers
{
    public class UserHtmlHelper
    {
        private readonly IUrlHelper _url;

        public UserHtmlHelper(IUrlHelper url)
        {
            _url = url;
        }

        public IHtmlContent Avatar(string profilePicUrl, object htmlAttributes = null)
        {
            var src = string.IsNullOrEmpty(profilePicUrl)
                ? "../../Content/templatemo_329_blue_urban/images/facebook-avatar.png"
                : _url.Content(profilePicUrl);
            var tag = new TagBuilder("img") { TagRenderMode = TagRenderMode.StartTag };
            tag.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), true);
            tag.AddCssClass("thumbnail");
            tag.Attributes["src"] = src;
            return tag;
        }
    }
}
