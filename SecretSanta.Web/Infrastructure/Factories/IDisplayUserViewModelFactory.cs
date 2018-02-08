using SecretSanta.Web.Models.Groups;
using SecretSanta.Web.Models.Users;
using System.Collections.Generic;

namespace SecretSanta.Web.Infrastructure.Factories
{
    public interface IDisplayUserViewModelFactory
    {
        DisplayUserViewModel CreateDisplayUserViewModel(string email, string firstName, string lastName, string displayName, string userName);
    }
}
