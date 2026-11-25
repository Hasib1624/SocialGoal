using System;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SocialGoal.Data.Infrastructure;
using SocialGoal.Data.Models;
using SocialGoal.Data.Repository;
using SocialGoal.Model.Models;
using SocialGoal.Service;
using SocialGoal.Web.Helpers;
using SocialGoal.Web.Mailers;
using SocialGoal.Web.Services;

// the original ran on an en-US server; dates are formatted and parsed with the current culture
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddDbContext<SocialGoalEntities>(o => o
    .UseLazyLoadingProxies()
    .UseSqlServer(builder.Configuration.GetConnectionString("SocialGoalEntities")));

services.AddScoped<IUnitOfWork, UnitOfWork>();
services.AddScoped<IDatabaseFactory, DatabaseFactory>();
foreach (var (assembly, suffix) in new[] { (typeof(FocusRepository).Assembly, "Repository"), (typeof(GoalService).Assembly, "Service") })
{
    foreach (var type in assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith(suffix)))
    {
        foreach (var contract in type.GetInterfaces())
            services.AddScoped(contract, type);
    }
}

// same rules as ASP.NET Identity 1.0: only a minimum password length, no lockout
services.AddIdentity<ApplicationUser, IdentityRole>(o =>
    {
        o.Password.RequiredLength = 6;
        o.Password.RequiredUniqueChars = 1;
        o.Password.RequireDigit = o.Password.RequireLowercase = o.Password.RequireUppercase = o.Password.RequireNonAlphanumeric = false;
        o.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789@_.";
        o.Lockout.AllowedForNewUsers = false;
    })
    .AddEntityFrameworkStores<SocialGoalEntities>()
    .AddErrorDescriber<LegacyIdentityErrors>();
services.ConfigureApplicationCookie(o => o.LoginPath = "/Account/Login");

var google = builder.Configuration.GetSection("Authentication:Google");
if (!string.IsNullOrEmpty(google["ClientId"]))
{
    services.AddAuthentication().AddGoogle(o =>
    {
        o.ClientId = google["ClientId"];
        o.ClientSecret = google["ClientSecret"];
    });
}

services.AddControllersWithViews().AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = null);
services.AddHttpContextAccessor();
services.AddSession();
services.AddScoped<IUserMailer, UserMailer>();
services.AddSingleton<IImageFetcher, ImageFetcher>();

var app = builder.Build();

SocialGoalSessionFacade.Accessor = app.Services.GetRequiredService<IHttpContextAccessor>();

// shortcut: migrates on startup, fine for one instance; move to a deploy step before scaling out
using (var scope = app.Services.CreateScope())
    scope.ServiceProvider.GetRequiredService<SocialGoalEntities>().Database.Migrate();

app.UseExceptionHandler(error => error.Run(async context =>
{
    // the exception message is shown in Development only, as the original did on a local machine
    var detail = app.Environment.IsDevelopment()
        ? System.Net.WebUtility.HtmlEncode(context.Features.Get<IExceptionHandlerFeature>()?.Error.Message)
        : "An error occurred while processing your request.";
    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.WriteAsync($"<!DOCTYPE html><title>Server Error</title><h1>Server Error</h1><p>{detail}</p>");
}));
app.UseStaticFiles();
app.UseRequestValidation();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
app.Run();
