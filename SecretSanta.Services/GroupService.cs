﻿using SecretSanta.Data.Contracts;
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
            group.Users.Add(user);
            this.unitOfWork.Commit();
        }

        public bool RemoveParticipant(Group group, User user)
        {
            var isParticipant = group.Users.Contains(user);
            if(!isParticipant)
            {
                return false;
            }

            group.Users.Remove(user);
            this.unitOfWork.Commit();

            return true;
        }

        public Group CreateGroup(string name, User admin)
        {
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

            return group;
        }

        public void MakeProcessStarted(Group group)
        {
            group.hasLinkingProcessStarted = true;

            this.unitOfWork.Commit();
        }
    }
}
