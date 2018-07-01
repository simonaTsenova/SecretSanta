using System;
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
using SecretSanta.Web.Models.Links;

namespace SecretSanta.Tests.SecretSanta.Web.Controllers.LinksControllerTests
{
    [TestFixture]
    public class GetUserGroupLinks_Should
    {
        private Mock<IAuthenticationProvider> authenticationProviderMock;
        private Mock<IGroupService> groupServiceMock;
        private Mock<ILinkService> linkServiceMock;
        private Mock<IMapper> mapperMock;
        private LinksController controller;
        private Link link;
        private Group group;
        private LinkViewModel resultViewModel;
        private const string GROUP_NAME = "Sample group";
        private const string NON_EXISTING_GROUP_NAME = "Non existing group";
        private const string USER_NAME = "joe";


        [Test]
        public void ReturnBadRequest_WhenModelIsInvalid()
        {
            var response = controller.GetUserGroupLinks(null, null);
            var responseContent = response as BadRequestErrorMessageResult;

            Assert.IsInstanceOf<BadRequestErrorMessageResult>(response);
            Assert.AreEqual(Constants.INVALID_MODEL, responseContent.Message);
        }

        [Test]
        public void ReturnOkResult_WhenModelIsValid()
        {
            var response = controller.GetUserGroupLinks(USER_NAME, GROUP_NAME);
            var responseContent = response as OkNegotiatedContentResult<LinkViewModel>;

            Assert.IsInstanceOf<OkNegotiatedContentResult<LinkViewModel>>(response);
            Assert.AreEqual(this.resultViewModel, responseContent.Content);
        }

        [Test]
        public void ReturnNotFound_WhenItemNotFoundExceptionIsThrown()
        {
            var response = controller.GetUserGroupLinks(USER_NAME, NON_EXISTING_GROUP_NAME);
            var responseContent = response as NegotiatedContentResult<string>;

            Assert.IsInstanceOf<NegotiatedContentResult<string>>(response);
            Assert.AreEqual(HttpStatusCode.NotFound, responseContent.StatusCode);
        }

        [Test]
        public void ReturnForbidden_WhenAccessForbiddenExceptionIsThrown()
        {
            var response = controller.GetUserGroupLinks("jack", GROUP_NAME);
            var responseContent = response as NegotiatedContentResult<string>;

            Assert.IsInstanceOf<NegotiatedContentResult<string>>(response);
            Assert.AreEqual(HttpStatusCode.Forbidden, responseContent.StatusCode);
        }

        [Test]
        public void ReturnInternalServerError_WhenExceptionExceptionIsThrown()
        {
            this.mapperMock.Setup(m => m.MapLink(this.link)).Throws<Exception>();

            var response = controller.GetUserGroupLinks(USER_NAME, GROUP_NAME);
            var responseContent = response as NegotiatedContentResult<string>;

            Assert.IsInstanceOf<NegotiatedContentResult<string>>(response);
            Assert.AreEqual(HttpStatusCode.InternalServerError, responseContent.StatusCode);
        }

        [SetUp]
        public void Setup()
        {
            this.group = new Group()
            {
                Name = GROUP_NAME
            };
            this.link = new Link()
            {
                Group = this.group
            };
            this.resultViewModel = new LinkViewModel(USER_NAME);

            this.authenticationProviderMock = new Mock<IAuthenticationProvider>();
            this.authenticationProviderMock.Setup(a => a.CurrentUserName).Returns(USER_NAME);
            this.groupServiceMock = new Mock<IGroupService>();
            this.groupServiceMock.Setup(s => s.GetGroupByName(GROUP_NAME)).Returns(this.group);
            this.groupServiceMock.Setup(s => s.GetGroupByName(NON_EXISTING_GROUP_NAME)).Throws<ItemNotFoundException>();
            this.linkServiceMock = new Mock<ILinkService>();
            this.linkServiceMock
                .Setup(s => s.CheckUserAcccessRights(USER_NAME, It.Is<string>(n => !n.Equals(USER_NAME))))
                .Throws<AccessForbiddenException>();
            this.linkServiceMock.Setup(s => s.GetByGroupAndSender(GROUP_NAME, USER_NAME)).Returns(this.link);
            this.mapperMock = new Mock<IMapper>();
            this.mapperMock.Setup(m => m.MapLink(this.link)).Returns(this.resultViewModel);

            this.controller = new LinksController(authenticationProviderMock.Object, groupServiceMock.Object,
                linkServiceMock.Object, mapperMock.Object);
        }
    }
}
