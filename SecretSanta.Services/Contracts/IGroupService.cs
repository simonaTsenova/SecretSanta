using SecretSanta.Models;

namespace SecretSanta.Services.Contracts
{
    public interface IGroupService
    {
        Group CreateGroup(string name, User admin);

        Group GetGroupByName(string name);

        void AddParticipant(Group group, User user);

        bool RemoveParticipant(Group group, User user);
    }
}
