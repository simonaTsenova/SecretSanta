using System;
using System.Collections.Generic;

namespace SecretSanta.Models
{
    public class Group
    {
        public Group()
        {
            this.Users = new HashSet<User>();
        }

        public Group(string name, User admin)
            : base()
        {
            this.Name = name;
            this.Admin = admin;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string AdminId { get; set; }

        public virtual User Admin { get; set; }

        public bool hasLinkingProcessStarted { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
