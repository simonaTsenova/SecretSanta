using SecretSanta.Web.Models.Groups;
using SecretSanta.Web.Models.Users;
using System.Collections.Generic;

namespace SecretSanta.Web.Infrastructure.Factories
{
    public interface IDisplayGroupViewModelFactory
    {
        DisplayGroupViewModel CreateDisplayGroupViewModel(string name, string admin);

        DisplayGroupViewModel CreateDisplayGroupViewModel(string name, string admin, ICollection<DisplayUserViewModel> members);
    }
}
