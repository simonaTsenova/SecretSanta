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
using SecretSanta.Web.Models;
using SecretSanta.Web.Models.Groups;

namespace SecretSanta.Tests.SecretSanta.Web.Controllers.GroupsControllerTests
{
    [TestFixture]
    public class GetUserGroups_Should
    {
        private Mock<IAuthenticationProvider> authenticationProviderMock;
        private Mock<IGroupService> groupServiceMock;
        private Mock<IUserService> userServiceMock;
        private Mock<IMapper> mapperMock;
        private GroupsController controller;
        private PagingViewModel viewModel;
        private IList<Group> groups;
        private IList<ShortGroupViewModel> resultViewModels;
        private const string USER_NAME = "user";

        [Test]
        public void ReturnBadRequest_WhenModelIsNull()
        {
            var response = controller.GetUserGroups(USER_NAME, null);
            var responseContent = (response as BadRequestErrorMessageResult).Message;

            Assert.IsInstanceOf<BadRequestErrorMessageResult>(response);
            Assert.AreEqual(Constants.INVALID_MODEL, responseContent);
        }

        [Test]
        public void CallAuthenticationProviderCurrentUsername_WhenModelIsValid()
        {
            controller.GetUserGroups(USER_NAME, viewModel);

            authenticationProviderMock.Verify(a => a.CurrentUserName, Times.Once);
        }

        [Test]
        public void CallGroupServiceCheckUserRights_WhenModelIsValid()
        {
            controller.GetUserGroups(USER_NAME, viewModel);

            groupServiceMock.Verify(g => g.CheckUserAcccessRights(USER_NAME, USER_NAME), Times.Once);
        }

        [Test]
        public void CallGroupServiceGetGroupsByUser_WhenMoelIsValid()
        {
            controller.GetUserGroups(USER_NAME, viewModel);

            groupServiceMock.Verify(s => s.GetGroupsByUser(USER_NAME, 0, 0), Times.Once);
        }

        [Test]
        public void CallMapperMapShortGroups_WhenModelIsValid()
        {
            controller.GetUserGroups(USER_NAME, viewModel);

            mapperMock.Verify(m => m.MapShortGroups(this.groups), Times.Once);
        }

        [Test]
        public void ReturnOkResult_WhenGroupsAreFound()
        {
            var result = controller.GetUserGroups(USER_NAME, viewModel);
            var resultContent = result as OkNegotiatedContentResult<IEnumerable<ShortGroupViewModel>>;

            Assert.IsInstanceOf<OkNegotiatedContentResult<IEnumerable<ShortGroupViewModel>>>(result);
            Assert.AreEqual(this.resultViewModels, resultContent.Content);
        }

        [Test]
        public void ReturnForbidden_WhenAccessForbiddenExceptionWasThrown()
        {
            var result = controller.GetUserGroups("invalid", viewModel);
            var resultContent = result as NegotiatedContentResult<string>;

            Assert.IsInstanceOf<NegotiatedContentResult<string>>(result);
            Assert.AreEqual(HttpStatusCode.Forbidden, resultContent.StatusCode);
        }

        [Test]
        public void ReturnInternalServerError_WhenExceptionIsThrown()
        {
            this.mapperMock.Setup(m => m.MapShortGroups(this.groups)).Throws<Exception>();

            var result = controller.GetUserGroups(USER_NAME, viewModel);
            var resultContent = result as NegotiatedContentResult<string>;

            Assert.IsInstanceOf<NegotiatedContentResult<string>>(result);
            Assert.AreEqual(HttpStatusCode.InternalServerError, resultContent.StatusCode);
        }

        [SetUp]
        public void SetUp()
        {
            this.viewModel = new PagingViewModel();
            this.groups = new List<Group>()
            {
                new Group() { Name = "Group 1" },
                new Group() { Name = "Group 2" },
            };
            this.resultViewModels = new List<ShortGroupViewModel>()
            {
                new ShortGroupViewModel("Group 1", USER_NAME),
                new ShortGroupViewModel("Group 2", USER_NAME)
            };

            this.authenticationProviderMock = new Mock<IAuthenticationProvider>();
            this.authenticationProviderMock.Setup(a => a.CurrentUserName).Returns(USER_NAME);
            this.groupServiceMock = new Mock<IGroupService>();
            this.groupServiceMock.Setup(s => s.CheckUserAcccessRights(USER_NAME, It.Is<string>(n => !n.Equals(USER_NAME))))
                .Throws<AccessForbiddenException>();
            this.groupServiceMock.Setup(s => s.GetGroupsByUser(USER_NAME, 0, 0)).Returns(this.groups);
            this.userServiceMock = new Mock<IUserService>();
            this.mapperMock = new Mock<IMapper>();
            this.mapperMock.Setup(m => m.MapShortGroups(this.groups)).Returns(this.resultViewModels);

            this.controller = new GroupsController(authenticationProviderMock.Object,
                groupServiceMock.Object, userServiceMock.Object, mapperMock.Object);
        }

    }
}
