using SecretSanta.Models;
using SecretSanta.Models.Enumerations;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;

namespace SecretSanta.Services.Contracts
{
    public interface IUserService
    {
        User GetUserByUserName(string userName);

        User GetUserById(string id);

        IEnumerable<User> GetAllUsers(int skip, int take, OrderType order, string searchPhrase);

        IdentityResult CreateUser(string email, string username,
            string displayName, string firstname, string lastname, string password);
    }
}
