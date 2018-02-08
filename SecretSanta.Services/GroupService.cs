using SecretSanta.Data.Contracts;
using SecretSanta.Factories;
using SecretSanta.Models;
using SecretSanta.Services.Contracts;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SecretSanta.Services
{
    public class GroupService : IGroupService
    {
        private readonly IGroupFactory groupFactory;
        private readonly IEfRepository<Group> groupRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IUserService userService;

        public GroupService(IGroupFactory groupFactory, IEfRepository<Group> groupRepository, 
            IUnitOfWork unitOfWork, IUserService userService)
        {
            this.groupFactory = groupFactory;
            this.groupRepository = groupRepository;
            this.unitOfWork = unitOfWork;
            this.userService = userService;
        }

        public Group CreateGroup(string name, string adminId)
        {
            var foundGroup = this.groupRepository.All
                .Where(g => g.Name == name)
                .FirstOrDefault();
            if (foundGroup != null)
            {
                return null;
            }

            var group = this.groupFactory.Create(name, adminId);
            group.Users = new HashSet<User>()
            {
                this.userService.GetUserById(adminId)
            };

            this.groupRepository.Add(group);
            this.unitOfWork.Commit();

            return group;
        }

        public Group GetGroupByName(string name)
        {
            var group = this.groupRepository.All
                .Where(g => g.Name == name)
                .Include(g => g.Admin)
                .FirstOrDefault();

            return group;
        }
    }
}
