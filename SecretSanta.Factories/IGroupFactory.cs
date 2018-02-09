using SecretSanta.Models;

namespace SecretSanta.Factories
{
    public interface IGroupFactory
    {
        Group Create(string name, User admin);
    }
}
