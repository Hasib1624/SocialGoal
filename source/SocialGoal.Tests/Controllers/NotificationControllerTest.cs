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
using SocialGoal.Tests.Helpers;


namespace SocialGoal.Tests.Controllers
{
    [TestFixture]
    public class NotificationControllerTest
    {

        Mock<IGroupInvitationRepository> groupInvitationRepository;
        Mock<ISupportInvitationRepository> supportInvitationRepository;
        Mock<IFollowRequestRepository> followRequestRepository;
        Mock<IUserRepository> userRepository;
        Mock<IUserProfileRepository> userProfileRepository;

        Mock<IUnitOfWork> unitOfWork;

        IGoalService goalService;
        IUpdateService updateService;
        ICommentService commentService;
        IGroupInvitationService groupInvitationService;
        ISupportInvitationService supportInvitationService;
        IFollowRequestService followRequestService;
        IUserService userService;


        [SetUp]
        public void SetUp()
        {
            groupInvitationRepository = new Mock<IGroupInvitationRepository>();
            supportInvitationRepository = new Mock<ISupportInvitationRepository>();
            followRequestRepository = new Mock<IFollowRequestRepository>();
            userRepository = new Mock<IUserRepository>();
            userProfileRepository = new Mock<IUserProfileRepository>();


            unitOfWork = new Mock<IUnitOfWork>();


            groupInvitationService = new GroupInvitationService(groupInvitationRepository.Object, unitOfWork.Object);
            supportInvitationService = new SupportInvitationService(supportInvitationRepository.Object, unitOfWork.Object);
            followRequestService = new FollowRequestService(followRequestRepository.Object, unitOfWork.Object);
            userService = new UserService(userRepository.Object, unitOfWork.Object, userProfileRepository.Object);
        }

        [TearDown]
        public void TearDown()
        {
        }
        [Test]
        public void Index_If_AjaxRequest_IsNull()
        {
            ApplicationUser applicationUser = getApplicationUser();

            userRepository.Setup(x => x.Get(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).Returns(applicationUser);

            NotificationController controller = new NotificationController(goalService, updateService, commentService, groupInvitationService, supportInvitationService, followRequestService, userService);


            controller.ControllerContext = Fakes.SignedIn();









            IEnumerable<GroupInvitation> groupInvitation = new List<GroupInvitation> {
            new GroupInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed", FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec"},
            new GroupInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed",},
            new GroupInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ee"},
           
           
          }.AsEnumerable();

            groupInvitationRepository.Setup(x => x.GetAll()).Returns(groupInvitation);

            IEnumerable<SupportInvitation> supportInvitation = new List<SupportInvitation> {
            new SupportInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec"},
            new SupportInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId ="402bd590-fdc7-49ad-9728-40efbfe512ed"},
            new SupportInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId ="402bd590-fdc7-49ad-9728-40efbfe512ee"},
           
           
          }.AsEnumerable();

            supportInvitationRepository.Setup(x => x.GetAll()).Returns(supportInvitation);

            IEnumerable<FollowRequest> followInvitation = new List<FollowRequest> {
            new FollowRequest{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec"},
            new FollowRequest{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId ="402bd590-fdc7-49ad-9728-40efbfe512ed"},
            new FollowRequest{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ee"},
           
           
          }.AsEnumerable();

            followRequestRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<FollowRequest, bool>>>())).Returns(followInvitation);
            ViewResult result = controller.Index(1) as ViewResult;
            ClassicAssert.IsNotNull(result);
        }

        [Test]
        public void Index_If_AjaxRequest_Is_NotNull()
        {
            ApplicationUser applicationUser = getApplicationUser();

            userRepository.Setup(x => x.Get(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).Returns(applicationUser);

            NotificationController controller = new NotificationController(goalService, updateService, commentService, groupInvitationService, supportInvitationService, followRequestService, userService);


            controller.ControllerContext = Fakes.SignedIn();
            controller.Request.Headers["X-Requested-With"] = "XMLHttpRequest";










            IEnumerable<GroupInvitation> groupInvitation = new List<GroupInvitation> {
            new GroupInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed", FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec"},
            new GroupInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed",},
            new GroupInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ee"},
           
           
          }.AsEnumerable();

            groupInvitationRepository.Setup(x => x.GetAll()).Returns(groupInvitation);

            IEnumerable<SupportInvitation> supportInvitation = new List<SupportInvitation> {
            new SupportInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec"},
            new SupportInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId ="402bd590-fdc7-49ad-9728-40efbfe512ed"},
            new SupportInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ee"},
           
           
          }.AsEnumerable();

            supportInvitationRepository.Setup(x => x.GetAll()).Returns(supportInvitation);

            IEnumerable<FollowRequest> followInvitation = new List<FollowRequest> {
            new FollowRequest{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec"},
            new FollowRequest{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId ="402bd590-fdc7-49ad-9728-40efbfe512ed"},
            new FollowRequest{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ee"},
           
           
          }.AsEnumerable();

            followRequestRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<FollowRequest, bool>>>())).Returns(followInvitation);
            PartialViewResult result = controller.Index(1) as PartialViewResult;
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual("_NotificationList", result.ViewName);
        }

        [Test]
        public void Get_All_Notifications()
        {
            ApplicationUser applicationUser = getApplicationUser();

            userRepository.Setup(x => x.Get(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).Returns(applicationUser);

            NotificationController controller = new NotificationController(goalService, updateService, commentService, groupInvitationService, supportInvitationService, followRequestService, userService);


            controller.ControllerContext = Fakes.SignedIn();









            IEnumerable<GroupInvitation> groupInvitation = new List<GroupInvitation> {
            new GroupInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed", FromUserId ="402bd590-fdc7-49ad-9728-40efbfe512ec"},
            new GroupInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed",},
            new GroupInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ee"},
           
           
          }.AsEnumerable();

            groupInvitationRepository.Setup(x => x.GetAll()).Returns(groupInvitation);

            IEnumerable<SupportInvitation> supportInvitation = new List<SupportInvitation> {
            new SupportInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec"},
            new SupportInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId ="402bd590-fdc7-49ad-9728-40efbfe512ed"},
            new SupportInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ee"},
           
           
          }.AsEnumerable();

            supportInvitationRepository.Setup(x => x.GetAll()).Returns(supportInvitation);

            IEnumerable<FollowRequest> followInvitation = new List<FollowRequest> {
            new FollowRequest{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec"},
            new FollowRequest{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId ="402bd590-fdc7-49ad-9728-40efbfe512ed"},
            new FollowRequest{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ee"},
           
           
          }.AsEnumerable();

            followRequestRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<FollowRequest, bool>>>())).Returns(followInvitation);
            IEnumerable<NotificationViewModel> result = controller.GetNotifications(5, 5) as IEnumerable<NotificationViewModel>;
            ClassicAssert.IsNotNull(result);
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
