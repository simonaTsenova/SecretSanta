using SecretSanta.Models;
using SecretSanta.Models.Enumerations;
using System.Collections.Generic;

namespace SecretSanta.Services.Contracts
{
    public interface IUserService
    {
        User GetUserByUserName(string userName);

        User GetUserById(string id);

        IEnumerable<User> GetAllUsers(int skip, int take, OrderType order, string searchPhrase);

        IEnumerable<Group> GetUserGroups(User user, int skip, int take);

        IEnumerable<Invitation> GetUserInvitations(User user, int skip, int take, OrderType order);
    }
}
