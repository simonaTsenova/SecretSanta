using SecretSanta.Models;
using System.Collections.Generic;

namespace SecretSanta.Services.Contracts
{
    public interface IGroupService
    {
        Group CreateGroup(string name, User admin);

        Group GetGroupByName(string name);

        void AddParticipant(Group group, User user);

        void RemoveParticipant(Group group, User user);

        void MakeProcessStarted(Group group);

        IEnumerable<Group> GetGroupsByUser(string username, int skip, int take);

        void CheckUserAcccessRights(string currentUsername, string username);
    }
}
