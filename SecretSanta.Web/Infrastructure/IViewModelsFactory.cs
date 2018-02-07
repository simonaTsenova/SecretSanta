using SecretSanta.Web.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSanta.Web.Infrastructure
{
    public interface IViewModelsFactory
    {
        DisplayUserViewModel Create(string email, string firstName, string lastName, string displayName, string userName);
    }
}
