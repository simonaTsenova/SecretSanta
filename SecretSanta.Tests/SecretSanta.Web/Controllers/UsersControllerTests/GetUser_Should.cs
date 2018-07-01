using System;
using System.Net;
using System.Web.Http.Results;
using Moq;
using NUnit.Framework;
using SecretSanta.Authentication.Contracts;
using SecretSanta.Common;
using SecretSanta.Common.Exceptions;
using SecretSanta.Factories;
using SecretSanta.Models;
using SecretSanta.Services.Contracts;
using SecretSanta.Web.Controllers;
using SecretSanta.Web.Mapper;
using SecretSanta.Web.Models.Users;

namespace SecretSanta.Tests.SecretSanta.Web.Controllers.UsersControllerTests
{
    [TestFixture]
    public class GetUser_Should
    {
        private Mock<IAuthenticationProvider> authenticationProviderMock;
        private Mock<IUserService> userServiceMock;
        private Mock<IUserFactory> userFactoryMock;
        private Mock<IMapper> mapperMock;
        private UsersController controller;
        private User user;
        private DisplayUserViewModel resultViewModel;
        private const string USER_NAME = "jack";

        [Test]
        public void ReturnBadRequest_WhenUsernameIsNull()
        {
            var result = controller.GetUser(null);
            var resultContent = result as BadRequestErrorMessageResult;

            Assert.IsInstanceOf<BadRequestErrorMessageResult>(result);
            Assert.AreEqual(Constants.REQUIRED_USERNAME, resultContent.Message);
        }

        [Test]
        public void CallGetUserByUsername_WhenUsernameIsNotNull()
        {
            controller.GetUser(USER_NAME);

            userServiceMock.Verify(s => s.GetUserByUserName(USER_NAME), Times.Once);
        }

        [Test]
        public void CallMapperMapUser_WhenUserIsFound()
        {
            controller.GetUser(USER_NAME);

            mapperMock.Verify(m => m.MapUser(this.user));
        }

        [Test]
        public void ReturnOkResult_WhenUserIsFound()
        {
            var response = controller.GetUser(USER_NAME);
            var responseContent = response as OkNegotiatedContentResult<DisplayUserViewModel>;

            Assert.IsInstanceOf<OkNegotiatedContentResult<DisplayUserViewModel>>(response);
            Assert.AreEqual(this.resultViewModel, responseContent.Content);
        }

        [Test]
        public void ReturnNotFound_WhenItemNotFoundExceptionIsThrown()
        {
            var response = controller.GetUser("Non existing username");
            var responseContent = response as NegotiatedContentResult<string>;

            Assert.IsInstanceOf<NegotiatedContentResult<string>>(response);
            Assert.AreEqual(HttpStatusCode.NotFound, responseContent.StatusCode);
        }

        [Test]
        public void ReturnInternalServerError_WhenExceptionIsThrown()
        {
            this.mapperMock.Setup(m => m.MapUser(It.IsAny<User>())).Throws<Exception>();

            var response = controller.GetUser(USER_NAME);
            var responseContent = response as NegotiatedContentResult<string>;

            Assert.IsInstanceOf<NegotiatedContentResult<string>>(response);
            Assert.AreEqual(HttpStatusCode.InternalServerError, responseContent.StatusCode);
        }

        [SetUp]
        public void SetUp()
        {
            this.user = new User()
            {
                UserName = USER_NAME
            };
            this.resultViewModel = new DisplayUserViewModel()
            {
                UserName = USER_NAME
            };

            this.authenticationProviderMock = new Mock<IAuthenticationProvider>();
            this.userServiceMock = new Mock<IUserService>();
            this.userServiceMock.Setup(s => s.GetUserByUserName(USER_NAME)).Returns(this.user);
            this.userServiceMock.Setup(s => s.GetUserByUserName("Non existing username"))
                .Throws<ItemNotFoundException>();
            this.userFactoryMock = new Mock<IUserFactory>();
            this.mapperMock = new Mock<IMapper>();
            this.mapperMock.Setup(m => m.MapUser(this.user)).Returns(this.resultViewModel);

            this.controller = new UsersController(authenticationProviderMock.Object,
                userServiceMock.Object, userFactoryMock.Object, mapperMock.Object);
        }
    }
}
