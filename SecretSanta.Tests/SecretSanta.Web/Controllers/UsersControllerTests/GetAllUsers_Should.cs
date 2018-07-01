using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Results;
using Moq;
using NUnit.Framework;
using SecretSanta.Authentication.Contracts;
using SecretSanta.Common;
using SecretSanta.Factories;
using SecretSanta.Models;
using SecretSanta.Models.Enumerations;
using SecretSanta.Services.Contracts;
using SecretSanta.Web.Controllers;
using SecretSanta.Web.Mapper;
using SecretSanta.Web.Models;
using SecretSanta.Web.Models.Users;

namespace SecretSanta.Tests.SecretSanta.Web.Controllers.UsersControllerTests
{
    [TestFixture]
    public class GetAllUsers_Should
    {
        private Mock<IAuthenticationProvider> authenticationProviderMock;
        private Mock<IUserService> userServiceMock;
        private Mock<IUserFactory> userFactoryMock;
        private Mock<IMapper> mapperMock;
        private UsersController controller;
        private ResultFormatViewModel model;
        private IList<User> users;
        private IEnumerable<DisplayUserViewModel> resultViewModels;
        private const int SKIP = 0;
        private const int TAKE = 0;
        private const string SEARCH = "na";

        [Test]
        public void ReturnBadRequest_WhenModelIsInvalid()
        {
            var result = controller.GetAllUsers(null);
            var resultContent = result as BadRequestErrorMessageResult;

            Assert.IsInstanceOf<BadRequestErrorMessageResult>(result);
            Assert.AreEqual(Constants.INVALID_MODEL, resultContent.Message);
        }

        [Test]
        public void CallServiceGetAllUsers_WhenModelIsValid()
        {
            controller.GetAllUsers(model);

            userServiceMock.Verify(s => s.GetAllUsers(SKIP, TAKE, OrderType.Ascending, SEARCH));
        }

        [Test]
        public void CallMapperMapUsers_WhenModelIsValid()
        {
            controller.GetAllUsers(model);

            mapperMock.Verify(m => m.MapUsers(this.users), Times.Once);
        }

        [Test]
        public void ReturnOkResult_WhenUsersAreFound()
        {
            var result = controller.GetAllUsers(model);
            var resultContent = result as OkNegotiatedContentResult<IEnumerable<DisplayUserViewModel>>;

            Assert.IsInstanceOf<OkNegotiatedContentResult<IEnumerable<DisplayUserViewModel>>>(result);
            Assert.AreEqual(this.resultViewModels, resultContent.Content);
        }

        [SetUp]
        public void SetUp()
        {
            this.model = new ResultFormatViewModel()
            {
                Search = SEARCH
            };

            this.users = new List<User>()
            {
                new User() { UserName = "one" },
                new User() { UserName = "two" }
            };

            this.resultViewModels = new List<DisplayUserViewModel>()
            {
                new DisplayUserViewModel() {UserName = "one"},
                new DisplayUserViewModel() {UserName = "two"}
            };

            this.authenticationProviderMock = new Mock<IAuthenticationProvider>();
            this.userServiceMock = new Mock<IUserService>();
            this.userServiceMock.Setup(s => s.GetAllUsers(SKIP, TAKE, OrderType.Ascending, SEARCH)).Returns(this.users);
            this.userFactoryMock = new Mock<IUserFactory>();
            this.mapperMock = new Mock<IMapper>();
            this.mapperMock.Setup(m => m.MapUsers(this.users)).Returns(this.resultViewModels);

            this.controller = new UsersController(authenticationProviderMock.Object,
                userServiceMock.Object, userFactoryMock.Object, mapperMock.Object);
        }
    }
}
