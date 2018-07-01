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
    public class CheckUserAccessRights_Should
    {
        private Mock<IEfRepository<Link>> repositoryMock;
        private Mock<ILinkFactory> factoryMock;
        private Mock<IUnitOfWork> unitOfWorkMock;
        private LinkService service;

        [Test]
        public void ThrowAccessForbiddenException_WhenUserHasNoLinkAccessRights()
        {
            var ex = Assert.Throws<AccessForbiddenException>(
                () => this.service.CheckUserAcccessRights("joe", "jack"));
            Assert.AreEqual(Constants.LINKS_ACCESS_FORBIDDEN, ex.Message);
        }

        [SetUp]
        public void Setup()
        {
            this.repositoryMock = new Mock<IEfRepository<Link>>();
            this.factoryMock = new Mock<ILinkFactory>();
            this.unitOfWorkMock = new Mock<IUnitOfWork>();

            this.service = new LinkService(factoryMock.Object,
                repositoryMock.Object, unitOfWorkMock.Object);
        }
    }
}
