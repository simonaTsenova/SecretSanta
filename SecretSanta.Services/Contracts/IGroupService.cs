using SecretSanta.Models;

namespace SecretSanta.Services.Contracts
{
    public interface IGroupService
    {
        Group CreateGroup(string name, string adminId);
    }
}
