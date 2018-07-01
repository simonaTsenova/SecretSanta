using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using SecretSanta.Authentication.Contracts;
using SecretSanta.Common;
using SecretSanta.Data.Contracts;
using SecretSanta.Models;
using SecretSanta.Services;
using SecretSanta.Common.Exceptions;
using SecretSanta.Factories;

namespace SecretSanta.Tests.SecretSanta.Services.UsersServiceTests
{
    [TestFixture]
    public class GetUserByUserName_Should
    {
        private Mock<IAuthenticationProvider> authenticationProviderMock;
        private Mock<IUserFactory> factoryMock;
        private Mock<IEfRepository<User>> repositoryMock;
        private UserService service;
        private IEnumerable<User> users;
        private User expectedUser;
        private const string USERNAME = "Sample";

        [Test]
        public void ReturnCorrectUser_WhenProvidedValidUsername()
        {
            var result = service.GetUserByUserName(USERNAME);

            Assert.AreEqual(expectedUser, result);
        }

        [Test]
        public void ThrowItemNotFoundException_WhenProvidedInvalidUsername()
        {
            var ex = Assert.Throws<ItemNotFoundException>(() => service.GetUserByUserName("username"));
            Assert.AreEqual(Constants.USERNAME_NOT_FOUND, ex.Message);
        }

        [SetUp]
        public void Setup()
        {
            this.users = new List<User>()
            {
                new User("Test 1", "Test 1", "Test 1", "Test 1", "Test 1"),
                new User("Sample", "Sample", "Sample", "Sample", "Sample"),
            };

            this.expectedUser = users.First(u => u.UserName.Equals(USERNAME));

            this.authenticationProviderMock = new Mock<IAuthenticationProvider>();
            this.factoryMock = new Mock<IUserFactory>();
            this.repositoryMock = new Mock<IEfRepository<User>>();
            this.repositoryMock.Setup(r => r.All).Returns(this.users.AsQueryable());

            this.service = new UserService(authenticationProviderMock.Object,
                factoryMock.Object, repositoryMock.Object);
        }
    }
}
