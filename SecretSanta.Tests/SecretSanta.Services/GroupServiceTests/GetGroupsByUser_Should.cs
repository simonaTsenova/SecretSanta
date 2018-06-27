using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using SecretSanta.Data.Contracts;
using SecretSanta.Factories;
using SecretSanta.Models;
using SecretSanta.Services;

namespace SecretSanta.Tests.SecretSanta.Services.GroupServiceTests
{
    [TestFixture]
    public class GetGroupsByUser_Should
    {
        private Mock<IEfRepository<Group>> repositoryMock;
        private Mock<IGroupFactory> factoryMock;
        private Mock<IUnitOfWork> unitOfWorkMock;
        private GroupService service;
        private IEnumerable<Group> groups;
        private IEnumerable<Group> expectedGroups;
        private const int SKIP = 0;
        private const int TAKE = 0;
        private const string USERNAME = "admin";

        [Test]
        public void ReturnCorrectGroupsCollection_WhenInvoked()
        {
            var result = this.service.GetGroupsByUser(USERNAME, SKIP, TAKE);

            Assert.AreEqual(this.expectedGroups.Count(), result.Count());
            CollectionAssert.AreEqual(this.expectedGroups, result);
        }
        
        [SetUp]
        public void Setup()
        {
            this.groups = new List<Group>()
            {
                new Group()
                {
                    Name = "pink",
                    Admin = new User() { UserName = USERNAME }
                },
                new Group()
                {
                    Name = "green",
                    Admin = new User() { UserName = "john" }
                }
            };
            this.expectedGroups = this.groups.Where(g => g.Admin.UserName.Equals(USERNAME));

            this.repositoryMock = new Mock<IEfRepository<Group>>();
            this.repositoryMock.Setup(r => r.All).Returns(this.groups.AsQueryable);

            this.factoryMock = new Mock<IGroupFactory>();
            this.unitOfWorkMock = new Mock<IUnitOfWork>();

            this.service = new GroupService(factoryMock.Object,
                repositoryMock.Object, unitOfWorkMock.Object);
        }
    }
}
