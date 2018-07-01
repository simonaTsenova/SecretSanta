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
    public class GetById_Should
    {
        private Mock<IInvitationFactory> factoryMock;
        private Mock<IEfRepository<Invitation>> repositoryMock;
        private Mock<IUnitOfWork> unitOfWorkMock;
        private InvitationService service;
        private IEnumerable<Invitation> invitations;
        private Invitation expectedInvitation;
        private Guid ID = Guid.NewGuid();

        [Test]
        public void ReturnCorrectInvitation_WhenProvidedValidId()
        {
            var result = this.service.GetById(ID.ToString());

            Assert.AreEqual(expectedInvitation, result);
        }

        [Test]
        public void ThrowItemNotFoundException_WhenProvidedInvalidId()
        {
            var ex = Assert.Throws<ItemNotFoundException>(() => this.service.GetById(Guid.NewGuid().ToString()));

            Assert.AreEqual(Constants.INVITATION_ID_NOT_FOUND, ex.Message);
        }

        [SetUp]
        public void SetUp()
        {
            this.invitations = new List<Invitation>()
            {
                new Invitation() { Id = ID },
                new Invitation() { Id = Guid.NewGuid() }
            };
            this.expectedInvitation = this.invitations.First(i => i.Id == ID);

            this.factoryMock = new Mock<IInvitationFactory>();
            this.repositoryMock = new Mock<IEfRepository<Invitation>>();
            repositoryMock.Setup(r => r.All).Returns(this.invitations.AsQueryable);
            this.unitOfWorkMock = new Mock<IUnitOfWork>();

            this.service = new InvitationService(factoryMock.Object, 
                repositoryMock.Object,unitOfWorkMock.Object);
        }
    }
}
