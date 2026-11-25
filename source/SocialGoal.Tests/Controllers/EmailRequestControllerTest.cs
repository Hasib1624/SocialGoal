using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialGoal.Web.Helpers;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocialGoal.Data.Repository;
using SocialGoal.Data.Infrastructure;
using SocialGoal.Model.Models;
using SocialGoal.Service;
using SocialGoal.Web.ViewModels;
using SocialGoal.Web.Core.Extensions;
using SocialGoal.Web.Controllers;
using NUnit.Framework;
using Moq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.IO;
using System.Reflection;
using SocialGoal.Tests.Helpers;


namespace SocialGoal.Tests.Controllers
{
    [TestFixture]
    public class EmailRequestControllerTest
    {
        Mock<ISecurityTokenRepository> securityTokenRepository;
        Mock<IGroupUserRepository> groupUserRepository;
        Mock<ISupportRepository> supportRepository;
        Mock<IGroupInvitationRepository> groupInvitationRepository;
        Mock<IUserRepository> userRepository;
        Mock<IFollowUserRepository> followUserRepository;


        Mock<IUnitOfWork> unitOfWork;


        public ISecurityTokenService securityTokenService;
        public IGroupUserService groupUserService;
        public ISupportService supportService;
        public IGroupInvitationService groupInvitationService;
        public IUserService userService;


        [SetUp]
        public void SetUp()
        {
            securityTokenRepository = new Mock<ISecurityTokenRepository>();
            supportRepository = new Mock<ISupportRepository>();
            groupInvitationRepository = new Mock<IGroupInvitationRepository>();
            groupUserRepository = new Mock<IGroupUserRepository>();
            userRepository = new Mock<IUserRepository>();
            followUserRepository = new Mock<IFollowUserRepository>();
            //userProfileRepository=new Mock<UserProfileRepository>();

            unitOfWork = new Mock<IUnitOfWork>();


            securityTokenService = new SecurityTokenService(securityTokenRepository.Object, unitOfWork.Object);
            groupInvitationService = new GroupInvitationService(groupInvitationRepository.Object, unitOfWork.Object);
            groupUserService = new GroupUserService(groupUserRepository.Object, userRepository.Object, unitOfWork.Object);
            supportService = new SupportService(supportRepository.Object, followUserRepository.Object, unitOfWork.Object);
            // userService = new UserService(userRepository.Object, unitOfWork.Object, userProfileRepository.Object);
        }
        [TearDown]
        public void TearDown()
        {

        }

        [Test]
        public void Add_GroupUser()
        {
            Guid guidToken = Guid.NewGuid();
            SecurityToken token = new SecurityToken()
            {
                SecurityTokenId = 1,
                Token = guidToken,
                ActualID = 1
            };

            securityTokenRepository.Setup(x => x.Get(It.IsAny<Expression<Func<SecurityToken, bool>>>())).Returns(token);

            ApplicationUser applicationUser = getApplicationUser();



            EmailRequestController controller = new EmailRequestController(securityTokenService, groupUserService, supportService, groupInvitationService);

            controller.ControllerContext = Fakes.SignedIn();









            SocialGoalSessionFacade.Accessor = Fakes.Accessor();
            //HttpContext.Current.Request.Session["somevalue"];

            controller.TempData = Fakes.TempData();
            controller.TempData["grToken"] = guidToken;
            var result = controller.AddGroupUser() as RedirectToActionResult;
            ClassicAssert.AreEqual("Index", result.ActionName);
        }

        [Test]
        public void Add_Support_ToGoal()
        {

            Guid guidToken = Guid.NewGuid();
            SecurityToken token = new SecurityToken()
            {
                SecurityTokenId = 1,
                Token = guidToken,
                ActualID = 1
            };

            securityTokenRepository.Setup(x => x.Get(It.IsAny<Expression<Func<SecurityToken, bool>>>())).Returns(token);

            ApplicationUser applicationUser = getApplicationUser();



            EmailRequestController controller = new EmailRequestController(securityTokenService, groupUserService, supportService, groupInvitationService);

            controller.ControllerContext = Fakes.SignedIn();









            SocialGoalSessionFacade.Accessor = Fakes.Accessor();

            controller.TempData = Fakes.TempData();
            controller.TempData["goToken"] = guidToken;
            var result = controller.AddSupportToGoal() as RedirectToActionResult;
            ClassicAssert.AreEqual("Index", result.ActionName);
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
