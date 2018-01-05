using SecretSanta.Models;

namespace SecretSanta.Factories
{
    public interface IUserFactory
    {
        User Create(string email, string username, string displayName,
            string firstName, string lastName);
    }
}
