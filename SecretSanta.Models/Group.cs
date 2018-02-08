using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSanta.Models
{
    public class Group
    {
        public Group()
        {
            this.Users = new HashSet<User>();
        }

        public Group(string name, string adminId)
            : base()
        {
            this.Name = name;
            this.AdminId = adminId;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string AdminId { get; set; }

        public virtual User Admin { get; set; }

        public bool hasLinkingProcessStarted { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
