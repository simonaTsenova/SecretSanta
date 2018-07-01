using SecretSanta.Common.Exceptions;
using SecretSanta.Data.Contracts;
using SecretSanta.Models;
using SecretSanta.Models.Enumerations;
using SecretSanta.Services.Contracts;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Microsoft.AspNet.Identity;
using SecretSanta.Authentication.Contracts;
using SecretSanta.Factories;
using Constants = SecretSanta.Common.Constants;

namespace SecretSanta.Services
{
    public class UserService : IUserService
    {
        private readonly IAuthenticationProvider authenticationProvider;
        private readonly IUserFactory userFactory;
        private readonly IEfRepository<User> userRepository;

        public UserService(IAuthenticationProvider authenticationProvider,
            IUserFactory userFactory, IEfRepository<User> userRepository)
        {
            this.authenticationProvider = authenticationProvider;
            this.userFactory = userFactory;
            this.userRepository = userRepository;
        }

        public IEnumerable<User> GetAllUsers(int skip, int take, OrderType order, string searchPhrase)
        {
            var users = this.userRepository.All;

            if(!string.IsNullOrEmpty(searchPhrase))
            {
                users = users.Where(u => u.UserName.Contains(searchPhrase) || u.DisplayName.Contains(searchPhrase));
            }

            users = this.OrderUsers(order, users);

            if(skip == 0 && take == 0)
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

            if (user == null)
            {
                throw new ItemNotFoundException(Constants.USERNAME_NOT_FOUND);
            }

            return user;
        }

        public User GetUserById(string id)
        {
            var user = this.userRepository.All
                .Where(u => u.Id == id)
                .Include(u => u.Invitations)
                .Include(u => u.Groups)
                .FirstOrDefault();

            if (user == null)
            {
                throw new ItemNotFoundException(Constants.USER_ID_NOT_FOUND);
            }

            return user;
        }

        private IQueryable<User> OrderUsers(OrderType orderType, IQueryable<User> users)
        {
            if (orderType == OrderType.Descending)
            {
                users = users.OrderByDescending(u => u.DisplayName);
            }
            else
            {
                users = users.OrderBy(u => u.DisplayName);
            }

            return users;
        }

        public IdentityResult CreateUser(string email, string username, 
            string displayName, string firstname, string lastname, string password)
        { 
            var user = this.userFactory.Create(email, username, displayName, firstname, lastname);
            var identityResult = this.authenticationProvider.RegisterUser(user, password);

            return identityResult;
        }
    }
}
