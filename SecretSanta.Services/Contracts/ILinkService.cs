using SecretSanta.Models;

namespace SecretSanta.Services.Contracts
{
    public interface ILinkService
    {
        Link GetByGroupAndSender(string groupname, string senderUsername);

        void CreateLinks(Group group, string currentUserId);

        void CreateLink(Group group, User sender, User receiver);

        void CheckUserAcccessRights(string currentUsername, string username);

        void CheckLinkingProcessStarted(Group group);
    }
}
