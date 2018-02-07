using SecretSanta.Models;
using SecretSanta.Models.Enumerations;
using System.Collections.Generic;

namespace SecretSanta.Services.Contracts
{
    public interface IUserService
    {
        User GetUserByUserName(string userName);

        IEnumerable<User> GetAllUsers(int skip, int take, OrderType order, string searchPhrase);
    }
}
