using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using SecretSanta.Common;
using SecretSanta.Common.Exceptions;
using SecretSanta.Data.Contracts;
using SecretSanta.Models;
using SecretSanta.Services;

namespace SecretSanta.Tests.SecretSanta.Services.UsersServiceTests
{
    [TestFixture]
    public class GetUserById_Should
    {
        private Mock<IEfRepository<User>> repositoryMock;
        private UserService service;
        private IEnumerable<User> users;
        private User expectedUser;
        private const string ID = "1";

        [Test]
        public void ReturnCorrectUser_WhenProvidedValidID()
        {
            var result = service.GetUserById(ID);

            Assert.AreEqual(expectedUser, result);
        }

        [Test]
        public void ThrowItemNotFoundException_WhenProvidedInvalidUsername()
        {
            var ex = Assert.Throws<ItemNotFoundException>(() => service.GetUserById("2"));
            Assert.AreEqual(Constants.USER_ID_NOT_FOUND, ex.Message);
        }

        [SetUp]
        public void Setup()
        {
            this.users = new List<User>()
            {
                new User() { Id = "1" }, 
            };

            this.expectedUser = users.First(u => u.Id.Equals(ID));

            this.repositoryMock = new Mock<IEfRepository<User>>();
            this.repositoryMock.Setup(r => r.All).Returns(this.users.AsQueryable());

            this.service = new UserService(repositoryMock.Object);
        }
    }
}
