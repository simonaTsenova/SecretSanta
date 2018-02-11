using SecretSanta.Models;

namespace SecretSanta.Services.Contracts
{
    public interface ISessionService
    {
        void CreateUserSession(User user, string authtoken);

        void DeleteExpiredSessions();

        void InvalidateUserSession();

        UserSession GetCurrentSession();

        User GetCurrentUser();

        UserSession GetSessionByUserId(string userId);
    }
}
