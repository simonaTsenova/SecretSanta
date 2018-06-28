using Moq;
using NUnit.Framework;
using SecretSanta.Data.Contracts;
using SecretSanta.Factories;
using SecretSanta.Models;
using SecretSanta.Services;

namespace SecretSanta.Tests.SecretSanta.Services.InvitationServiceTests
{
    [TestFixture]
    public class DeleteInvitation_Should
    {
        private Mock<IInvitationFactory> factoryMock;
        private Mock<IEfRepository<Invitation>> repositoryMock;
        private Mock<IUnitOfWork> unitOfWorkMock;
        private InvitationService service;
        private Invitation invitation;

        [Test]
        public void CallRepositoryDelete_WhenInvoked()
        {
            this.service.DeleteInvitation(invitation);
            
            repositoryMock.Verify(r => r.Delete(invitation), Times.Once);
        }

        [Test]
        public void CallUnitOfWorkCommit_WhenInvoked()
        {
            this.service.DeleteInvitation(invitation);

            unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }

        [SetUp]
        public void SetUp()
        {
            this.invitation = new Invitation();

            this.factoryMock = new Mock<IInvitationFactory>();
            this.repositoryMock = new Mock<IEfRepository<Invitation>>();
            this.unitOfWorkMock = new Mock<IUnitOfWork>();

            this.service = new InvitationService(factoryMock.Object,
                repositoryMock.Object, unitOfWorkMock.Object);
        }
    }
}
