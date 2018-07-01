using System;
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

namespace SecretSanta.Tests.SecretSanta.Services.InvitationServiceTests
{
    [TestFixture]
    public class CreateInvitation_Should
    {
        private Mock<IInvitationFactory> factoryMock;
        private Mock<IEfRepository<Invitation>> repositoryMock;
        private Mock<IUnitOfWork> unitOfWorkMock;
        private InvitationService service;
        private IEnumerable<Invitation> invitations;
        private Guid GROUP_ID = Guid.NewGuid();
        private DateTime SENT_DATE = DateTime.Today;
        private string USER_ID = "1";

        [Test]
        public void CallFactoryCreate_WhenInvitationIsCreated()
        {
            this.service.CreateInvitation(Guid.NewGuid(), SENT_DATE, "2");

            factoryMock.Verify(f => f.Create(It.IsAny<Guid>(), SENT_DATE, It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void CallRepositoryAdd_WhenInvitationIsCreated()
        {
            this.service.CreateInvitation(Guid.NewGuid(), SENT_DATE, "2");

            repositoryMock.Verify(r => r.Add(It.IsAny<Invitation>()), Times.Once);
        }

        [Test]
        public void CallUnitOfWorkCommit_WhenInvitationIsCreated()
        {
            this.service.CreateInvitation(Guid.NewGuid(), SENT_DATE, "2");

            unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }

        [Test]
        public void ThrowItemAlreadyExistingException_WhenInvitationWithProvidedGroupAndUserAlreadyExists()
        {
            var ex = Assert.Throws<ItemAlreadyExistingException>(() => this.service.CreateInvitation(GROUP_ID, SENT_DATE, USER_ID));
            Assert.AreEqual(Constants.INVITATION_ALREADY_EXISTS, ex.Message);
        }

        [SetUp]
        public void SetUp()
        {
            this.invitations = new List<Invitation>()
            {
                new Invitation()
                {
                    Group = new Group() { Id = GROUP_ID },
                    Receiver =  new User() { Id = USER_ID }
                }
            };

            this.factoryMock = new Mock<IInvitationFactory>();
            this.repositoryMock = new Mock<IEfRepository<Invitation>>();
            repositoryMock.Setup(r => r.All).Returns(this.invitations.AsQueryable);
            this.unitOfWorkMock = new Mock<IUnitOfWork>();

            this.service = new InvitationService(factoryMock.Object,
                repositoryMock.Object, unitOfWorkMock.Object);
        }
    }
}
