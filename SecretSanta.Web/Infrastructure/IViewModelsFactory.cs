using SecretSanta.Web.Models.Users;

namespace SecretSanta.Web.Infrastructure
{
    public interface IViewModelsFactory
    {
        DisplayUserViewModel CreateDisplayUserViewModel(string email, string firstName, string lastName, string displayName, string userName);
    }
}
