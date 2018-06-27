using SecretSanta.Data.Contracts;
using SecretSanta.Models;
using SecretSanta.Services.Contracts;
using System.Linq;
using System;
using SecretSanta.Factories;
using SecretSanta.Common.Exceptions;
using SecretSanta.Common;
using SecretSanta.Models.Enumerations;
using System.Collections.Generic;

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

        public Invitation GetById(string id)
        {
            var invitation = this.invitationRepository.All
                .Where(i => i.Id.ToString() == id)
                .FirstOrDefault();

            if (invitation == null)
            {
                throw new ItemNotFoundException(Constants.INVITATION_ID_NOT_FOUND);
            }

            return invitation;
        }

        public Invitation GetByGroupAndUser(Guid groupId, string userId)
        {
            var invitation = this.invitationRepository.All
                .Where(i => i.Group.Id == groupId && i.Receiver.Id == userId)
                .FirstOrDefault();

            if (invitation == null)
            {
                throw new ItemNotFoundException(Constants.INVITATION_NOT_FOUND);
            }

            return invitation;
        }

        public IEnumerable<Invitation> GetByUser(string username, int skip, int take, OrderType order)
        {
            var invitations = this.invitationRepository.All
                .Where(i => i.Receiver.UserName == username)
                .ToList();

            if (order == OrderType.Descending)
            {
                invitations = invitations.OrderByDescending(i => i.SentDate).ToList();
            }
            else
            {
                invitations = invitations.OrderBy(i => i.SentDate).ToList();
            }

            if (skip == 0 && take == 0)
            {
                take = invitations.Count();
            }

            invitations = invitations.Skip(skip).Take(take).ToList();

            return invitations;
        }

        public void CreateInvitation(Guid groupId, DateTime sentDate, string receiverId)
        {
            try
            {
                var existingInvitation = this.GetByGroupAndUser(groupId, receiverId);
                if (existingInvitation != null)
                {
                    throw new ItemAlreadyExistingException(Constants.INVITATION_ALREADY_EXISTS);
                }
            }
            catch (ItemNotFoundException)
            {
                var invitation = this.invitationFactory.Create(groupId, sentDate, receiverId);

                this.invitationRepository.Add(invitation);
                this.unitOfWork.Commit();
            }
        }

        public void DeleteInvitation(Invitation invitation)
        {
            this.invitationRepository.Delete(invitation);
            this.unitOfWork.Commit();
        }

        public void CheckUserAcccessRights(string currentUsername, string username)
        {
            if (currentUsername != username)
            {
                throw new AccessForbiddenException(Constants.INVITATION_ACCESS_FORBIDDEN);
            }
        }

        public Invitation GetUserInvitation(Guid groupId, string userId)
        {
            try
            {
                var invitation = this.GetByGroupAndUser(groupId, userId);

                return invitation;
            }
            catch (ItemNotFoundException)
            {
                throw new AccessForbiddenException(Constants.USER_HAS_NO_INVITATIONS_FOR_GROUP);
            }
        }
    }
}
