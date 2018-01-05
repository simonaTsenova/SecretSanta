using SecretSanta.Models;

namespace SecretSanta.Services.Contracts
{
    public interface IUserService
    {
        User GetUserByUserName(string userName);
    }
}
