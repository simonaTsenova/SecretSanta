using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using SecretSanta.Common;
using SecretSanta.Common.Exceptions;
using SecretSanta.Data.Contracts;
using SecretSanta.Factories;
using SecretSanta.Models;
using SecretSanta.Services;

namespace SecretSanta.Tests.SecretSanta.Services.GroupServiceTests
{
    [TestFixture]
    public class CreateGroup_Should
    {
        private Mock<IEfRepository<Group>> repositoryMock;
        private Mock<IGroupFactory> factoryMock;
        private Mock<IUnitOfWork> unitOfWorkMock;
        private GroupService service;
        private Group group;
        private User user;
        private const string NAME = "pink";

        [Test]
        public void CallFactoryCreate_WhenGroupIsCreated()
        {
            this.service.CreateGroup(NAME, this.user);

            factoryMock.Verify(f => f.Create(NAME, this.user), Times.Once);
        }

        [Test]
        public void CallRepositoryAdd_WhenGroupIsCreated()
        {
            this.service.CreateGroup(NAME, this.user);

            repositoryMock.Verify(r => r.Add(It.IsAny<Group>()), Times.Once);
        }

        [Test]
        public void CallUnitOfWorkCommit_WhenGroupIsCreated()
        {
            this.service.CreateGroup(NAME, this.user);

            unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }

        [Test]
        public void ThrowItemAlreadyExistingException_WhenGroupWithProvidedNameAlreadyExists()
        {
            repositoryMock.Setup(r => r.All).Returns(new List<Group>() { this.group }.AsQueryable);

            var ex = Assert.Throws<ItemAlreadyExistingException>(() => this.service.CreateGroup(NAME, this.user));
            Assert.AreEqual(Constants.GROUP_NAME_ALREADY_EXISTS, ex.Message);
        }

        [SetUp]
        public void Setup()
        {
            this.group = new Group() { Name = "pink" };
            this.user = new User() { UserName = "joe" };

            this.repositoryMock = new Mock<IEfRepository<Group>>();
            this.factoryMock = new Mock<IGroupFactory>();
            this.factoryMock.Setup(f => f.Create(NAME, this.user)).Returns(this.group);
            this.unitOfWorkMock = new Mock<IUnitOfWork>();

            this.service = new GroupService(factoryMock.Object,
                repositoryMock.Object, unitOfWorkMock.Object);
        }
    }
}
