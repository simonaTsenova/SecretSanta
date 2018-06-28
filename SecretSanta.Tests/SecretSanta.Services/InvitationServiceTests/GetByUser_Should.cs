using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using SecretSanta.Data.Contracts;
using SecretSanta.Factories;
using SecretSanta.Models;
using SecretSanta.Models.Enumerations;
using SecretSanta.Services;

namespace SecretSanta.Tests.SecretSanta.Services.InvitationServiceTests
{
    [TestFixture]
    public class GetByUser_Should
    {
        private Mock<IInvitationFactory> factoryMock;
        private Mock<IEfRepository<Invitation>> repositoryMock;
        private Mock<IUnitOfWork> unitOfWorkMock;
        private InvitationService service;
        private IEnumerable<Invitation> invitations;
        private IEnumerable<Invitation> expectedInvitations;
        private const int SKIP = 0;
        private const int TAKE = 0;
        private const string USERNAME = "admin";

        [Test]
        public void ReturnCorrectInvitationsCollection_WhenOrderIsAscending()
        {
            var result = this.service.GetByUser(USERNAME, SKIP, TAKE, OrderType.Ascending);

            Assert.AreEqual(this.expectedInvitations.Count(), result.Count());
            CollectionAssert.AreEqual(this.expectedInvitations.OrderBy(i => i.SentDate), result);
        }

        [Test]
        public void ReturnCorrectInvitationsCollection_WhenOrderIsDescending()
        {
            var result = this.service.GetByUser(USERNAME, SKIP, TAKE, OrderType.Descending);

            Assert.AreEqual(this.expectedInvitations.Count(), result.Count());
            CollectionAssert.AreEqual(this.expectedInvitations.OrderByDescending(i => i.SentDate), result);
        }

        [SetUp]
        public void SetUp()
        {
            this.invitations = new List<Invitation>()
            {
                new Invitation()
                {
                    SentDate = new DateTime(2018, 7, 1),
                    Receiver =  new User() { UserName = USERNAME}
                },
                new Invitation()
                {
                    SentDate = new DateTime(2018, 6, 30),
                    Receiver = new User() { UserName = USERNAME }
                },
                new Invitation()
                {
                    Receiver = new User() { UserName = "jack" }
                }
            };
            this.expectedInvitations = this.invitations.Where(i => i.Receiver.UserName == USERNAME);

            this.factoryMock = new Mock<IInvitationFactory>();
            this.repositoryMock = new Mock<IEfRepository<Invitation>>();
            repositoryMock.Setup(r => r.All).Returns(this.invitations.AsQueryable);
            this.unitOfWorkMock = new Mock<IUnitOfWork>();

            this.service = new InvitationService(factoryMock.Object,
                repositoryMock.Object, unitOfWorkMock.Object);
        }
    }
}
