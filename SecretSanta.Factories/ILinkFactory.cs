using SecretSanta.Models;

namespace SecretSanta.Factories
{
    public interface ILinkFactory
    {
        Link Create(Group group, User sender, User receiver);
    }
}
