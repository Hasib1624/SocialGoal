using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialGoal.Web.Helpers;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocialGoal.Web.Controllers;
using NUnit.Framework;
using SocialGoal.Model.Models;
using SocialGoal.Tests.Helpers;
using SocialGoal.Data.Repository;
using Moq;
using SocialGoal.Data.Infrastructure;
using System.Security.Principal;
using System.IO;
using SocialGoal.Service;
using SocialGoal.Web.Mailers;
using SocialGoal.Models;
using System.Linq.Expressions;
using SocialGoal.Web.ViewModels;
using System.Reflection;
using System.Collections.Specialized;
namespace SocialGoal.Web.Controllers.Test
{
    [TestFixture()]
    public class AccountControllerTest
    {
        Mock<IUserRepository> userRepository;
        Mock<IUserProfileRepository> userProfileRepository;
        Mock<IFollowRequestRepository> followRequestRepository;
        Mock<IFollowUserRepository> followUserRepository;
        Mock<ISecurityTokenRepository> securityTokenRepository;
        Mock<IUnitOfWork> unitOfWork;


        IUserService userService;
        IUserProfileService userProfileService;
        IFollowRequestService followRequestService;
        IFollowUserService followUserService;
        ISecurityTokenService securityTokenService;


        [SetUp]
        public void SetUp()
        {
            userRepository = new Mock<IUserRepository>();
            userProfileRepository = new Mock<IUserProfileRepository>();
            followRequestRepository = new Mock<IFollowRequestRepository>();
            followUserRepository = new Mock<IFollowUserRepository>();
            securityTokenRepository = new Mock<ISecurityTokenRepository>();


            unitOfWork = new Mock<IUnitOfWork>();

            userService = new UserService(userRepository.Object, unitOfWork.Object, userProfileRepository.Object);
            userProfileService = new UserProfileService(userProfileRepository.Object, unitOfWork.Object);
            followRequestService = new FollowRequestService(followRequestRepository.Object, unitOfWork.Object);
            followUserService = new FollowUserService(followUserRepository.Object, unitOfWork.Object);
            securityTokenService = new SecurityTokenService(securityTokenRepository.Object, unitOfWork.Object);




        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void SearchUser()
        {
            var userManager = Fakes.UserManager().Object;
            IEnumerable<ApplicationUser> fake = new List<ApplicationUser> 
            {
             new ApplicationUser{Activated=true,Email="user1@foo.com",FirstName="user1",LastName="user1",RoleId=0},
              new ApplicationUser{Activated=true,Email="user2@foo.com",FirstName="user2",LastName="user2",RoleId=0},
              new ApplicationUser{Activated=true,Email="user3@foo.com",FirstName="user3",LastName="user3",RoleId=0},
              new ApplicationUser{Activated=true,Email="user4@foo.com",FirstName="user4",LastName="user4",RoleId=0}
          }.AsEnumerable();
            userRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).Returns(fake);
            AccountController contr = new AccountController(userService, userProfileService, null, null, null, followRequestService, followUserService, securityTokenService, userManager, Fakes.SignInManager(userManager).Object, null, null);
            IEnumerable<ApplicationUser> result = contr.SearchUser("u") as IEnumerable<ApplicationUser>;
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(4, result.Count(), "not matching");
        }

