using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http.Results;
using Moq;
using NUnit.Framework;
using SecretSanta.Authentication.Contracts;
using SecretSanta.Common;
using SecretSanta.Common.Exceptions;
using SecretSanta.Models;
using SecretSanta.Services.Contracts;
using SecretSanta.Web.Controllers;
using SecretSanta.Web.Mapper;

namespace SecretSanta.Tests.SecretSanta.Web.Controllers.GroupsControllerTests
{
    [TestFixture]
    public class RemoveParticipant_Should
    {
        private Mock<IAuthenticationProvider> authenticationProviderMock;
        private Mock<IGroupService> groupServiceMock;
        private Mock<IUserService> userServiceMock;
        private Mock<IMapper> mapperMock;
        private GroupsController controller;
        private Group group;
        private User user;
        private const string ADMIN_NAME = "jack";
        private const string USER_NAME = "joe";
        private const string GROUP_NAME = "Sample group";

        [Test]
        public void ReturnBadRequest_WhenGroupnameIsNull()
        {
            var result = controller.RemoveParticipant(null, GROUP_NAME);
            var resultContent = (result as BadRequestErrorMessageResult).Message;

            Assert.IsInstanceOf<BadRequestErrorMessageResult>(result);
            Assert.AreEqual(Constants.INVALID_MODEL, resultContent);
        }

        [Test]
        public void CallServiceGetGroupByName_WhenInvoked()
        {
            controller.RemoveParticipant(GROUP_NAME, USER_NAME);

            groupServiceMock.Verify(s => s.GetGroupByName(GROUP_NAME), Times.Once);
        }

        [Test]
        public void CallAuthProviderCurrentUserName_WhenGroupIsFound()
        {
            controller.RemoveParticipant(GROUP_NAME, USER_NAME);

            authenticationProviderMock.Verify(a => a.CurrentUserName, Times.Once);
        }

        [Test]
        public void CallCheckUserRights_WhenGroupIsFound()
        {
            controller.RemoveParticipant(GROUP_NAME, USER_NAME);

            groupServiceMock.Verify(s => s.CheckUserAcccessRights(ADMIN_NAME, ADMIN_NAME), Times.Once);
        }

        [Test]
        public void CallGetUserByUsername_WhenUserHasRightAccess()
        {
            controller.RemoveParticipant(GROUP_NAME, USER_NAME);

            userServiceMock.Verify(s => s.GetUserByUserName(USER_NAME), Times.Once);
        }

        [Test]
        public void CallRemoveParticipant_WhenUserIsFound()
        {
            controller.RemoveParticipant(GROUP_NAME, USER_NAME);

            groupServiceMock.Verify(s => s.RemoveParticipant(this.group, this.user), Times.Once);
        }

        [Test]
        public void ReturnNoContent_WhenParticipantIsRemoved()
        {
            var result = controller.RemoveParticipant(GROUP_NAME, USER_NAME);
            var resultStatusCode = (result as StatusCodeResult).StatusCode;

            Assert.IsInstanceOf<StatusCodeResult>(result);
            Assert.AreEqual(HttpStatusCode.NoContent, resultStatusCode);
        }

        [Test]
        public void ReturnNotFound_WhenItemNotFoundExceptionIsThrown()
        {
            var response = controller.RemoveParticipant(GROUP_NAME, "Non existing user");
            var responseContent = response as NegotiatedContentResult<string>;

            Assert.IsInstanceOf<NegotiatedContentResult<string>>(response);
            Assert.AreEqual(HttpStatusCode.NotFound, responseContent.StatusCode);
        }

        [Test]
        public void ReturnForbidden_WhenAccessForbiddenExceptionIsThrown()
        {
            var response = controller.RemoveParticipant("Forbidden group", USER_NAME);
            var responseContent = response as NegotiatedContentResult<string>;

            Assert.IsInstanceOf<NegotiatedContentResult<string>>(response);
            Assert.AreEqual(HttpStatusCode.Forbidden, responseContent.StatusCode);
        }

        [Test]
        public void ReturnInternalServerError_WhenExceptionIsThrown()
        {
            this.groupServiceMock.Setup(s => s.RemoveParticipant(this.group, this.user)).Throws<Exception>();

            var response = controller.RemoveParticipant(GROUP_NAME, USER_NAME);
            var responseContent = response as NegotiatedContentResult<string>;

            Assert.IsInstanceOf<NegotiatedContentResult<string>>(response);
            Assert.AreEqual(HttpStatusCode.InternalServerError, responseContent.StatusCode);
        }

        [SetUp]
        public void SetUp()
        {
            this.user = new User() { UserName = USER_NAME };
            this.group = new Group()
            {
                Name = GROUP_NAME,
                Admin = new User() { UserName = ADMIN_NAME },
                Users = new List<User>()
                {
                    this.user
                }
            };

            this.authenticationProviderMock = new Mock<IAuthenticationProvider>();
            this.authenticationProviderMock.Setup(a => a.CurrentUserName).Returns(ADMIN_NAME);
            this.groupServiceMock = new Mock<IGroupService>();
            this.groupServiceMock.Setup(s => s.GetGroupByName(GROUP_NAME)).Returns(this.group);
            this.groupServiceMock.Setup(s => s.GetGroupByName("Forbidden group"))
                .Returns(new Group() { Admin = new User() { UserName = USER_NAME }});
            this.groupServiceMock.Setup(s => s.CheckUserAcccessRights(ADMIN_NAME, It.Is<string>(n => !n.Equals(ADMIN_NAME))))
                .Throws<AccessForbiddenException>();
            this.userServiceMock = new Mock<IUserService>();
            this.userServiceMock.Setup(s => s.GetUserByUserName(USER_NAME)).Returns(this.user);
            this.userServiceMock.Setup(s => s.GetUserByUserName("Non existing user")).Throws<ItemNotFoundException>();
            this.mapperMock = new Mock<IMapper>();

            this.controller = new GroupsController(authenticationProviderMock.Object,
                groupServiceMock.Object, userServiceMock.Object, mapperMock.Object);
        }
    }
}
