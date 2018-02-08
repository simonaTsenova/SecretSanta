using SecretSanta.Data.Contracts;
using SecretSanta.Models;
using SecretSanta.Models.Enumerations;
using SecretSanta.Services.Contracts;
using System;
using System.Collections.Generic;
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

        public IEnumerable<User> GetAllUsers(int skip, int take, OrderType order, string searchPhrase)
        {
            var users = this.userRepository.All;

            if(!string.IsNullOrEmpty(searchPhrase))
            {
                users = users.Where(u => u.UserName.Contains(searchPhrase) || u.DisplayName.Contains(searchPhrase));
            }

            if(order == OrderType.Descending)
            {
                users = users.OrderByDescending(u => u.DisplayName);
            }
            else
            {
                users = users.OrderBy(u => u.DisplayName);
            }

            if(take == 0)
            {
                take = users.Count();
            }

            users = users.Skip(skip).Take(take);

            return users.ToList();
        }

        public User GetUserByUserName(string userName)
        {
            var user = this.userRepository.All
                .Where(u => u.UserName == userName)
                .FirstOrDefault();

            return user;
        }

        public User GetUserById(string id)
        {
            var user = this.userRepository.All
                .Where(u => u.Id == id)
                .FirstOrDefault();

            return user;
        }
    }
}
