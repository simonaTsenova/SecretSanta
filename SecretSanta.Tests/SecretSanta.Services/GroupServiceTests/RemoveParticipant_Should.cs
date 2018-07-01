using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SecretSanta.Common;
using SecretSanta.Common.Exceptions;
using SecretSanta.Data.Contracts;
using SecretSanta.Factories;
using SecretSanta.Models;
using SecretSanta.Services;

namespace SecretSanta.Tests.SecretSanta.Services.GroupServiceTests
{
    [TestFixture]
    public class RemoveParticipant_Should
    {
        private Mock<IEfRepository<Group>> repositoryMock;
        private Mock<IGroupFactory> factoryMock;
        private Mock<IUnitOfWork> unitOfWorkMock;
        private GroupService service;
        private Group group;
        private List<User> users;

        [Test]
        public void CallUnitOfWorkCommit_WhenParticipantRemoved()
        {
            this.service.RemoveParticipant(group, this.users[0]);

            unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }

        [Test]
        public void RemoveUserFromGroup_WhenUserIsMember()
        {
            this.service.RemoveParticipant(group, this.users[0]);

            CollectionAssert.DoesNotContain(group.Users, this.users[0]);
        }

        [Test]
        public void ThrowItemAlreadyExistingException_WhenParticipantAlreadyGroupMember()
        {
            var ex = Assert.Throws<ItemNotFoundException>(() => this.service.RemoveParticipant(group, this.users[1]));
            Assert.AreEqual(Constants.PARTICIPANT_NOT_FOUND, ex.Message);
        }

        [SetUp]
        public void Setup()
        {
            this.repositoryMock = new Mock<IEfRepository<Group>>();
            this.factoryMock = new Mock<IGroupFactory>();
            this.unitOfWorkMock = new Mock<IUnitOfWork>();

            this.users = new List<User>()
            {
                new User() {UserName = "janedoe"},
                new User() { UserName = "johnfoe" }
            };
            this.group = new Group()
            {
                Name = "Sample group",
                Users = new List<User>()
                {
                    this.users[0]
                }
            };

            this.service = new GroupService(factoryMock.Object,
                repositoryMock.Object, unitOfWorkMock.Object);
        }
    }
}
