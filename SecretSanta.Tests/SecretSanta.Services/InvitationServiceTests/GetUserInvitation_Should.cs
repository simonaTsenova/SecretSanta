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
    public class GetUserInvitation_Should
    {
        private Mock<IInvitationFactory> factoryMock;
        private Mock<IEfRepository<Invitation>> repositoryMock;
        private Mock<IUnitOfWork> unitOfWorkMock;
        private InvitationService service;
        private IList<Invitation> invitations;
        private Guid GROUP_ID = Guid.NewGuid();
        private string USER_ID = "1";

        [Test]
        public void ThrowAccessForbiddenException_WhenInvitationDoesNotExist()
        {
            var ex = Assert.Throws<AccessForbiddenException>(
                () => this.service.GetUserInvitation(Guid.NewGuid(), USER_ID));
            Assert.AreEqual(Constants.USER_HAS_NO_INVITATIONS_FOR_GROUP, ex.Message);
        }

        [Test]
        public void ReturnCorrectInvitation_WhenInvitationIsFound()
        {
            var invitation = this.service.GetUserInvitation(GROUP_ID, USER_ID);

            Assert.AreEqual(this.invitations[0], invitation);
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
            this.repositoryMock.Setup(r => r.All).Returns(this.invitations.AsQueryable());
            this.unitOfWorkMock = new Mock<IUnitOfWork>();

            this.service = new InvitationService(factoryMock.Object,
                repositoryMock.Object, unitOfWorkMock.Object);
        }
    }
}
