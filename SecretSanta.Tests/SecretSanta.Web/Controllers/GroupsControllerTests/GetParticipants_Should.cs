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
using SecretSanta.Web.Models.Users;

namespace SecretSanta.Tests.SecretSanta.Web.Controllers.GroupsControllerTests
{
    [TestFixture]
    public class GetParticipants_Should
    {
        private Mock<IAuthenticationProvider> authenticationProviderMock;
        private Mock<IGroupService> groupServiceMock;
        private Mock<IUserService> userServiceMock;
        private Mock<IMapper> mapperMock;
        private GroupsController controller;
        private Group group;
        private IList<DisplayUserViewModel> resultViewModels;
        private const string GROUP_NAME = "Sample group";
        private const string USER_NAME = "admin";

        [Test]
        public void ReturnBadRequest_WhenGroupnameIsNull()
        {
            var result = controller.GetParticipants(null);
            var resultContent = (result as BadRequestErrorMessageResult).Message;

            Assert.IsInstanceOf<BadRequestErrorMessageResult>(result);
            Assert.AreEqual(Constants.INVALID_MODEL, resultContent);
        }

        [Test]
        public void CallGroupServiceGetGroupByName_WhenInvoked()
        {
            controller.GetParticipants(GROUP_NAME);

            groupServiceMock.Verify(s => s.GetGroupByName(GROUP_NAME), Times.Once);
        }

        [Test]
        public void CallAuthProviderCurrentUserName_WhenGroupIsFound()
        {
            controller.GetParticipants(GROUP_NAME);

            authenticationProviderMock.Verify(a => a.CurrentUserName, Times.Once);
        }

        [Test]
        public void CallCheckUserAccessRights_WhenGroupIsFound()
        {
            controller.GetParticipants(GROUP_NAME);

            groupServiceMock.Verify(s => s.CheckUserAcccessRights(USER_NAME, USER_NAME), Times.Once);
        }

        [Test]
        public void CallMaperMapUsers_WhenUserHasAccessRights()
        {
            controller.GetParticipants(GROUP_NAME);

            mapperMock.Verify(m => m.MapUsers(this.group.Users), Times.Once);
        }

        [Test]
        public void ReturnOk_WhenParticipantsAreFound()
        {
            var response = controller.GetParticipants(GROUP_NAME);
            var responseContent = response as NegotiatedContentResult<IEnumerable<DisplayUserViewModel>>;

            Assert.IsInstanceOf<NegotiatedContentResult<IEnumerable<DisplayUserViewModel>>>(response);
            Assert.AreEqual(HttpStatusCode.OK, responseContent.StatusCode);
            Assert.AreEqual(this.resultViewModels, responseContent.Content);
        }

        [Test]
        public void ReturnNotFound_WhenItemNotFoundExceptionIsThrown()
        {
            var response = controller.GetParticipants("Non existing group");
            var responseContent = response as NegotiatedContentResult<string>;

            Assert.IsInstanceOf<NegotiatedContentResult<string>>(response);
            Assert.AreEqual(HttpStatusCode.NotFound, responseContent.StatusCode);
        }

        [Test]
        public void ReturnForbidden_WhenAccessForbiddenExceptionIsThrown()
        {
            var response = controller.GetParticipants("Forbidden group");
            var responseContent = response as NegotiatedContentResult<string>;

            Assert.IsInstanceOf<NegotiatedContentResult<string>>(response);
            Assert.AreEqual(HttpStatusCode.Forbidden, responseContent.StatusCode);
        }

        [Test]
        public void ReturnInternalServerError_WhenExceptionIsThrown()
        {
            this.mapperMock.Setup(m => m.MapUsers(this.group.Users)).Throws<Exception>();

            var response = controller.GetParticipants(GROUP_NAME);
            var responseContent = response as NegotiatedContentResult<string>;

            Assert.IsInstanceOf<NegotiatedContentResult<string>>(response);
            Assert.AreEqual(HttpStatusCode.InternalServerError, responseContent.StatusCode);
        }

        [SetUp]
        public void SetUp()
        {
            this.group = new Group()
            {
                Name = GROUP_NAME,
                Admin = new User() {  UserName = USER_NAME },
                Users = new List<User>()
                {
                    new User() {  UserName = USER_NAME },
                    new User() {  UserName = "jack" },
                }
            };
            this.resultViewModels = new List<DisplayUserViewModel>()
            {
                new DisplayUserViewModel() {UserName = USER_NAME},
                new DisplayUserViewModel() {UserName = "jack"}
            };

            this.authenticationProviderMock = new Mock<IAuthenticationProvider>();
            this.authenticationProviderMock.Setup(a => a.CurrentUserName).Returns(USER_NAME);
            this.groupServiceMock = new Mock<IGroupService>();
            this.groupServiceMock.Setup(s => s.GetGroupByName(GROUP_NAME)).Returns(this.group);
            this.groupServiceMock.Setup(s => s.GetGroupByName("Non existing group")).Throws<ItemNotFoundException>();
            this.groupServiceMock.Setup(s => s.GetGroupByName("Forbidden group"))
                .Returns(new Group() { Admin = new User() { UserName = "joe"}});
            this.groupServiceMock.Setup(s => s.CheckUserAcccessRights(USER_NAME, It.Is<string>(n => !n.Equals(USER_NAME))))
                .Throws<AccessForbiddenException>();
            this.userServiceMock = new Mock<IUserService>();
            this.mapperMock = new Mock<IMapper>();
            this.mapperMock.Setup(m => m.MapUsers(this.group.Users)).Returns(this.resultViewModels);

            this.controller = new GroupsController(authenticationProviderMock.Object,
                groupServiceMock.Object, userServiceMock.Object, mapperMock.Object);
        }
    }
}
