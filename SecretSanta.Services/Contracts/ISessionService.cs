using SecretSanta.Models;

namespace SecretSanta.Services.Contracts
{
    public interface ISessionService
    {
        void CreateUserSession(string userName, string authtoken);

        void DeleteExpiredSessions();

        void InvalidateUserSession();

        UserSession GetCurrentSession();

        User GetCurrentUser();
    }
}
