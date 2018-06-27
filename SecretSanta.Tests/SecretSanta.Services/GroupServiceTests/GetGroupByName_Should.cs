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

namespace SecretSanta.Tests.SecretSanta.Services.GroupServiceTests
{
    [TestFixture]
    public class GetGroupByName_Should
    {
        private Mock<IEfRepository<Group>> repositoryMock;
        private Mock<IGroupFactory> factoryMock;
        private Mock<IUnitOfWork> unitOfWorkMock;
        private GroupService service;
        private IEnumerable<Group> groups;
        private Group expectedGroup;
        private const string NAME = "pink";

        [Test]
        public void ReturnCorrectGroup_WhenProvidedValidName()
        {
            var result = service.GetGroupByName(NAME);

            Assert.AreEqual(expectedGroup, result);
        }

        [Test]
        public void ThrowItemNotFoundException_WhenProvidedInvalidName()
        {
            var ex = Assert.Throws<ItemNotFoundException>(() => service.GetGroupByName("not found"));
            Assert.AreEqual(Constants.GROUP_NAME_NOT_FOUND, ex.Message);
        }

        [SetUp]
        public void Setup()
        {
            this.groups = new List<Group>()
            {
                new Group() { Name = NAME },
                new Group() { Name = "green" },
            };
            this.expectedGroup = this.groups.First(g => g.Name.Equals(NAME));

            this.repositoryMock = new Mock<IEfRepository<Group>>();
            repositoryMock.Setup(r => r.All).Returns(this.groups.AsQueryable);

            this.factoryMock = new Mock<IGroupFactory>();
            this.unitOfWorkMock = new Mock<IUnitOfWork>();

            this.service = new GroupService(factoryMock.Object,
                repositoryMock.Object, unitOfWorkMock.Object);
        }
    }
}
