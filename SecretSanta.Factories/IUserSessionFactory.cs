using SecretSanta.Models;
using System;

namespace SecretSanta.Factories
{
    public interface IUserSessionFactory
    {
        UserSession Create(string userId, string authToken, DateTime expiresOn);
    }
}
