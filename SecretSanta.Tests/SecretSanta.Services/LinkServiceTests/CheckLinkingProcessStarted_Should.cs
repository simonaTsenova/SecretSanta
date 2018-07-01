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
    public class CheckLinkingProcessStarted_Should
    {
        private Mock<IEfRepository<Link>> repositoryMock;
        private Mock<ILinkFactory> factoryMock;
        private Mock<IUnitOfWork> unitOfWorkMock;
        private LinkService service;
        private Group group;

        [Test]
        public void ThrowItemAlreadyExistingException_WhenLinkingProcessCompleted()
        {
            var ex = Assert.Throws<ItemAlreadyExistingException>(
                () => this.service.CheckLinkingProcessStarted(this.@group));
            Assert.AreEqual(Constants.LINKING_PROCESS_ALREADY_DONE, ex.Message);
        }

        [SetUp]
        public void Setup()
        {
            this.group = new Group() { Name = "pink", hasLinkingProcessStarted = true };

            this.repositoryMock = new Mock<IEfRepository<Link>>();
            this.factoryMock = new Mock<ILinkFactory>();
            this.unitOfWorkMock = new Mock<IUnitOfWork>();

            this.service = new LinkService(factoryMock.Object,
                repositoryMock.Object, unitOfWorkMock.Object);
        }
    }
}
