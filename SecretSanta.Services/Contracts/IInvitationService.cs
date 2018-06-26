using SecretSanta.Models;
using SecretSanta.Models.Enumerations;
using System;
using System.Collections.Generic;

namespace SecretSanta.Services.Contracts
{
    public interface IInvitationService
    {
        Invitation GetById(string id);

        Invitation GetByGroupAndUser(Guid group, string user);

        IEnumerable<Invitation> GetByUser(string username, int skip, int take, OrderType order);

        void CreateInvitation(Guid groupId, DateTime sentDate, string receiverId);

        void DeleteInvitation(Invitation invitation);
    }
}
