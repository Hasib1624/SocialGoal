using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SocialGoal.Mappings;
using SocialGoal.Web.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SocialGoal.Models;
using SocialGoal.Data.Models;
using SocialGoal.Model.Models;
using SocialGoal.Service;
using SocialGoal.Web.Mailers;
using SocialGoal.Web.ViewModels;
using SocialGoal.Properties;
using System.IO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using SkiaSharp;
using SocialGoal.Web.Services;
using SocialGoal.Web.Helpers;

namespace SocialGoal.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private IUserService userService;
        private IUserProfileService userProfileService;
        private IGoalService goalService;
        private IUpdateService updateService;
        private ICommentService commentService;
        private IFollowRequestService followRequestService;
        private IFollowUserService followUserService;
        private ISecurityTokenService securityTokenService;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IWebHostEnvironment environment;
        private readonly IImageFetcher imageFetcher;
        private UserManager<ApplicationUser> UserManager;
        public AccountController(IUserService userService, IUserProfileService userProfileService, IGoalService goalService, IUpdateService updateService, ICommentService commentService, IFollowRequestService followRequestService, IFollowUserService followUserService, ISecurityTokenService securityTokenService, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IWebHostEnvironment environment, IImageFetcher imageFetcher)
        {
            this.userService = userService;
            this.userProfileService = userProfileService;
            this.goalService = goalService;
            this.updateService = updateService;
            this.commentService = commentService;
            this.followRequestService = followRequestService;
            this.followUserService = followUserService;
            this.securityTokenService = securityTokenService;
            this.UserManager = userManager;
            this.signInManager = signInManager;
            this.environment = environment;
            this.imageFetcher = imageFetcher;
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (Request.Query.ContainsKey("guid"))
                SocialGoalSessionFacade.JoinGroupOrGoal = Request.Query["guid"];
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user != null && await UserManager.CheckPasswordAsync(user, model.Password))
                {
                    await SignInAsync(user, model.RememberMe);
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser() { UserName = model.UserName };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var userId = user.Id;
                    var userName = user.UserName;
                    userProfileService.CreateUserProfile(userId);
                    await SignInAsync(user, isPersistent: false);
                    if (SocialGoalSessionFacade.JoinGroupOrGoal != null)
                    {
                        string groupOrGoalJoinToken = SocialGoalSessionFacade.JoinGroupOrGoal;
                        if (groupOrGoalJoinToken.StartsWith("gr:"))
                        {
                            Guid groupIdToken = new Guid(groupOrGoalJoinToken.Substring(3));
                            TempData["grToken"] = groupIdToken;
                            return RedirectToAction("AddGroupUser", "EmailRequest");
                        }
                        else if (groupOrGoalJoinToken.StartsWith("go:"))
                        {
                            Guid goalIdToken = new Guid(groupOrGoalJoinToken.Substring(3));
                            TempData["goToken"] = goalIdToken;
                            return RedirectToAction("AddSupportToGoal", "EmailRequest");
                        }
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        
        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            IdentityResult result = await UserManager.RemoveLoginAsync(await CurrentUserAsync(), loginProvider, providerKey);
            if (result.Succeeded)
            {
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage
        public async Task<ActionResult> Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            ViewBag.HasLocalPassword = await HasPasswordAsync();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            bool hasPassword = await HasPasswordAsync();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(await CurrentUserAsync(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                var state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                    state.ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.AddPasswordAsync(await CurrentUserAsync(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            var redirect = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            return Challenge(signInManager.ConfigureExternalAuthenticationProperties(provider, redirect), provider);
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await signInManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = await UserManager.FindByLoginAsync(loginInfo.LoginProvider, loginInfo.ProviderKey);
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // If the user does not have an account, then prompt the user to create an account
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.LoginProvider;
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = loginInfo.Principal.FindFirstValue(ClaimTypes.Name)?.Replace(" ", "") });
            }
        }

        //
        // POST: /Account/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
            return Challenge(properties, provider);
        }

        //
        // GET: /Account/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await signInManager.GetExternalLoginInfoAsync(User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(await CurrentUserAsync(), loginInfo);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser() { UserName = model.UserName };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await SignInAsync(user, isPersistent: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> LogOff()
        {
            await signInManager.SignOutAsync();
            SocialGoalSessionFacade.Clear();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [NonAction]
        public ActionResult RemoveAccountList()
        {
            // child action: rendered synchronously from the Manage view
            var user = CurrentUserAsync().GetAwaiter().GetResult();
            var linkedAccounts = UserManager.GetLoginsAsync(user).GetAwaiter().GetResult();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        [AllowAnonymous]
        public PartialViewResult LoginPartial()
        {
            return PartialView("_LoginPartial", new LoginViewModel());
        }

        [AllowAnonymous]
        public PartialViewResult RegisterPartial()
        {
            return PartialView("_Registerpartial", new RegisterViewModel());
        }

        [NonAction]
        public IEnumerable<ApplicationUser> SearchUser(string name)
        {
            var users = userService.SearchUser(name);
            return users;
        }

        public ActionResult ImageUpload()
        {
            UploadImageViewModel imageVM = new UploadImageViewModel();
            imageVM.LocalPath = userService.GetUser(User.Identity.GetUserId()).ProfilePicUrl;
            return PartialView(imageVM);
        }

        [HttpPost]
        public async Task<ActionResult> UploadImage(UploadImageViewModel model)
        {
            //Check if all simple data annotations are valid
            if (ModelState.IsValid)
            {
                //Prepare the needed variables
                byte[] original = null;
                var errorField = string.Empty;

                if (model.IsUrl)
                {
                    errorField = "Url";
                    original = await imageFetcher.FetchAsync(model.Url);
                }
                else if (model.File != null)
                {
                    errorField = "File";
                    if (model.File.Length <= ImageFetcher.MaxBytes)
                    {
                        using var stream = new MemoryStream();
                        await model.File.CopyToAsync(stream);
                        original = stream.ToArray();
                    }
                }

                var img = ImageCropper.CropToPng(original, model.X, model.Y, model.Width, model.Height);

                //If we had success so far
                if (img != null)
                {
                    var fileName = Guid.NewGuid().ToString() + ".png";
                    var folder = Path.Combine(environment.WebRootPath, "Content", "ProfilePics");
                    var oldFile = userService.GetUser(User.Identity.GetUserId()).ProfilePicUrl;

                    //Demo purposes only - save image in the file system
                    Directory.CreateDirectory(folder);
                    await System.IO.File.WriteAllBytesAsync(Path.Combine(folder, fileName), img);
                    userService.SaveImageURL(User.Identity.GetUserId(), "~/Content/ProfilePics/" + fileName);

                    // delete only a previous upload (a GUID file name), never the shared default avatar
                    var oldName = Path.GetFileName(oldFile ?? string.Empty);
                    if (Guid.TryParse(Path.GetFileNameWithoutExtension(oldName), out _))
                    {
                        System.IO.File.Delete(Path.Combine(folder, oldName));
                    }

                    return RedirectToAction("UserProfile", new { id = User.Identity.GetUserId() });
                }
                else //Otherwise we add an error and return to the (previous) view with the model data
                    ModelState.AddModelError(errorField, Resources.UploadError);
            }

            return View("ImageUpload", model);
        }

        private IEnumerable<string> GetErrorsFromModelState()
        {
            return ModelState.SelectMany(x => x.Value.Errors.Select(error => error.ErrorMessage));
        }

        [HttpGet]
        public ViewResult UserProfile(string id)
        {
            var currentuserid = User.Identity.GetUserId();
            var user = userService.GetUserProfile(id);
            var userdetail = userProfileService.GetUser(id);
            UserProfileViewModel userprofile = new UserProfileViewModel()
            {
                FirstName = userdetail.FirstName,
                LastName = userdetail.LastName,
                Email = userdetail.Email,
                UserName = user.UserName,
                DateCreated = user.DateCreated,
                LastLoginTime = user.LastLoginTime,
                UserId = user.Id,
                ProfilePicUrl = user.ProfilePicUrl,
                DateOfBirth = userdetail.DateOfBirth,
                Gender = userdetail.Gender,
                Address = userdetail.Address,
                City = userdetail.City,
                State = userdetail.State,
                Country = userdetail.Country,
                ZipCode = userdetail.ZipCode,
                ContactNo = userdetail.ContactNo,
            };
            if (followRequestService.RequestSent((User.Identity.GetUserId()), id))
            {
                userprofile.RequestSent = true;
            }
            if (followUserService.IsFollowing(currentuserid, id))
            {
                userprofile.Following = true;
            }
            return View(userprofile);
        }


        public ActionResult EditBasicInfo()
        {
            var user = userProfileService.GetUser(User.Identity.GetUserId());
            UserProfileFormModel editUser = Mapper.Map<UserProfile, UserProfileFormModel>(user);
            if (user == null)
            {
                return NotFound();
            }
            return PartialView("EditBasicInfo", editUser);
        }

        public ActionResult EditPersonalInfo()
        {
            var user = userProfileService.GetUser(User.Identity.GetUserId());
            UserProfileFormModel editUser = Mapper.Map<UserProfile, UserProfileFormModel>(user);
            if (user == null)
            {
                return NotFound();
            }
            return PartialView("EditPersonalInfo", editUser);
        }


        [HttpPost]
        public ActionResult EditProfile(UserProfileFormModel editedProfile)
        {
            UserProfile user = Mapper.Map<UserProfileFormModel, UserProfile>(editedProfile);
            ApplicationUser applicationUser = userService.GetUser(editedProfile.UserId);
            applicationUser.FirstName = editedProfile.FirstName;
            applicationUser.LastName = editedProfile.LastName;
            applicationUser.Email = editedProfile.Email;
            if (ModelState.IsValid)
            {
                userService.UpdateUser(applicationUser);
                userProfileService.UpdateUserProfile(user);
                return RedirectToAction("UserProfile", new { id = editedProfile.UserId });
            }
            return PartialView("EditProfile", editedProfile);
        }


        public ActionResult FollowRequest(string id)
        {
            var followRequestFormModel = new FollowRequestFormModel()
            {
                FromUserId = User.Identity.GetUserId(),
                ToUserId = id,
                FromUser = userService.GetUser(User.Identity.GetUserId()),
                ToUser = userService.GetUser(id)
            };
            var followRequest = Mapper.Map<FollowRequestFormModel, FollowRequest>(followRequestFormModel);
            followRequestService.CreateFollowRequest(followRequest);
            return RedirectToAction("UserProfile", new { id = followRequestFormModel.ToUserId });
        }


        public ActionResult AcceptRequest(string touserid, string fromuserid)
        {
            var newFollowUser = new FollowUser()
            {
                Accepted = true,
                FromUserId = fromuserid,
                ToUserId = touserid,
                FromUser = userService.GetUser(fromuserid),
                ToUser = userService.GetUser(touserid)
            };
            followUserService.CreateFollowUserFromRequest(newFollowUser, followRequestService);
            return RedirectToAction("Index", "Notification");
        }

        public ActionResult RejectRequest(string toUserId, string fromuserId)
        {
            followRequestService.DeleteFollowRequest(fromuserId, toUserId);
            return RedirectToAction("Index", "Notification");
        }

        public ActionResult Unfollow(string id)
        {
            followUserService.DeleteFollowUser(id, User.Identity.GetUserId());
            return RedirectToAction("UserProfile", new { id = id });

        }


        /// <summary>
        /// Followed users by page
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult Followers(int page = 0)
        {
            var users = followUserService.GetFollowers(User.Identity.GetUserId(), page, 10);
            var followers = Mapper.Map<IEnumerable<ApplicationUser>, IEnumerable<FollowersViewModel>>(users);

            if (Request.IsAjaxRequest())
            {
                if (followers.Count() != 0)
                    return PartialView("_FollowersView", followers);
                else
                    return new EmptyResult();
            }

            return View("FollowerUsers", followers);
        }

        /// <summary>
        /// following users by page
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult Followings(int page = 0)
        {
            var users = followUserService.GetFollowings(User.Identity.GetUserId(), page, 10);
            var followings = Mapper.Map<IEnumerable<ApplicationUser>, IEnumerable<FollowingViewModel>>(users);

            if (Request.IsAjaxRequest())
            {
                if (followings.Count() != 0)
                    return PartialView("_FollowingsView", followings);
                else
                    return new EmptyResult();
            }

            return View("FollowingUsers", followings);
        }


        #region Helpers
        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            await signInManager.SignInAsync(user, isPersistent);
        }

        private Task<ApplicationUser> CurrentUserAsync() => UserManager.GetUserAsync(User);

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        private async Task<bool> HasPasswordAsync()
        {
            var user = await CurrentUserAsync();
            return user?.PasswordHash != null;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        #endregion
    }
}