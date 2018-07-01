using Moq;
using NUnit.Framework;
using SecretSanta.Data.Contracts;
using SecretSanta.Factories;
using SecretSanta.Models;
using SecretSanta.Services;

namespace SecretSanta.Tests.SecretSanta.Services.LinkServiceTests
{
    [TestFixture]
    public class CreateLink_Should
    {
        private Mock<IEfRepository<Link>> repositoryMock;
        private Mock<ILinkFactory> factoryMock;
        private Mock<IUnitOfWork> unitOfWorkMock;
        private LinkService service;
        private Group group;
        private User sender;
        private User receiver;
        private Link link;

        [Test]
        public void CallFactoryCreate_WhenInvoked()
        {
            this.service.CreateLink(this.group, this.sender, this.receiver);

            factoryMock.Verify(f => f.Create(this.group, this.sender, this.receiver));
        }

        [Test]
        public void CallRepositoryAdd_WhenInvoked()
        {
            this.service.CreateLink(this.group, this.sender, this.receiver);

            repositoryMock.Verify(r => r.Add(this.link), Times.Once);
        }

        [Test]
        public void CallUnitOfWorkCommit_WhenInvoked()
        {
            this.service.CreateLink(this.group, this.sender, this.receiver);

            unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }

        [SetUp]
        public void Setup()
        {
            this.group = new Group() { Name = "pink" };
            this.sender = new User() { UserName = "sender" };
            this.receiver = new User() { UserName = "receiver" };
            this.link = new Link(group, sender, receiver);

            this.repositoryMock = new Mock<IEfRepository<Link>>();
            this.factoryMock = new Mock<ILinkFactory>();
            factoryMock.Setup(f => f.Create(group, sender, receiver)).Returns(this.link);
            this.unitOfWorkMock = new Mock<IUnitOfWork>();

            this.service = new LinkService(factoryMock.Object,
                repositoryMock.Object, unitOfWorkMock.Object);
        }
    }
}
