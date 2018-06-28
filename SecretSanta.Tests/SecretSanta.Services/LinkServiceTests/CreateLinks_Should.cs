using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SecretSanta.Common;
using SecretSanta.Common.Exceptions;
using SecretSanta.Data.Contracts;
using SecretSanta.Factories;
using SecretSanta.Models;
using SecretSanta.Services;

namespace SecretSanta.Tests.SecretSanta.Services.LinkServiceTests
{
    [TestFixture]
    public class CreateLinks_Should
    {
        private Mock<IEfRepository<Link>> repositoryMock;
        private Mock<ILinkFactory> factoryMock;
        private Mock<IUnitOfWork> unitOfWorkMock;
        private LinkService service;
        private IList<Group> groups;
        private IList<User> users;
        private const string USER_ID = "1";

        [Test]
        public void ThrowAccessForbiddenException_WhenUserIsNotGroupAdmin()
        {
            var ex = Assert.Throws<AccessForbiddenException>(() => this.service.CreateLinks(this.groups[1], USER_ID));
            Assert.AreEqual(Constants.LINKING_PROCESS_START_FORBIDDEN, ex.Message);
        }

        [Test]
        public void CallFactoryCreate_WhenLinkingProcessStarts()
        {
            this.service.CreateLinks(this.groups[0], USER_ID);

            factoryMock.Verify(f => 
                f.Create(It.IsAny<Group>(), It.IsAny<User>(), It.IsAny<User>()), Times.Exactly(this.groups[0].Users.Count));
        }

        [SetUp]
        public void Setup()
        {
            this.users = new List<User>()
            {
                new User() { Id = "1" },
                new User() { Id = "2" },
            };
            this.groups = new List<Group>()
            {
                new Group()
                {
                    Name = "pink", Admin = this.users[0], Users =  this.users
                },
                new Group()
                {
                    Name = "green", Admin = this.users[1]
                },
            };

            this.repositoryMock = new Mock<IEfRepository<Link>>();
            this.factoryMock = new Mock<ILinkFactory>();
            this.unitOfWorkMock = new Mock<IUnitOfWork>();

            this.service = new LinkService(factoryMock.Object,
                repositoryMock.Object, unitOfWorkMock.Object);
        }
    }
}
