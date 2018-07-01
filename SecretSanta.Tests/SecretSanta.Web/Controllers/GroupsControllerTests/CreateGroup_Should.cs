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
using SecretSanta.Web.Models.Groups;

namespace SecretSanta.Tests.SecretSanta.Web.Controllers.GroupsControllerTests
{
    [TestFixture]
    public class CreateGroup_Should
    {
        private Mock<IAuthenticationProvider> authenticationProviderMock;
        private Mock<IGroupService> groupServiceMock;
        private Mock<IUserService> userServiceMock;
        private Mock<IMapper> mapperMock;
        private GroupsController controller;
        private IList<CreateGroupViewModel> viewModels;
        private CreateGroupViewModel viewModel;
        private DisplayGroupViewModel resultViewModel;
        private User user;
        private Group group;
        private const string USER_ID = "1";
        private const string GROUP_NAME = "Sample group";
        private const string EXISTING_GROUP_NAME = "Existing group";
        private const string INVALID_GROUP_NAME = "Invalid group";

        [Test]
        public void ReturnBadRequest_WhenModelIsNull()
        {
            var response = controller.CreateGroup(null);
            var responseContent = (response as BadRequestErrorMessageResult).Message;
            
            Assert.IsInstanceOf<BadRequestErrorMessageResult>(response);
            Assert.AreEqual(Constants.GROUP_NAME_REQUIRED, responseContent);
        }

        [Test]
        public void ReturnBadRequest_WhenModelIsNotValid()
        {
            controller.ModelState.AddModelError("key", "error message");

            var response = controller.CreateGroup(viewModel);

            Assert.IsInstanceOf<InvalidModelStateResult>(response);
        }

        [Test]
        public void CallAuthenticationProviderCurrentUserId_WhenModelIsValid()
        {
            controller.CreateGroup(viewModel);

            authenticationProviderMock.Verify(a => a.CurrentUserId, Times.Once);
        }

        [Test]
        public void CallUserServiceGetUserById_WhenModelIsValid()
        {
            controller.CreateGroup(viewModel);

            userServiceMock.Verify(s => s.GetUserById(USER_ID), Times.Once);
        }

        [Test]
        public void CallGroupServiceCreateGroup_WhenModelIsValid()
        {
            controller.CreateGroup(viewModel);

            groupServiceMock.Verify(s => s.CreateGroup(GROUP_NAME, this.user), Times.Once);
        }

        [Test]
        public void CallMapperMapGroup_WhenModelIsValid()
        {
            controller.CreateGroup(viewModel);

            mapperMock.Verify(m => m.MapGroup(this.group), Times.Once);
        }


        [Test]
        public void ReturnCreatedResultWithRightModel_WhenGroupIsCreated()
        {
            var result = controller.CreateGroup(viewModel);
            var resultContent = result as NegotiatedContentResult<DisplayGroupViewModel>;

            Assert.IsInstanceOf<NegotiatedContentResult<DisplayGroupViewModel>>(result);
            Assert.AreEqual(HttpStatusCode.Created, resultContent.StatusCode);
        }

        [Test]
        public void ReturnNotFound_WhenItemNotFoundExceptionIsThrown()
        {
            this.userServiceMock.Setup(s => s.GetUserById(It.IsAny<string>())).Throws<ItemNotFoundException>();

            var result = controller.CreateGroup(viewModel);
            var resultContent = result as NegotiatedContentResult<string>;

            Assert.IsInstanceOf<NegotiatedContentResult<string>>(result);
            Assert.AreEqual(HttpStatusCode.NotFound, resultContent.StatusCode);
        }

        [Test]
        public void ReturnConflict_WhenItemAlreadyExistingExceptionIsThrown()
        {
            viewModel = viewModels.First(m => m.Name.Equals(EXISTING_GROUP_NAME));

            var result = controller.CreateGroup(viewModel);
            var resultContent = result as NegotiatedContentResult<string>;

            Assert.IsInstanceOf<NegotiatedContentResult<string>>(result);
            Assert.AreEqual(HttpStatusCode.Conflict, resultContent.StatusCode);
        }

        [Test]
        public void ReturnInternalServerError_WhenExceptionIsThrown()
        {
            viewModel = viewModels.First(m => m.Name.Equals(INVALID_GROUP_NAME));

            var result = controller.CreateGroup(viewModel);
            var resultContent = result as NegotiatedContentResult<string>;

            Assert.IsInstanceOf<NegotiatedContentResult<string>>(result);
            Assert.AreEqual(HttpStatusCode.InternalServerError, resultContent.StatusCode);
        }

        [SetUp]
        public void SetUp()
        {
            this.viewModels = new List<CreateGroupViewModel>()
            {
                new CreateGroupViewModel()
                {
                    Name = GROUP_NAME
                },
                new CreateGroupViewModel()
                {
                    Name = INVALID_GROUP_NAME
                },
                new CreateGroupViewModel()
                {
                    Name = EXISTING_GROUP_NAME
                }
            };
            this.viewModel = viewModels.First(m => m.Name.Equals(GROUP_NAME));
            this.resultViewModel = new DisplayGroupViewModel() { GroupName = GROUP_NAME };

            this.user = new User() { Id = USER_ID };
            this.group = new Group() { Name = GROUP_NAME };

            this.authenticationProviderMock = new Mock<IAuthenticationProvider>();
            this.authenticationProviderMock.Setup(a => a.CurrentUserId).Returns(USER_ID);
            this.groupServiceMock = new Mock<IGroupService>();
            this.groupServiceMock.Setup(g => g.CreateGroup(GROUP_NAME, this.user)).Returns(this.group);
            this.groupServiceMock.Setup(g => g.CreateGroup(EXISTING_GROUP_NAME, this.user)).Throws<ItemAlreadyExistingException>();
            this.groupServiceMock.Setup(g => g.CreateGroup(INVALID_GROUP_NAME, this.user)).Throws<Exception>();
            this.userServiceMock = new Mock<IUserService>();
            this.userServiceMock.Setup(s => s.GetUserById(USER_ID)).Returns(this.user);
            this.mapperMock = new Mock<IMapper>();
            this.mapperMock.Setup(m => m.MapGroup(this.group)).Returns(resultViewModel);

            this.controller = new GroupsController(authenticationProviderMock.Object,
                groupServiceMock.Object, userServiceMock.Object, mapperMock.Object);
        }
    }
}
