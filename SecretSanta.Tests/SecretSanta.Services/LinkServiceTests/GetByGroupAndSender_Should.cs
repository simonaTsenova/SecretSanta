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

namespace SecretSanta.Tests.SecretSanta.Services.LinkServiceTests
{
    [TestFixture]
    public class GetByGroupAndSender_Should
    {
        private Mock<IEfRepository<Link>> repositoryMock;
        private Mock<ILinkFactory> factoryMock;
        private Mock<IUnitOfWork> unitOfWorkMock;
        private LinkService service;
        private IEnumerable<Link> links;
        private Link expectedLink;
        private const string GROUPNAME = "pink";
        private const string USERNAME = "admin";

        [Test]
        public void ReturnCorrectLink_WhenProvidedValidGroupAndUser()
        {
            var result = service.GetByGroupAndSender(GROUPNAME, USERNAME);

            Assert.AreEqual(expectedLink, result);
        }

        [Test]
        public void ThrowItemNotFoundException_WhenProvidedInvalidGroupAndName()
        {
            var ex = Assert.Throws<ItemNotFoundException>(() => service.GetByGroupAndSender("chefs", "joe"));
            Assert.AreEqual(Constants.LINK_NOT_FOUND, ex.Message);
        }

        [SetUp]
        public void Setup()
        {
            this.links = new List<Link>()
            {
                new Link()
                {
                    Group = new Group() {Name = GROUPNAME },
                    Sender = new User() { UserName = USERNAME }
                },
                new Link()
                {
                    Group = new Group() {Name = "not found" },
                    Sender = new User() { UserName = "not admin" }
                },
            };
            this.expectedLink = this.links.First(l => l.Group.Name == GROUPNAME && l.Sender.UserName == USERNAME);

            this.repositoryMock = new Mock<IEfRepository<Link>>();
            repositoryMock.Setup(r => r.All).Returns(this.links.AsQueryable);

            this.factoryMock = new Mock<ILinkFactory>();
            this.unitOfWorkMock = new Mock<IUnitOfWork>();

            this.service = new LinkService(factoryMock.Object,
                repositoryMock.Object, unitOfWorkMock.Object);
        }
    }
}
