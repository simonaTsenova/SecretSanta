using SecretSanta.Web.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SecretSanta.Web.Models.Groups
{
    public class DisplayGroupViewModel
    {
        public DisplayGroupViewModel()
        {
            this.members = new List<DisplayUserViewModel>();
        }

        public DisplayGroupViewModel(string name, string admin)
            : base()
        {
            this.GroupName = name;
            this.Admin = admin;
        }

        public DisplayGroupViewModel(string name, string admin, ICollection<DisplayUserViewModel> members)
        {
            this.GroupName = name;
            this.Admin = admin;
            this.members = members;
        }

        public string GroupName { get; set; }

        public string Admin { get; set; }

        public ICollection<DisplayUserViewModel> members;
    }
}