using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using SocialGoal.Model.Models;

namespace SocialGoal.Tests.Helpers
{
    public static class Fakes
    {
        public const string UserId = "402bd590-fdc7-49ad-9728-40efbfe512ec";

        /// <summary>Request context of a signed-in user.</summary>
        public static ControllerContext SignedIn(HttpContext http = null)
        {
            http = http ?? new DefaultHttpContext();
            http.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, UserId),
                new Claim(ClaimTypes.Name, "adarsh")
            }, "test"));
            return new ControllerContext { HttpContext = http };
        }

        public static IHttpContextAccessor Accessor() =>
            new HttpContextAccessor { HttpContext = new DefaultHttpContext { Session = Mock.Of<ISession>() } };

        public static ITempDataDictionary TempData() =>
            new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

        public static HttpContext HttpWithAuthentication()
        {
            var services = new Mock<IServiceProvider>();
            services.Setup(s => s.GetService(typeof(IAuthenticationService))).Returns(Mock.Of<IAuthenticationService>());
            return new DefaultHttpContext { RequestServices = services.Object };
        }

        public static Mock<UserManager<ApplicationUser>> UserManager() =>
            new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(),
                null, null, null, null, null, null, null, null);

        public static Mock<SignInManager<ApplicationUser>> SignInManager(UserManager<ApplicationUser> users) =>
            new Mock<SignInManager<ApplicationUser>>(users, Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(), null, null, null, null);
    }
}
