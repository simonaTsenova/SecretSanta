using SecretSanta.Common;
using SecretSanta.Common.Exceptions;
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

        public GroupService(IGroupFactory groupFactory, IEfRepository<Group> groupRepository, 
            IUnitOfWork unitOfWork, IUserService userService)
        {
            this.groupFactory = groupFactory;
            this.groupRepository = groupRepository;
            this.unitOfWork = unitOfWork;
        }

        public void AddParticipant(Group group, User user)
        {
            if(group.Users.Contains(user))
            {
                throw new ItemAlreadyExistingException(Constants.USER_ALREADY_MEMBER_OF_GROUP);
            }

            group.Users.Add(user);
            this.unitOfWork.Commit();
        }

        public void RemoveParticipant(Group group, User user)
        {
            var isParticipant = group.Users.Contains(user);
            if(!isParticipant)
            {
                throw new ItemNotFoundException(Constants.PARTICIPANT_NOT_FOUND);
            }

            group.Users.Remove(user);
            this.unitOfWork.Commit();
        }

        public Group CreateGroup(string name, User admin)
        {
            var existingGroup = this.GetGroupByName(name);
            if (existingGroup != null)
            {
                throw new ItemAlreadyExistingException(Constants.GROUP_NAME_ALREADY_EXISTS);
            }

            var group = this.groupFactory.Create(name, admin);
            group.Users = new HashSet<User>()
            {
                admin
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
                .Include(g => g.Users)
                .FirstOrDefault();

            if (group == null)
            {
                throw new ItemNotFoundException(Constants.GROUP_NAME_NOT_FOUND);
            }

            return group;
        }

        public void MakeProcessStarted(Group group)
        {
            group.hasLinkingProcessStarted = true;

            this.unitOfWork.Commit();
        }

        public IEnumerable<Group> GetGroupsByUser(string username, int skip, int take)
        {
            var groups = this.groupRepository.All
                .Where(g => g.Admin.UserName == username)
                .ToList();

            if (skip == 0 && take == 0)
            {
                take = groups.Count();
            }

            groups = groups.OrderBy(g => g.Name).Skip(skip).Take(take).ToList();

            return groups;
        }
    }
}
