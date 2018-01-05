using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SecretSanta.Models
{
    public class User : IdentityUser
    {
        public User()
        {
        }

        public User(string email, string username, string displayName,
            string firstName, string lastName)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.DisplayName = displayName;
            this.Email = email;
            this.UserName = username;
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [MinLength(6)]
        public string DisplayName { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager,
            string authenticationType)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);

            return userIdentity;
        }
    }
}
