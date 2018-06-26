using Microsoft.AspNet.Identity;
using SecretSanta.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace SecretSanta.Authentication.Contracts
{
    public interface IAuthenticationProvider
    {
        IdentityResult RegisterUser(User user, string password);

        Task<HttpResponseMessage> GetAccessToken(string username, string password);

        string CurrentUserId { get; }

        string CurrentUserName { get; }
    }
}
