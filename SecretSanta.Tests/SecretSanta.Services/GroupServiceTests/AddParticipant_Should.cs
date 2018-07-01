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
    public class AddParticipant_Should
    {
        private Mock<IEfRepository<Group>> repositoryMock;
        private Mock<IGroupFactory> factoryMock;
        private Mock<IUnitOfWork> unitOfWorkMock;
        private GroupService service;
        private Group group;
        private List<User> users;

        [Test]
        public void CallUnitOfWorkCommit_WhenParticipantAdded()
        {
            this.service.AddParticipant(group, this.users[0]);

            unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }

        [Test]
        public void AddUserToGroup_WhenUserIsNotMember()
        {
            this.service.AddParticipant(group, this.users[0]);

            CollectionAssert.Contains(group.Users, this.users[0]);
        }

        [Test]
        public void ThrowItemAlreadyExistingException_WhenParticipantAlreadyGroupMember()
        {
            var ex = Assert.Throws<ItemAlreadyExistingException>(() => this.service.AddParticipant(group, this.users[1]));
            Assert.AreEqual(Constants.USER_ALREADY_MEMBER_OF_GROUP, ex.Message);
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
                    this.users[1]
                }
            };

            this.service = new GroupService(factoryMock.Object,
                repositoryMock.Object, unitOfWorkMock.Object);
        }
    }
}
