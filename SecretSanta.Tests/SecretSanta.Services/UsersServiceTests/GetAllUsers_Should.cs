using Moq;
using NUnit.Framework;
using SecretSanta.Data.Contracts;
using SecretSanta.Models;
using SecretSanta.Models.Enumerations;
using SecretSanta.Services;
using System.Collections.Generic;
using System.Linq;
using SecretSanta.Authentication.Contracts;
using SecretSanta.Factories;

namespace SecretSanta.Tests.SecretSanta.Services.UsersServiceTests
{
    [TestFixture]
    public class GetAllUsers_Should
    {
        private Mock<IAuthenticationProvider> authenticationProviderMock;
        private Mock<IUserFactory> factoryMock;
        private Mock<IEfRepository<User>> repositoryMock;
        private UserService service;
        private IEnumerable<User> users;
        private IEnumerable<User> expectedUsers;
        private const int SKIP = 0;
        private const int TAKE = 0;
        private const string SEARCH = "es";

        [Test]
        public void CallRepositoryAll_WhenInvoked()
        {
            service.GetAllUsers(SKIP, TAKE, OrderType.Ascending, SEARCH);

            this.repositoryMock.Verify(r => r.All, Times.Once);
        }

        [Test]
        public void ReturnCorrectUsersCollection_WhenOrderTypeIsAscending()
        {
            var result = service.GetAllUsers(SKIP, TAKE, OrderType.Ascending, SEARCH);

            Assert.AreEqual(this.expectedUsers.Count(), result.Count());
            CollectionAssert.AreEqual(this.expectedUsers, result);
        }

        [Test]
        public void ReturnCorrectUsersCollection_WhenOrderTypeIsDescending()
        {
            var result = service.GetAllUsers(SKIP, TAKE, OrderType.Descending, SEARCH);

            Assert.AreEqual(this.expectedUsers.Count(), result.Count());
            CollectionAssert.AreEqual(this.expectedUsers.OrderByDescending(u => u.DisplayName), result);
        }

        [SetUp]
        public void Setup()
        {
            this.users = new List<User>()
            {
                new User("Test 1", "Test 1", "Test 1", "Test 1", "Test 1"),
                new User("Sample", "Sample", "Sample", "Sample", "Sample"),
                new User("Test 2", "Test 2", "Test 2", "Test 2", "Test 2")
            };

            this.expectedUsers = this.users.Where(u => u.DisplayName.Contains(SEARCH));

            this.authenticationProviderMock = new Mock<IAuthenticationProvider>();
            this.factoryMock = new Mock<IUserFactory>();
            this.repositoryMock = new Mock<IEfRepository<User>>();
            this.repositoryMock.Setup(r => r.All).Returns(this.users.AsQueryable());

            this.service = new UserService(authenticationProviderMock.Object,
                factoryMock.Object, repositoryMock.Object);
        }
    }
}
