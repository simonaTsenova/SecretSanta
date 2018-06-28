using Moq;
using NUnit.Framework;
using SecretSanta.Common;
using SecretSanta.Common.Exceptions;
using SecretSanta.Data.Contracts;
using SecretSanta.Factories;
using SecretSanta.Models;
using SecretSanta.Services;

namespace SecretSanta.Tests.SecretSanta.Services.InvitationServiceTests
{
    [TestFixture]
    public class CheckUserAccessRights_Should
    {
        private Mock<IInvitationFactory> factoryMock;
        private Mock<IEfRepository<Invitation>> repositoryMock;
        private Mock<IUnitOfWork> unitOfWorkMock;
        private InvitationService service;

        [Test]
        public void ThrowAccessForbiddenException_WhenUserHasNoAccessRights()
        {
            var ex = Assert.Throws<AccessForbiddenException>(() => this.service.CheckUserAcccessRights("jack", "joe"));
            Assert.AreEqual(Constants.INVITATION_ACCESS_FORBIDDEN, ex.Message); 
        }

        [SetUp]
        public void SetUp()
        {
            this.factoryMock = new Mock<IInvitationFactory>();
            this.repositoryMock = new Mock<IEfRepository<Invitation>>();
            this.unitOfWorkMock = new Mock<IUnitOfWork>();

            this.service = new InvitationService(factoryMock.Object,
                repositoryMock.Object, unitOfWorkMock.Object);
        }
    }
}
