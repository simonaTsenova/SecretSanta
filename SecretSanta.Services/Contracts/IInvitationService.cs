using SecretSanta.Models;
using System;

namespace SecretSanta.Services.Contracts
{
    public interface IInvitationService
    {
        Invitation GetById(string id);

        Invitation GetByGroupAndUser(string group, string user);

        void CreateInvitation(Guid groupId, DateTime sentDate, string receiverId);

        void DeleteInvitation(Invitation invitation);
    }
}
