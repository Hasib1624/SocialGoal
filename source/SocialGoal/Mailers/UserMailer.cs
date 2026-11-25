using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SocialGoal.Web.Helpers;

namespace SocialGoal.Web.Mailers
{
    public class UserMailer : IUserMailer
    {
        private readonly IHttpContextAccessor accessor;
        private readonly ITempDataDictionaryFactory tempData;
        private readonly IConfiguration mail;
        private readonly IWebHostEnvironment environment;
        private readonly ILogger<UserMailer> logger;

        public UserMailer(IHttpContextAccessor accessor, ITempDataDictionaryFactory tempData, IConfiguration configuration, IWebHostEnvironment environment, ILogger<UserMailer> logger)
        {
            this.accessor = accessor;
            this.tempData = tempData;
            this.mail = configuration.GetSection("Mail");
            this.environment = environment;
            this.logger = logger;
        }

        public OutgoingMail Welcome() => Create("Welcome", "Welcome", null, null, null);

        public OutgoingMail Invite(string email, Guid groupIdToken) => Create("Invite", "Invite", email, "group", "gr:" + groupIdToken);

        public OutgoingMail Support(string email, Guid goalIdToken) => Create("Support My Goal", "SupportGoal", email, "goal", "go:" + goalIdToken);

        public OutgoingMail ResetPassword(string email, Guid passwordResetToken) => Create("Reset Password", "PasswordReset", email, "token", "pwreset:" + passwordResetToken);

        public OutgoingMail InviteNewUser(string email, Guid registrationToken) => Create("Invitation to SocialGoal", "InviteNewUser", email, "token", "reg:" + registrationToken);

        private OutgoingMail Create(string subject, string view, string email, string key, string value)
        {
            var http = accessor.HttpContext;
            var context = new ActionContext(http, http.GetRouteData() ?? new RouteData(), new ActionDescriptor());
            var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());
            // links come from configuration, not the Host header, so a forged request cannot redirect tokens
            viewData["BaseUrl"] = mail["BaseUrl"]?.TrimEnd('/');
            if (key != null)
                viewData[key] = value;

            var message = new OutgoingMail(mail, environment.ContentRootPath, logger)
            {
                Subject = subject,
                IsBodyHtml = true,
                Body = ViewRenderer.RenderAsync(context, $"~/Views/UserMailer/{view}.cshtml", viewData, tempData.GetTempData(http)).GetAwaiter().GetResult()
            };
            if (email != null)
                message.To.Add(email);
            return message;
        }
    }
}
