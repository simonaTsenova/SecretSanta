using SecretSanta.Web.Models.Invitations;
using System;

namespace SecretSanta.Web.Infrastructure.Factories
{
    public interface IInvitationViewModelFactory
    {
        InvitationViewModel Create(Guid id, DateTime sentDate, string groupName, string receiver);
    }
}
