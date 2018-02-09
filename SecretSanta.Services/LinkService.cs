using SecretSanta.Data.Contracts;
using SecretSanta.Models;
using SecretSanta.Services.Contracts;
using System.Linq;

namespace SecretSanta.Services
{
    public class LinkService : ILinkService
    {
        private readonly IEfRepository<Link> linkRepository;
        private readonly IUnitOfWork unitOfWork;

        public LinkService(IEfRepository<Link> linkRepository, IUnitOfWork unitOfWork)
        {
            this.linkRepository = linkRepository;
            this.unitOfWork = unitOfWork;
        }

        public Link GetByGroupAndSender(string groupname, string senderUsername)
        {
            var link = this.linkRepository.All
                .Where(l => l.Group.Name == groupname && l.Sender.UserName == senderUsername)
                .FirstOrDefault();

            return link;
        }
    }
}
