using SecretSanta.Data.Contracts;
using SecretSanta.Models;
using SecretSanta.Services.Contracts;
using System;
using System.Linq;

namespace SecretSanta.Services
{
    public class UserService : IUserService
    {
        private readonly IEfRepository<User> userRepository;

        public UserService(IEfRepository<User> userRepository)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException();
        }

        public User GetUserByUserName(string userName)
        {
            var user = this.userRepository.All
                .Where(u => u.UserName == userName)
                .FirstOrDefault();

            return user;
        }
    }
}
