using SecretSanta.Common;
using SecretSanta.Common.Exceptions;
using SecretSanta.Data.Contracts;
using SecretSanta.Factories;
using SecretSanta.Models;
using SecretSanta.Services.Contracts;
using System;
using System.Linq;

namespace SecretSanta.Services
{
    public class LinkService : ILinkService
    {
        private readonly ILinkFactory linkFactory;
        private readonly IEfRepository<Link> linkRepository;
        private readonly IUnitOfWork unitOfWork;

        public LinkService(ILinkFactory linkFactory, IEfRepository<Link> linkRepository, IUnitOfWork unitOfWork)
        {
            this.linkFactory = linkFactory;
            this.linkRepository = linkRepository;
            this.unitOfWork = unitOfWork;
        }

        public Link GetByGroupAndSender(string groupname, string senderUsername)
        {
            var link = this.linkRepository.All
                .Where(l => l.Group.Name == groupname && l.Sender.UserName == senderUsername)
                .FirstOrDefault();

            if (link == null)
            {
                throw new ItemNotFoundException(Constants.LINK_NOT_FOUND);
            }

            return link;
        }

        public void CreateLinks(Group group, string currentUserId)
        {
            if (currentUserId != group.Admin.Id)
            {
                throw new AccessForbiddenException(Constants.LINKING_PROCESS_START_FORBIDDEN);
            }

            var members = group.Users.ToList();
            var availableReceivers = Enumerable.Range(0, members.Count).ToList();
            var random = new Random();

            for (int i = 0; i < members.Count; i++)
            {
                var randomIndex = random.Next(0, availableReceivers.Count);
                var currentReceiverIndex = availableReceivers[randomIndex];

                while (currentReceiverIndex == i)
                {
                    randomIndex = random.Next(0, availableReceivers.Count);
                    currentReceiverIndex = availableReceivers[randomIndex];
                }

                availableReceivers.RemoveAt(randomIndex);

                this.CreateLink(group, members[i], members[currentReceiverIndex]);
            }
        }

        private void CreateLink(Group group, User sender, User receiver)
        {
            var link = this.linkFactory.Create(group, sender, receiver);

            this.linkRepository.Add(link);
            this.unitOfWork.Commit();
        }

        public void CheckUserAcccessRights(string currentUsername, string username)
        {
            if (currentUsername != username)
            {
                throw new AccessForbiddenException(Constants.LINKS_ACCESS_FORBIDDEN);
            }
        }

        public void CheckLinkingProcessStarted(Group group)
        {
            if (!group.hasLinkingProcessStarted)
            {
                throw new ItemNotFoundException(Constants.LINKING_PROCESS_NOT_STARTED);
            }
        }
    }
}
