using System;
using System.Collections.Generic;
using System.Linq;
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

namespace SecretSanta.Tests.SecretSanta.Web.Controllers.LinksControllerTests
{
    [TestFixture]
    public class StartLinkingProcess_Should
    {
        private Mock<IAuthenticationProvider> authenticationProviderMock;
        private Mock<IGroupService> groupServiceMock;
        private Mock<ILinkService> linkServiceMock;
        private Mock<IMapper> mapperMock;
        private LinksController controller;
        private IEnumerable<Group> groups;
        private const string GROUP_NAME = "Sample group";
        private const string NON_EXISTING_GROUP_NAME = "Non existing group";
        private const string LINKING_COMPLETED_GROUP = "Linking completed group";
        private const string FORBIDDEN_GROUP = "Forbidden group";
        private const string USER_ID = "1";

        [Test]
        public void ReturnBadRequest_WhenModelIsInvalid()
        {
            var response = controller.StartLinkingProcess(null);
            var responseContent = response as BadRequestErrorMessageResult;

            Assert.IsInstanceOf<BadRequestErrorMessageResult>(response);
            Assert.AreEqual(Constants.INVALID_MODEL, responseContent.Message);
        }

        [Test]
        public void ReturnCreated_WhenLinkingProcessIsStarted()
        {
            var response = controller.StartLinkingProcess(GROUP_NAME);
            var responseContent = response as NegotiatedContentResult<string>;

            Assert.IsInstanceOf<NegotiatedContentResult<string>>(response);
            Assert.AreEqual(HttpStatusCode.Created, responseContent.StatusCode);
            Assert.AreEqual(Constants.LINKING_PROCESS_COMPLETE_SUCCESS, responseContent.Content);
        }

        [Test]
        public void ReturnForbidden_WhenAcessFrobiddenExceptionIsThrown()
        {
            var response = controller.StartLinkingProcess(FORBIDDEN_GROUP);
            var responseContent = response as NegotiatedContentResult<string>;

            Assert.IsInstanceOf<NegotiatedContentResult<string>>(response);
            Assert.AreEqual(HttpStatusCode.Forbidden, responseContent.StatusCode);
        }

        [Test]
        public void ReturnNotFound_WhenItemNotFoundExceptionIsThrown()
        {
            var response = controller.StartLinkingProcess(NON_EXISTING_GROUP_NAME);
            var responseContent = response as NegotiatedContentResult<string>;

            Assert.IsInstanceOf<NegotiatedContentResult<string>>(response);
            Assert.AreEqual(HttpStatusCode.NotFound, responseContent.StatusCode);
        }

        [Test]
        public void ReturnConflict_WhenItemAlreadyExistingExceptionIsThrown()
        {
            var response = controller.StartLinkingProcess(LINKING_COMPLETED_GROUP);
            var responseContent = response as NegotiatedContentResult<string>;

            Assert.IsInstanceOf<NegotiatedContentResult<string>>(response);
            Assert.AreEqual(HttpStatusCode.Conflict, responseContent.StatusCode);
        }

        [Test]
        public void ReturnInternalServerError_WhenExceptionIsThrown()
        {
            this.groupServiceMock.Setup(s => s.MakeProcessStarted(It.IsAny<Group>())).Throws<Exception>();

            var response = controller.StartLinkingProcess(GROUP_NAME);
            var responseContent = response as NegotiatedContentResult<string>;

            Assert.IsInstanceOf<NegotiatedContentResult<string>>(response);
            Assert.AreEqual(HttpStatusCode.InternalServerError, responseContent.StatusCode);
        }

        [SetUp]
        public void Setup()
        {
            this.groups = new List<Group>()
            {
                new Group()
                {
                    Name = GROUP_NAME ,
                    Users = new List<User>()
                    {
                        new User(),
                        new User()
                    }
                },
                new Group()
                {
                    Name = LINKING_COMPLETED_GROUP,
                    Users = new List<User>()
                    {
                        new User(),
                        new User()
                    }
                },
                new Group()
                {
                    Name = FORBIDDEN_GROUP,
                    Users = new List<User>()
                    {
                        new User(),
                        new User()
                    }
                }
            };

            this.authenticationProviderMock = new Mock<IAuthenticationProvider>();
            this.authenticationProviderMock.Setup(a => a.CurrentUserId).Returns(USER_ID);
            this.groupServiceMock = new Mock<IGroupService>();
            this.groupServiceMock.Setup(s => s.GetGroupByName(GROUP_NAME))
                .Returns(this.groups.First(g => g.Name.Equals(GROUP_NAME)));
            this.groupServiceMock.Setup(s => s.GetGroupByName(NON_EXISTING_GROUP_NAME)).Throws<ItemNotFoundException>();
            this.groupServiceMock.Setup(s => s.GetGroupByName(LINKING_COMPLETED_GROUP))
                .Returns(this.groups.First(g => g.Name.Equals(LINKING_COMPLETED_GROUP)));
            this.groupServiceMock.Setup(s => s.GetGroupByName(FORBIDDEN_GROUP))
                .Returns(this.groups.First(g => g.Name.Equals(FORBIDDEN_GROUP)));
            this.linkServiceMock = new Mock<ILinkService>();
            this.linkServiceMock
                .Setup(s => s.CheckLinkingProcessStarted(this.groups.ElementAt(1)))
                .Throws<ItemAlreadyExistingException>();
            this.linkServiceMock
                .Setup(s => s.CreateLinks(this.groups.ElementAt(2), USER_ID))
                .Throws<AccessForbiddenException>();
            this.mapperMock = new Mock<IMapper>();

            this.controller = new LinksController(authenticationProviderMock.Object, groupServiceMock.Object,
                linkServiceMock.Object, mapperMock.Object);
        }
    }
}
