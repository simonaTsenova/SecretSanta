using SecretSanta.Models;
using System;

namespace SecretSanta.Factories
{
    public interface IInvitationFactory
    {
        Invitation Create(Guid groupId, DateTime sentDate, string receiverId);
    }
}
