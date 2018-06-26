using SecretSanta.Web.Models.Users;
using System.Collections.Generic;

namespace SecretSanta.Web.Models.Groups
{
    public class DisplayGroupViewModel
    {
        public DisplayGroupViewModel()
        {
            this.Members = new List<DisplayUserViewModel>();
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
            this.Members = members;
        }

        public string GroupName { get; set; }

        public string Admin { get; set; }

        public ICollection<DisplayUserViewModel> Members;
    }
}