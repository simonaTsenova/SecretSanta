using SecretSanta.Models;
using System;

namespace SecretSanta.Services.Contracts
{
    public interface IInvitationService
    {
        Invitation GetByGroupAndUser(string group, string user);

        void CreateInvitation(Guid groupId, DateTime sentDate, string receiverId);
    }
}