        [Test]
        public void Image_Upload_GetView()
        {
            var userManager = Fakes.UserManager().Object;
            ApplicationUser applicationUser = getApplicationUser();
            AccountController controller = new AccountController(userService, userProfileService, null, null, null, followRequestService, followUserService, securityTokenService,userManager, Fakes.SignInManager(userManager).Object, null, null);
            controller.ControllerContext = Fakes.SignedIn();
            userRepository.Setup(x => x.Get(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).Returns(applicationUser);
            PartialViewResult result = controller.ImageUpload() as PartialViewResult;
            ClassicAssert.IsNotNull(result);
            ClassicAssert.IsInstanceOf(typeof(UploadImageViewModel), result.ViewData.Model, "Wrong model");
            var data = result.ViewData.Model as UploadImageViewModel;
            ClassicAssert.AreEqual(null, data.LocalPath, "not matching");
        }

        [Test]
        public void Upload_Image_Post()
        {
            var userManager = Fakes.UserManager().Object;
            UploadImageViewModel image = new UploadImageViewModel()
            {
                IsFile = true,
                UserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",
                LocalPath = "dddd"
            };
            AccountController controller = new AccountController(userService, userProfileService, null, null, null, followRequestService, followUserService, securityTokenService,userManager, Fakes.SignInManager(userManager).Object, null, null);
            ViewResult result = controller.UploadImage(image).Result as ViewResult;
            ClassicAssert.IsNotNull(result);
            ClassicAssert.IsInstanceOf(typeof(UploadImageViewModel), result.ViewData.Model, "WrongType");
            ClassicAssert.AreEqual("ImageUpload", result.ViewName);
        }

        [Test]
        public void UserProfile()
        {
            var userManager = Fakes.UserManager().Object;
            ApplicationUser applicationUser = getApplicationUser();
            userRepository.Setup(x => x.Get(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).Returns(applicationUser);
            AccountController controller = new AccountController(userService, userProfileService, null, null, null, followRequestService, followUserService, securityTokenService,userManager, Fakes.SignInManager(userManager).Object, null, null);
            controller.ControllerContext = Fakes.SignedIn();
            UserProfile prfil = new UserProfile()
            {
                FirstName="Adarsh",
                LastName="Vikraman",
                DateOfBirth = DateTime.Now,
                Gender = true,
                Address = "a",
                City = "a",
                State = "a",
                Country = "a",
                ZipCode = 2344545,
                ContactNo = 1223344556,
                UserId = "402bd590-fdc7-49ad-9728-40efbfe512ec"
            };
            userProfileRepository.Setup(x => x.Get(It.IsAny<Expression<Func<UserProfile, bool>>>())).Returns(prfil);
            IEnumerable<FollowRequest> fake = new List<FollowRequest> {
            new FollowRequest { FollowRequestId =1, FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec", ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed"},
            new FollowRequest { FollowRequestId =2, FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec", ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ee"},
            };
            followRequestRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<FollowRequest, bool>>>())).Returns(fake);
            IEnumerable<FollowUser> fakeuser = new List<FollowUser> {
            new FollowUser {FollowUserId =1, Accepted = false,FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec", ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed"},
            new FollowUser {FollowUserId =2, Accepted = false,FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec", ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ee" },           
            };
            followUserRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<FollowUser, bool>>>())).Returns(fakeuser);
            ViewResult result = controller.UserProfile("402bd590-fdc7-49ad-9728-40efbfe512ec") as ViewResult;
            ClassicAssert.IsNotNull(result);
            ClassicAssert.IsInstanceOf(typeof(UserProfileViewModel), result.ViewData.Model, "WrongType");
            var data = result.ViewData.Model as UserProfileViewModel;
            ClassicAssert.AreEqual("adarsh", data.UserName);
        }

        [Test]
        public void Edit_Basic_Info()
        {
            var userManager = Fakes.UserManager().Object;
            ApplicationUser applicationUser = getApplicationUser();
            AccountController controller = new AccountController(userService, userProfileService, null, null, null, followRequestService, followUserService, securityTokenService,userManager, Fakes.SignInManager(userManager).Object, null, null);
            controller.ControllerContext = Fakes.SignedIn();
            UserProfile userProfile = new UserProfile()
            {
                UserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",
                FirstName = "Adarsh",
                LastName = "Vikraman",
                Email = "adarsh@foo.com",
            };
            userProfileRepository.Setup(x => x.Get(It.IsAny<Expression<Func<UserProfile, bool>>>())).Returns(userProfile);
            PartialViewResult result = controller.EditBasicInfo() as PartialViewResult;
            ClassicAssert.IsNotNull(result);
            ClassicAssert.IsInstanceOf(typeof(UserProfileFormModel), result.ViewData.Model, "WrongType");
            var data = result.ViewData.Model as UserProfileFormModel;
            ClassicAssert.AreEqual("adarsh@foo.com", data.Email);
        }

        [Test]
        public void Edit_Personal_Info()
        {
            var userManager = Fakes.UserManager().Object;
            ApplicationUser applicationUser = getApplicationUser();
            AccountController controller = new AccountController(userService, userProfileService, null, null, null, followRequestService, followUserService, securityTokenService,userManager, Fakes.SignInManager(userManager).Object, null, null);
            controller.ControllerContext = Fakes.SignedIn();
            UserProfile grpuser = new UserProfile()
            {
                UserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",
                Address = "t",
                City = "t",
                State = "adarsh@foo.com",
            };
            userProfileRepository.Setup(x => x.Get(It.IsAny<Expression<Func<UserProfile, bool>>>())).Returns(grpuser);

            PartialViewResult result = controller.EditPersonalInfo() as PartialViewResult;
            ClassicAssert.IsNotNull(result);
            ClassicAssert.IsInstanceOf(typeof(UserProfileFormModel), result.ViewData.Model, "WrongType");
            var data = result.ViewData.Model as UserProfileFormModel;
            ClassicAssert.AreEqual("t", data.Address);
        }

        [Test]
        public void Editprofile_Post()
        {
            var userManager = Fakes.UserManager().Object;
            ApplicationUser applicationUser = getApplicationUser();
            userRepository.Setup(x => x.Get(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).Returns(applicationUser);
            UserProfileFormModel profile = new UserProfileFormModel();

            profile.FirstName ="adarsh";

            AccountController contr = new AccountController(userService, userProfileService, null, null, null, followRequestService, followUserService, securityTokenService,userManager, Fakes.SignInManager(userManager).Object, null, null);
            var result = contr.EditProfile(profile) as RedirectToActionResult;

            ClassicAssert.AreEqual("UserProfile", result.ActionName);
        }

        [Test]
        public void Follow_Request()
        {
            var userManager = Fakes.UserManager().Object;
            ApplicationUser applicationUser = getApplicationUser();



            AccountController controller = new AccountController(userService, userProfileService, null, null, null, followRequestService, followUserService, securityTokenService,userManager, Fakes.SignInManager(userManager).Object, null, null);
            controller.ControllerContext = Fakes.SignedIn();










            userRepository.Setup(x => x.Get(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).Returns(applicationUser);


            var result = controller.FollowRequest("402bd590-fdc7-49ad-9728-40efbfe512ec") as RedirectToActionResult;
            ClassicAssert.AreEqual("UserProfile", result.ActionName);
        }

        [Test]
        public void Accept_Request()
        {
            var userManager = Fakes.UserManager().Object;
            ApplicationUser applicationUser = getApplicationUser();
            userRepository.Setup(x => x.Get(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).Returns(applicationUser);
            AccountController contr = new AccountController(userService, userProfileService, null, null, null, followRequestService, followUserService, securityTokenService,userManager, Fakes.SignInManager(userManager).Object, null, null);
            var result = contr.AcceptRequest("402bd590-fdc7-49ad-9728-40efbfe512ed","402bd590-fdc7-49ad-9728-40efbfe512ec") as RedirectToActionResult;
            ClassicAssert.AreEqual("Index", result.ActionName);

        }

        [Test]
        public void Delete_Follow_Request()
        {
            var userManager = Fakes.UserManager().Object;
            FollowRequest request = new FollowRequest()
            {
                FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",
                ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed",

            };
            followRequestRepository.Setup(x => x.Get(It.IsAny<Expression<Func<FollowRequest, bool>>>())).Returns(request);
            AccountController contr = new AccountController(userService, userProfileService, null, null, null, followRequestService, followUserService, securityTokenService,userManager, Fakes.SignInManager(userManager).Object, null, null);
            var result = contr.RejectRequest("402bd590-fdc7-49ad-9728-40efbfe512ed","402bd590-fdc7-49ad-9728-40efbfe512ec") as RedirectToActionResult;
            ClassicAssert.AreEqual("Index", result.ActionName);
        }
        [Test]
        public void UnFollow()
        {
            var userManager = Fakes.UserManager().Object;
            ApplicationUser applicationUser = getApplicationUser();



            AccountController controller = new AccountController(userService, userProfileService, null, null, null, followRequestService, followUserService, securityTokenService,userManager, Fakes.SignInManager(userManager).Object, null, null);

            controller.ControllerContext = Fakes.SignedIn();









            FollowUser flwuser = new FollowUser()
            {
                FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",
                ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed",

            };
            followUserRepository.Setup(x => x.Get(It.IsAny<Expression<Func<FollowUser, bool>>>())).Returns(flwuser);

            var result = controller.Unfollow("402bd590-fdc7-49ad-9728-40efbfe512ed") as RedirectToActionResult;
            ClassicAssert.AreEqual("UserProfile", result.ActionName);
        }

        [Test]
        public void Followers_List()
        {
            var userManager = Fakes.UserManager().Object;
            ApplicationUser applicationUser = getApplicationUser();



            AccountController controller = new AccountController(userService, userProfileService, null, null, null, followRequestService, followUserService, securityTokenService,userManager, Fakes.SignInManager(userManager).Object, null, null);

            controller.ControllerContext = Fakes.SignedIn();









            IEnumerable<FollowUser> fakeuser = new List<FollowUser> {
            new FollowUser {FollowUserId =1, Accepted = false,FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed", ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",},
            new FollowUser {FollowUserId =2, Accepted = false,FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ee", ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec" },

            };
            followUserRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<FollowUser, bool>>>())).Returns(fakeuser);



            ViewResult result = controller.Followers() as ViewResult;
            ClassicAssert.IsNotNull(result);
            ClassicAssert.IsInstanceOf(typeof(IEnumerable<FollowersViewModel>), result.ViewData.Model, "WrongType");
            var data = result.ViewData.Model as IEnumerable<FollowersViewModel>;
            ClassicAssert.AreEqual(2, data.Count());
        }


        [Test]
        public void Followings_list()
        {
            var userManager = Fakes.UserManager().Object;
            ApplicationUser applicationUser = getApplicationUser();



            AccountController controller = new AccountController(userService, userProfileService, null, null, null, followRequestService, followUserService, securityTokenService,userManager, Fakes.SignInManager(userManager).Object, null, null);

            controller.ControllerContext = Fakes.SignedIn();









            IEnumerable<FollowUser> fakeuser = new List<FollowUser> {
            new FollowUser {FollowUserId =1, Accepted = false,FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec", ToUserId ="402bd590-fdc7-49ad-9728-40efbfe512ed",},
            new FollowUser {FollowUserId =2, Accepted = false,FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec", ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ee" },

            };
            followUserRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<FollowUser, bool>>>())).Returns(fakeuser);



            ViewResult result = controller.Followings() as ViewResult;
            ClassicAssert.IsNotNull(result);
            ClassicAssert.IsInstanceOf(typeof(IEnumerable<FollowingViewModel>), result.ViewData.Model, "WrongType");
            var data = result.ViewData.Model as IEnumerable<FollowingViewModel>;
            ClassicAssert.AreEqual(2, data.Count());
        }

        [Test]
        public void Login_Get_View_If_Guid_Is_Null()
        {
            var userManager = Fakes.UserManager().Object;
            Guid goalIdToken = Guid.NewGuid();
            AccountController controller = new AccountController(userService, userProfileService, null, null, null, followRequestService, followUserService, securityTokenService, userManager, Fakes.SignInManager(userManager).Object, null, null);
            controller.ControllerContext = Fakes.SignedIn();
            ViewResult rslt = controller.Login("abcd") as ViewResult;
            ClassicAssert.IsNotNull(rslt);

        }

        [Test]
        public void Login_Get_View_If_Guid_Is_NotNull()
        {
            var userManager = Fakes.UserManager().Object;
            //mocking QueryString
            var querystring = new NameValueCollection { { "guid", "got_value" } };
            var querystring1 = new NameValueCollection { { "reg", "value" } };
            Guid goalIdToken = Guid.NewGuid();
            // Guid 
            //controllerContext.SetupGet(p => p.HttpContext.Request.QueryString).Returns(querystring1);
            AccountController controller = new AccountController(userService, userProfileService, null, null, null, followRequestService, followUserService, securityTokenService, userManager, Fakes.SignInManager(userManager).Object, null, null);
            controller.ControllerContext = Fakes.SignedIn();

            SocialGoalSessionFacade.Accessor = Fakes.Accessor();

            ViewResult rslt = controller.Login("abcd") as ViewResult;
            ClassicAssert.IsNotNull(rslt);
        }

        [Test]
        public void LoginTest()
        {
            ApplicationUser applicationUser = getApplicationUser();
            var userManager = Fakes.UserManager();
            userManager.Setup(m => m.FindByNameAsync("adarsh")).ReturnsAsync(applicationUser);
            userManager.Setup(m => m.CheckPasswordAsync(applicationUser, "123456")).ReturnsAsync(true);
            var signInManager = Fakes.SignInManager(userManager.Object);
            AccountController controller = new AccountController(userService, userProfileService, null, null, null, followRequestService, followUserService, securityTokenService, userManager.Object, signInManager.Object, null, null);
            controller.ControllerContext = Fakes.SignedIn(Fakes.HttpWithAuthentication());
            controller.Url = Mock.Of<IUrlHelper>();

            var result = controller.Login(new LoginViewModel { Email = "adarsh", Password = "123456", RememberMe = false }, "abcd").Result as RedirectToActionResult;

            ClassicAssert.AreEqual("Index", result.ActionName);
            signInManager.Verify(m => m.SignInAsync(applicationUser, false, null), Times.Once());
        }

        [Test]
        public void LogOff()
        {
            var userManager = Fakes.UserManager().Object;
            AccountController controller = new AccountController(userService, userProfileService, null, null, null, followRequestService, followUserService, securityTokenService, userManager, Fakes.SignInManager(userManager).Object, null, null);
            SocialGoalSessionFacade.Accessor = Fakes.Accessor();

            controller.ControllerContext = Fakes.SignedIn();
            var result = controller.LogOff().Result as RedirectToActionResult;
            ClassicAssert.AreEqual("Index", result.ActionName);
        }

        [Test]
        public void Register_Get_Returns_View()
        {
            var userManager = Fakes.UserManager().Object;
            AccountController controller = new AccountController(userService, userProfileService, null, null, null, followRequestService, followUserService, securityTokenService, userManager, Fakes.SignInManager(userManager).Object, null, null);
            ViewResult rslt = controller.Register() as ViewResult;
            ClassicAssert.IsNotNull(rslt);
        }

        [Test]
        public void RegisterTest()
        {
            ApplicationUser created = null;
            var userManager = Fakes.UserManager();
            userManager.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), "123456"))
                .Callback<ApplicationUser, string>((user, password) => created = user)
                .ReturnsAsync(IdentityResult.Success);
            var signInManager = Fakes.SignInManager(userManager.Object);
            AccountController controller = new AccountController(userService, userProfileService, null, null, null, followRequestService, followUserService, securityTokenService, userManager.Object, signInManager.Object, null, null);
            controller.ControllerContext = Fakes.SignedIn(Fakes.HttpWithAuthentication());
            controller.Url = Mock.Of<IUrlHelper>();
            SocialGoalSessionFacade.Accessor = Fakes.Accessor();

            var result = controller.Register(new RegisterViewModel
            {
                UserName = "adarsh",
                Password = "123456",
                ConfirmPassword = "123456"
            }).Result as RedirectToActionResult;

            ClassicAssert.AreEqual("Index", result.ActionName);
            ClassicAssert.AreEqual("adarsh", created.UserName);
            signInManager.Verify(m => m.SignInAsync(created, false, null), Times.Once());
        }

         public ApplicationUser getApplicationUser()
        {
          ApplicationUser applicationUser = new ApplicationUser()
            {
                Activated = true,
                Email = "adarsh@foo.com",
                FirstName = "Adarsh",
                LastName = "Vikraman",
                UserName = "adarsh",
                RoleId = 0,
                Id = "402bd590-fdc7-49ad-9728-40efbfe512ec",
                DateCreated = DateTime.Now,
                LastLoginTime = DateTime.Now,
                ProfilePicUrl = null,
            };
          return applicationUser;
        }
    }
}
