using SecretSanta.Models;

namespace SecretSanta.Services.Contracts
{
    public interface ILinkService
    {
        Link GetByGroupAndSender(string groupname, string senderUsername);
    }
}
