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
    public class GetByGroupAndUser_Should
    {
        private Mock<IInvitationFactory> factoryMock;
        private Mock<IEfRepository<Invitation>> repositoryMock;
        private Mock<IUnitOfWork> unitOfWorkMock;
        private InvitationService service;
        private IEnumerable<Invitation> invitations;
        private Invitation expectedInvitation;
        private Guid GROUP_ID = Guid.NewGuid();
        private string USER_ID = "1";

        [Test]
        public void ReturnCorrectInvitation_WhenProvidedValidGroupAndUser()
        {
            var result = this.service.GetByGroupAndUser(GROUP_ID, USER_ID);

            Assert.AreEqual(expectedInvitation, result);
        }

        [Test]
        public void ThrowItemNotFoundException_WhenProvidedInvalidGroupAndUser()
        {
            var ex = Assert.Throws<ItemNotFoundException>(
                () => this.service.GetByGroupAndUser(Guid.NewGuid(), "2"));

            Assert.AreEqual(Constants.INVITATION_NOT_FOUND, ex.Message);
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
                },
                new Invitation()
                {
                    Group = new Group() { Id = Guid.NewGuid() },
                    Receiver = new User() { Id = "2" }
                }
            };
            this.expectedInvitation = this.invitations
                .First(i => i.Group.Id == GROUP_ID && i.Receiver.Id == USER_ID);

            this.factoryMock = new Mock<IInvitationFactory>();
            this.repositoryMock = new Mock<IEfRepository<Invitation>>();
            repositoryMock.Setup(r => r.All).Returns(this.invitations.AsQueryable);
            this.unitOfWorkMock = new Mock<IUnitOfWork>();

            this.service = new InvitationService(factoryMock.Object,
                repositoryMock.Object, unitOfWorkMock.Object);
        }
    }
}
