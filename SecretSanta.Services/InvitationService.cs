using SecretSanta.Data.Contracts;
using SecretSanta.Models;
using SecretSanta.Services.Contracts;
using System.Linq;
using System;
using SecretSanta.Factories;

namespace SecretSanta.Services
{
    public class InvitationService : IInvitationService
    {
        private readonly IInvitationFactory invitationFactory;
        private readonly IEfRepository<Invitation> invitationRepository;
        private readonly IUnitOfWork unitOfWork;

        public InvitationService(IInvitationFactory invitationFactory, 
            IEfRepository<Invitation> invitationRepository, IUnitOfWork unitOfWork)
        {
            this.invitationFactory = invitationFactory;
            this.invitationRepository = invitationRepository;
            this.unitOfWork = unitOfWork;
        }

        public Invitation GetByGroupAndUser(string group, string user)
        {
            var invitation = this.invitationRepository.All
                .Where(i => i.Group.Name == group && i.Receiver.UserName == user)
                .FirstOrDefault();

            return invitation;
        }

        public void CreateInvitation(Guid groupId, DateTime sentDate, string receiverId)
        {
            var invitation = this.invitationFactory.Create(groupId, sentDate, receiverId);

            this.invitationRepository.Add(invitation);
            this.unitOfWork.Commit();
        }
    }
}
