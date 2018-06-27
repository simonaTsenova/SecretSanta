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
    public class CheckUserAccessRights_Should
    {
        private Mock<IEfRepository<Group>> repositoryMock;
        private Mock<IGroupFactory> factoryMock;
        private Mock<IUnitOfWork> unitOfWorkMock;
        private GroupService service;

        [Test]
        public void ThrowAccessForbiddenException_WhenUserHasNoGroupAccessRights()
        {
            var ex = Assert.Throws<AccessForbiddenException>(
                () => this.service.CheckUserAcccessRights("joe", "jack"));
            Assert.AreEqual(Constants.GROUP_ACCESS_FORBIDDEN, ex.Message);
        }

        [SetUp]
        public void Setup()
        {
            this.repositoryMock = new Mock<IEfRepository<Group>>();
            this.factoryMock = new Mock<IGroupFactory>();
            this.unitOfWorkMock = new Mock<IUnitOfWork>();

            this.service = new GroupService(factoryMock.Object,
                repositoryMock.Object, unitOfWorkMock.Object);
        }
    }
}
