using Moq;
using NUnit.Framework;
using SecretSanta.Data.Contracts;
using SecretSanta.Factories;
using SecretSanta.Models;
using SecretSanta.Services;

namespace SecretSanta.Tests.SecretSanta.Services.GroupServiceTests
{
    [TestFixture]
    public class MakeProcessStarted_Should
    {
        private Mock<IEfRepository<Group>> repositoryMock;
        private Mock<IGroupFactory> factoryMock;
        private Mock<IUnitOfWork> unitOfWorkMock;
        private GroupService service;
        private Group group;

        [Test]
        public void CallUnitOfWorkCommit_WhenInvoked()
        {
            this.service.MakeProcessStarted(group);

            unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }

        [Test]
        public void UpdateCorrectlyStartedProcess_WhenInvoked()
        {
            this.service.MakeProcessStarted(group);

            Assert.IsTrue(group.hasLinkingProcessStarted);
        }

        [SetUp]
        public void Setup()
        {
            this.group = new Group() { Name = "pink" };

            this.repositoryMock = new Mock<IEfRepository<Group>>();
            this.factoryMock = new Mock<IGroupFactory>();
            this.unitOfWorkMock = new Mock<IUnitOfWork>();

            this.service = new GroupService(factoryMock.Object,
                repositoryMock.Object, unitOfWorkMock.Object);
        }
    }
}
